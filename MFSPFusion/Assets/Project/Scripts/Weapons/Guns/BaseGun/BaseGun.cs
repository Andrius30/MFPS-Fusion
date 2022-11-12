using UnityEngine;
using Andrius.Core.Utils;

public abstract class BaseGun : Weapon
{
    public int maxAmmoAmount;
    public int amoAmount;
    public Transform raycastOrigin;


    void Awake()
    {
        raycastOrigin = StaticFunctions.FindChild(transform.root, "Main Camera");
    }
    protected int currentAmmoAmount;

    public abstract override void Attack();
    public abstract void Reload();

}
