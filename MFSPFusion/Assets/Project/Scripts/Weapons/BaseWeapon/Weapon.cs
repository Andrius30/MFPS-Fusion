using Fusion;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public abstract class Weapon : NetworkBehaviour
{
    public int weaponID;
    public WeaponData data;
    public bool isDropable = false;
    public NetworkObject thisWeaponPrefab;
    [Networked] public NetworkBool isEquiped { get; set; }
    protected Animator animator;
    protected int shootTrigger = Animator.StringToHash("Shoot");
    public Rigidbody rb;
    public BoxCollider weaponCollider;
    public BoxCollider weaponTriggerCollider;


    protected virtual void Start()
    {
        if (!isEquiped) return;
        animator = GetComponentInChildren<Animator>();
    }

    public abstract void Attack();

    public virtual void Update()
    {
    }
    public override void FixedUpdateNetwork()
    {
        if (!isEquiped) return;
        Attack();
    }

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
                    weaponHolder.weaponsList.Add(this);
                }
                return;
            }
            else
            {
            }
        }
    }
    public void DropWeapon()
    {
        NetworkObject obj = null;

        if (Runner.IsServer)
        {
            WeaponsHolder holder = transform.parent.GetComponent<WeaponsHolder>();
            foreach (var weapon in holder.weapons)
            {
                if (weapon.weaponID == weaponID)
                {
                    obj = Runner.Spawn(weapon.gameObject, transform.position, transform.rotation);
                    var wep = obj.GetComponent<Weapon>();
                    wep.isEquiped = false;
                    ToggleComponents(obj, true);
                    rb.AddForce(transform.forward * 1, ForceMode.Impulse);
                    rb.AddForce(transform.up * 5, ForceMode.Impulse);
                    return;
                }
            }
        }
    }

    void ToggleComponents(NetworkObject obj, bool enable)
    {
        rb = obj.GetComponent<Rigidbody>();
        rb.isKinematic = !enable;
        weaponCollider.enabled = enable;
        weaponTriggerCollider.enabled = enable;
    }


}
