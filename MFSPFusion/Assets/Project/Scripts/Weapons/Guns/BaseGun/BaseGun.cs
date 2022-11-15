using UnityEngine;
using Andrius.Core.Utils;
using Fusion;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System;

public enum GunType
{
    Semi,
    Auto
}

public abstract class BaseGun : Weapon
{
    public static Action onWeaponSwitched;

    public int maxAmmoAmount;
    public int maxAmoAtClipAmount;
    [ReadOnly] int currentAmount;
    public Transform weaponBarel;
    public GunType gunType = GunType.Semi;

    [HideInInspector] public Transform raycastOrigin;

    protected int currentAmmoAmount;
    protected TickTimer cooldownTimer;
    NetworkObject muzleFlashObject;
    List<ParticleSystem> muzleFlashs = new List<ParticleSystem>();
    BaseCharacter controller;
    [Networked] TickTimer reloadTimer { get; set; }

    void Start()
    {
        reloadTimer = TickTimer.CreateFromSeconds(Runner, data.reloadTime);
        currentAmount = maxAmoAtClipAmount;
        controller = transform.root.GetComponent<BaseCharacter>();
        controller.statsScreen.SetAmmo(currentAmount, maxAmmoAmount);
        if (Runner.IsServer)
        {
            muzleFlashObject = Runner.Spawn(data.muzleFlashPrefab, weaponBarel.position, weaponBarel.rotation, Object.InputAuthority);
            muzleFlashObject.transform.SetParent(weaponBarel);
            muzleFlashs.AddRange(StaticFunctions.GetAllComponentsInChildren<ParticleSystem>(muzleFlashObject.transform.GetChild(0)));
            raycastOrigin = StaticFunctions.FindChild(transform.root, "MainCamera");
        }
    }

    public override void Attack()
    {
        if (!cooldownTimer.ExpiredOrNotRunning(Runner)) return;
        if (raycastOrigin == null) return;
        if (GetInput<NetworkInputs>(out var input) == false) return;
        if (currentAmount > 0)
        {
            Debug.DrawRay(raycastOrigin.position, raycastOrigin.forward * data.weaponRange, Color.red);
            if (input.buttons.IsSet(MyButtons.Fire) && gunType == GunType.Semi)
            {
                Shoot();
            }
            else if (input.buttons.IsSet(MyButtons.FireHold) && gunType == GunType.Auto)
            {
                Shoot();
            }
        }
        else
        {
            if (maxAmmoAmount > 0)
                Reload();
        }
    }

    public virtual void Shoot()
    {
        currentAmount -= 1;
        controller.statsScreen.SetAmmo(currentAmount, maxAmmoAmount);

        ActivateParticleSystem();
        cooldownTimer = TickTimer.CreateFromSeconds(Runner, data.cooldown);
        if (data.shootType == ShootType.Raycast)
        {
            var hitOptions = HitOptions.IncludePhysX | HitOptions.SubtickAccuracy | HitOptions.IgnoreInputAuthority;
            if (Runner.LagCompensation.Raycast(raycastOrigin.position, raycastOrigin.forward, data.weaponRange, Object.InputAuthority, out LagCompensatedHit hit, data.weaponMask, hitOptions))
            {
                Surface surface = null;
                if (hit.Hitbox != null) // hit player
                {
                    surface = hit.Hitbox.Root.GetComponent<Surface>();
                    var character = hit.Hitbox.Root.GetComponent<PlayerController>();
                    if (character != null)
                    {
                        character.ApplyDamage(data.weaponDamage);
                    }
                }
                else if (hit.Collider != null) // hit something else
                {
                    Debug.Log($"hit collider");
                    surface = hit.Collider.GetComponent<Surface>();
                    // TODO: Add some logic what will happens if hit other objects
                }
                if (surface != null)
                {
                    Debug.Log($"Surface {surface.surfaceType}");
                    data.PlayImpactEffect(Runner, hit, surface);
                }
                Vector3 dir = (hit.Point - weaponBarel.position).normalized;
                CreateProjectile(dir);
            }
            else
            {
                CreateProjectile(weaponBarel.forward);
            }
        }
    }
    public virtual void Reload()
    {
        if (currentAmount <= 0)
        {
            Debug.Log($"Reloading");
            if (reloadTimer.ExpiredOrNotRunning(Runner))
            {
                if (maxAmmoAmount - maxAmoAtClipAmount >= 0)
                {
                    currentAmmoAmount = maxAmoAtClipAmount;
                    maxAmmoAmount -= maxAmoAtClipAmount;
                    controller.statsScreen.SetAmmo(currentAmount, maxAmmoAmount);
                }
                else
                {
                    currentAmmoAmount = maxAmmoAmount;
                    maxAmmoAmount = 0;
                    controller.statsScreen.SetAmmo(currentAmount, maxAmmoAmount);
                }
                Debug.Log($"Reload end");
            }
        }
    }

    void UpdateUI()
    {
        controller.statsScreen.SetAmmo(currentAmount, maxAmmoAmount);
        controller.statsScreen.SetGunIcon(data.weaponIcon);
    }
    void ActivateParticleSystem() => muzleFlashs.ForEach(x => x.Play());
    void CreateProjectile(Vector3 direction)
    {
        NetworkObject projectileObj = Runner.Spawn(data.hitScanProjectilePrefab, weaponBarel.position, weaponBarel.rotation, Object.InputAuthority);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        projectile.direction = direction;
    }
    public virtual void OnEnable()
    {
        onWeaponSwitched += UpdateUI;
    }
    public virtual void OnDisable()
    {
        onWeaponSwitched -= UpdateUI;
    }
}
