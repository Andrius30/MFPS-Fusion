using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public abstract class Weapon : NetworkBehaviour
{
    public int weaponID;
    public WeaponData data;
    public bool isDropable = false;
    [HideInInspector, Networked] public NetworkBool isEquiped { get; set; }
    [Networked] protected TickTimer cooldownTimer { get; set; }
    protected Animator animator;
    protected int shootTrigger = Animator.StringToHash("Shoot");
    public Rigidbody rb;
    public BoxCollider weaponCollider;
    public BoxCollider weaponTriggerCollider;


    protected virtual void Start()
    {
        if (!isEquiped) return;
        animator = GetComponentInChildren<Animator>();
        ToggleComponents(Object, false);
    }

    public abstract void Attack();
    public virtual void Update() { }

    public override void FixedUpdateNetwork()
    {
        if (!isEquiped) return;
        Attack();
        if (GetInput<NetworkInputs>(out var input) == false) return;
        if (input.buttons.IsSet(MyButtons.DropWeapon))
        {
            DropCurrentWeapon();
        }
    }

    public override void Spawned()
    {
        if (isEquiped)
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
    }

    public void DropCurrentWeapon()
    {
        WeaponsHolder holder = transform.parent.GetComponent<WeaponsHolder>();

        if (weaponID == holder.weaponsList[holder.CurrentWeaponIndex].weaponID)
        {
            DropWeaponLogic(holder, this);
        }
    }

    public void DropWeapon()
    {
        WeaponsHolder holder = transform.parent.GetComponent<WeaponsHolder>();
        foreach (var weapon in GameManager.instance.allWeapons)
        {
            if (weapon.weaponID == weaponID)
            {
                DropWeaponLogic(holder, weapon);
                return;
            }
        }
    }
    public void ToggleComponents(NetworkObject obj, bool enable)
    {
        rb = obj.GetComponent<Rigidbody>();
        var networkRb = obj.GetComponent<NetworkRigidbody>();
        rb.isKinematic = !enable;
        weaponCollider.enabled = enable;
        weaponTriggerCollider.enabled = enable;
        networkRb.enabled = enable;
    }

    void DropWeaponLogic(WeaponsHolder holder, Weapon weapon)
    {
        if (!isDropable) return;
        if (Runner != null && !Runner.IsServer) return;
        Weapon obj = Runner.Spawn(GameManager.instance.GetWeaponByID(weaponID), transform.position, transform.rotation);
        obj.isEquiped = false;
        obj.gameObject.layer = 7;
        obj.transform.parent = null;
        ToggleComponents(obj.Object, true);
        rb.AddForce(transform.forward * 1, ForceMode.Impulse);
        rb.AddForce(transform.up * 5, ForceMode.Impulse);
        Runner.Despawn(holder.weaponsList[holder.CurrentWeaponIndex].Object);
        holder.weaponsList.Remove(holder.weaponsList[holder.CurrentWeaponIndex].GetComponent<Weapon>());
        holder.currentWeaponIndex = holder.weaponsList[0].weaponID;
    }

}
