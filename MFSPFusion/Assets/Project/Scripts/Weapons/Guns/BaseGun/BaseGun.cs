using UnityEngine;
using Andrius.Core.Utils;
using Fusion;
using System.Collections.Generic;

public enum GunType
{
    Single,
    Auto
}

public abstract class BaseGun : Weapon
{
    public int maxAmmoAmount;
    public int amoAmount;
    public Transform weaponBarel;
    public GunType gunType = GunType.Single;

    [HideInInspector] public Transform raycastOrigin;

    protected int currentAmmoAmount;
    protected TickTimer cooldownTimer;
    NetworkObject muzleFlashObject;
    List<ParticleSystem> muzleFlashs = new List<ParticleSystem>();

    void Start()
    {
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
        Debug.DrawRay(raycastOrigin.position, raycastOrigin.forward * data.weaponRange, Color.red);
        if (input.buttons.IsSet(MyButtons.Fire) && gunType == GunType.Single)
        {
            Shoot();
        }
        else if (input.buttons.IsSet(MyButtons.FireHold) && gunType == GunType.Auto)
        {
            Shoot();
        }
    }

    void Shoot()
    {
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

    void ActivateParticleSystem() => muzleFlashs.ForEach(x => x.Play());

    void CreateProjectile(Vector3 direction)
    {
        NetworkObject projectileObj = Runner.Spawn(data.hitScanProjectilePrefab, weaponBarel.position, weaponBarel.rotation, Object.InputAuthority);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        projectile.direction = direction;
    }
    public abstract void Reload();
}
