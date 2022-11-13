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
    public Transform raycastOrigin;
    public GunType gunType = GunType.Single;

    protected int currentAmmoAmount;

    void Start()
    {
        raycastOrigin = StaticFunctions.FindChild(transform.root, "MainCamera");
    }

    public override void Attack()
    {
        if (raycastOrigin == null) return;
        if (GetInput<NetworkInputs>(out var input) == false) return;
        Debug.DrawRay(raycastOrigin.position, raycastOrigin.forward * data.weaponRange, Color.red);
        if (input.buttons.IsSet(MyButtons.Fire) && gunType == GunType.Single)
        {
            if (data.shootType == ShootType.Raycast)
            {
                var hitOptions = HitOptions.IncludePhysX | HitOptions.SubtickAccuracy | HitOptions.IgnoreInputAuthority;
                if (Runner.LagCompensation.Raycast(raycastOrigin.position, raycastOrigin.forward, data.weaponRange, Object.InputAuthority, out LagCompensatedHit hit, data.weaponMask, hitOptions))
                {
                    if (hit.GameObject != null)
                    {
                        if (Runner.IsServer)
                        {
                            NetworkObject gm = Runner.Spawn(data.hitEffectPrefab, hit.Point, Quaternion.identity);

                        }
                    }
                }
            }
        }
    }
    public abstract void Reload();
}
