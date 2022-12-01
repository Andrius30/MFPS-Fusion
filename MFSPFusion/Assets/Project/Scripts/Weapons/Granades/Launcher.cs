using Andrius.Core.Utils;
using Fusion;
using UnityEngine;

public class Launcher : NetworkBehaviour
{
    [SerializeField] Transform handTransform;
    [SerializeField] Camera cam;
    [SerializeField] GameObject granadePrefab;
    [SerializeField] GameObject cursor;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float maxDistance;
    [SerializeField] float time = 2f;
    RaycastHit hit = default;
    Vector3 vo = Vector3.zero;

    public override void FixedUpdateNetwork()
    {
        if (GetInput<NetworkInputs>(out var input) == false) return;
        if (input.buttons.IsSet(MyButtons.FireHold))
        {
            cursor.SetActive(true);
            hit = StaticFunctions.GetHitInfo(cam, maxDistance, groundMask);
            cursor.transform.position = hit.point + Vector3.up * 0.1f;
            vo = StaticFunctions.CalculateVelocity(hit.point, handTransform.position, time);
            handTransform.rotation = Quaternion.LookRotation(vo);
        }
        if (input.buttons.IsSet(MyButtons.FireUp))
        {
            cursor.SetActive(false);
            Attack(vo);
        }
    }

    public void Attack(Vector3 velocity)
    {
        var obj = Runner.Spawn(granadePrefab, handTransform.position, Quaternion.identity);
        Granade granade = obj.GetComponent<Granade>();
        granade.OnLauched();
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        WeaponsHolder holder = GetComponent<WeaponsHolder>();
        granade.ToggleComponents(obj, true);
        rb.velocity = velocity;
        Runner.Despawn(holder.GetWeaponById(granade.weaponID).Object);
        for (int i = 0; i < holder.weaponsList.Count; i++)
        {
            Weapon weapon = holder.weaponsList[i];
            if (weapon.weaponID == granade.weaponID)
            {
                holder.weaponsList.RemoveAt(i);
                break;
            }
        }
        holder.currentGranades--;
        if (holder.currentGranades <= 0)
        {
            holder.launcher.enabled = false;
            holder.SwitchWeapon(0);
            holder.RPC_SwitchWeapon(0);
        }
    }
}
