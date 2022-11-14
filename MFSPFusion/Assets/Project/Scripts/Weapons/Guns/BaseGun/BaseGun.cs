using UnityEngine;
using Andrius.Core.Utils;
using Fusion;

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

    void Start()
    {
        raycastOrigin = StaticFunctions.FindChild(transform.root, "MainCamera");
    }

    public override void Attack()
    {
        if (!cooldownTimer.ExpiredOrNotRunning(Runner)) return;
        if (raycastOrigin == null) return;
        if (GetInput<NetworkInputs>(out var input) == false) return;
        Debug.DrawRay(raycastOrigin.position, raycastOrigin.forward * data.weaponRange, Color.red);
        if (input.buttons.IsSet(MyButtons.Fire) && gunType == GunType.Single)
        {
            cooldownTimer = TickTimer.CreateFromSeconds(Runner, data.cooldown);
            if (data.shootType == ShootType.Raycast)
            {
                var hitOptions = HitOptions.IncludePhysX | HitOptions.SubtickAccuracy | HitOptions.IgnoreInputAuthority;
                if (Runner.LagCompensation.Raycast(raycastOrigin.position, raycastOrigin.forward, data.weaponRange, Object.InputAuthority, out LagCompensatedHit hit, data.weaponMask, hitOptions))
                {
                    if (hit.Hitbox != null) // hit player
                    {
                        if (Runner.IsServer)
                        {
                            // TODO: Add damagable system
                        }
                    }
                    else if (hit.Collider != null) // hit something else
                    {
                        // TODO: Add some logic what will happens if hit other objects
                    }
                    CreatehitEffect(hit);
                    Vector3 dir = (hit.Point - weaponBarel.position).normalized;
                    CreateProjectile(dir);
                }
                else
                {
                    CreateProjectile(weaponBarel.forward);
                }
            }
        }
    }

    void CreatehitEffect(LagCompensatedHit hit)
    {
        NetworkObject gm = Runner.Spawn(data.hitEffectPrefab, hit.Point, Quaternion.identity);
    }
    void CreateProjectile(Vector3 direction)
    {
        NetworkObject projectileObj = Runner.Spawn(data.hitScanProjectilePrefab, weaponBarel.position, weaponBarel.rotation, Object.InputAuthority);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        projectile.direction = direction;
    }
    public abstract void Reload();
}
