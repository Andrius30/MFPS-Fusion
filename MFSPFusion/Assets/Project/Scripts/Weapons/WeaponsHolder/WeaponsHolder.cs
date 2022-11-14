using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsHolder : NetworkBehaviour
{
    public static Action onSceneLoadDone;

    [SerializeField] List<Weapon> weapons;

    public List<GameObject> weaponsList = new List<GameObject>();

    [Networked] public int weaponHolderId { get; set; }

    int currentWeaponIndex = 0;

    void Update()
    {
        if (Object == null)
        {
            return;
        }
        if (!Object.HasInputAuthority)
        {
            return;
        }
        if (weaponsList.Count <= 0)
        {
            return;
        }
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        if (wheel > 0)
        {
            currentWeaponIndex++;
            if (currentWeaponIndex > weaponsList.Count - 1)
            {
                currentWeaponIndex = 0;
            }
            SwitchWeapon(currentWeaponIndex);
            RPC_SwitchWeapon(currentWeaponIndex);
        }
        else if (wheel < 0)
        {
            currentWeaponIndex--;
            if (currentWeaponIndex < 0)
            {
                currentWeaponIndex = weaponsList.Count - 1;
            }
            SwitchWeapon(currentWeaponIndex);
            RPC_SwitchWeapon(currentWeaponIndex);
        }
    }

    public override void Spawned()
    {
        weaponHolderId = Object.InputAuthority.PlayerId;

        CreateWepons();
        StartCoroutine(DelayedSwitch());
    }

    IEnumerator DelayedSwitch()
    {
        yield return new WaitForSeconds(.5f);
        SwitchWeapon(currentWeaponIndex);
        RPC_SwitchWeapon(currentWeaponIndex);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All, InvokeLocal = false)]
    public void RPC_SwitchWeapon(int index)
    {
        SwitchWeapon(index);
    }

    void CreateWepons()
    {
        if (!Runner.IsServer) return;
        foreach (var weapon in weapons)
        {
            var gm = Runner.Spawn(weapon, inputAuthority: Object.InputAuthority);
            gm.transform.SetParent(transform);
            gm.transform.localPosition = weapon.data.weaponPositionAtHand;
            gm.transform.localRotation = weapon.data.weaponRotationAthand;
            weaponsList.Add(gm.gameObject);
        }
    }
    void SwitchWeapon(int index)
    {
        if (weaponsList.Count <= 0) return;
        weaponsList.ForEach(x => x.gameObject.SetActive(false));
        weaponsList[index].SetActive(true);
    }
}
