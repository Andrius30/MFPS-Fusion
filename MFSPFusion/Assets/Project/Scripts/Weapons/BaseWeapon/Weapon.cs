using Fusion;
using UnityEngine;

public abstract class Weapon : NetworkBehaviour
{
    public WeaponData data;
    protected Animator animator;
    protected int shootTrigger = Animator.StringToHash("Shoot");

    protected virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public abstract void Attack();

    public virtual void Update() { }
    public override void FixedUpdateNetwork() { Attack(); }

    public override void Spawned()
    {

        foreach (var weaponHolder in FindObjectsOfType<WeaponsHolder>())
        {
            if (weaponHolder.weaponHolderId == Object.InputAuthority.PlayerId)
            {
                transform.SetParent(weaponHolder.transform);
                transform.localPosition = data.weaponPositionAtHand;
                transform.localRotation = data.weaponRotationAthand;

                if (!Runner.IsServer)
                {
                    weaponHolder.weaponsList.Add(gameObject);
                }
                return;
            }
            else
            {
            }
        }
    }
}
