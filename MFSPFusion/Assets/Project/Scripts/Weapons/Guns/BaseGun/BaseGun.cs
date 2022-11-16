using UnityEngine;
using Andrius.Core.Utils;
using Fusion;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System;
using Andrius.Core.Timers;
using Timer = Andrius.Core.Timers.Timer;

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

    [SerializeField] bool unlimtedAmmo = false;
    [HideInInspector] public Transform raycastOrigin;

    protected TickTimer cooldownTimer;
    NetworkObject muzleFlashObject;
    List<ParticleSystem> muzleFlashs = new List<ParticleSystem>();
    BaseCharacter controller;
    Timer reloadTimer;


   protected override void Start()
    {
        base.Start();
        reloadTimer = new Timer(data.reloadTime, Reload, false, true);
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
    public override void Update()
    {
        if (!reloadTimer.IsDone())
        {
            reloadTimer.StartTimer();
        }
        base.Update();
        if (currentAmount <= 0)
        {
            if (Input.GetKeyDown(KeyCode.R) && reloadTimer.IsDone())
            {
                reloadTimer.SetTimer(data.reloadTime, false);
            }
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

    }

    public virtual void Shoot()
    {
        if (!unlimtedAmmo)
        {
            currentAmount -= 1;
            controller.statsScreen.SetAmmo(currentAmount, maxAmmoAmount);
        }
        animator.SetTrigger(shootTrigger);
        Recoil.onRecoil?.Invoke(this);
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
                    surface = hit.Collider.GetComponent<Surface>();
                    // TODO: Add some logic what will happens if hit other objects
                }
                if (surface != null)
                {
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
        if (maxAmmoAmount - maxAmoAtClipAmount >= 0)
        {
            currentAmount = maxAmoAtClipAmount;
            maxAmmoAmount -= maxAmoAtClipAmount;
            controller.statsScreen.SetAmmo(currentAmount, maxAmmoAmount);
        }
        else
        {
            currentAmount = maxAmmoAmount;
            maxAmmoAmount = 0;
            controller.statsScreen.SetAmmo(currentAmount, maxAmmoAmount);
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
