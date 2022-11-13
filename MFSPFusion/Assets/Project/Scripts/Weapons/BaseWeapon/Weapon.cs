using Fusion;
using UnityEngine;

public abstract class Weapon : NetworkBehaviour
{
    public WeaponData data;

    public abstract void Attack();
    public override void FixedUpdateNetwork()
    {
        Debug.Log($"Fixed update called");
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
