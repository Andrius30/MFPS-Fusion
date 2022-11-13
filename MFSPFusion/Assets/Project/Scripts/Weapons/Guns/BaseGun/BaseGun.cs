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

    void Awake()
    {
        raycastOrigin = StaticFunctions.FindChild(transform.root, "Main Camera");
    }
    public override void Update()
    {
        base.Update();
        Attack();
    }
    public override void Attack()
    {
        Debug.Log($"Attack");
        if (GetInput<NetworkInputs>(out var input) == false) return;
        if (input.buttons.IsSet(MyButtons.Fire) && gunType == GunType.Single)
        {
            Debug.Log($"Pressed to shoot");
            if (data.shootType == ShootType.Raycast)
            {
                var hitOptions = HitOptions.IncludePhysX | HitOptions.SubtickAccuracy | HitOptions.IgnoreInputAuthority;
                if (Runner.LagCompensation.Raycast(raycastOrigin.position, raycastOrigin.forward, data.weaponRange, Object.InputAuthority, out LagCompensatedHit hit, data.weaponMask, hitOptions))
                {
                    if (hit.GameObject != null)
                    {
                        #region Debuging
                        Debug.Log($"Collider hit");
                        GameObject gm = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), hit.Point, Quaternion.identity);
                        Destroy(gm.GetComponent<Collider>());
                        gm.transform.localScale = new Vector3(.3f, .3f, .3f);
                        Destroy(gm, 3f);
                        #endregion
                    }
                }
            }
        }
    }
    public abstract void Reload();



    void OnDrawGizmos()
    {
        if (raycastOrigin == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(raycastOrigin.position, (raycastOrigin.position + raycastOrigin.forward) * data.weaponRange);
    }
}
