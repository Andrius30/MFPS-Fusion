using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsHolder : NetworkBehaviour
{
    public static Action onSceneLoadDone;

    [SerializeField] List<Weapon> weapons;

    List<GameObject> weaponsList = new List<GameObject>();

    int currentWeaponIndex = 0;

    void Update()
    {
        if (Object == null) return;
        if (!Object.HasInputAuthority) return;
        if (weaponsList.Count <= 0) return;
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        if (wheel > 0)
        {
            currentWeaponIndex++;
            if (currentWeaponIndex >= weaponsList.Count)
                currentWeaponIndex = 0;
            RPC_SwitchWeapon(currentWeaponIndex);
            SwitchWeapon(currentWeaponIndex);
        }
        else if (wheel < 0)
        {
            currentWeaponIndex--;
            if (currentWeaponIndex < 0)
                currentWeaponIndex = weaponsList.Count - 1;
            RPC_SwitchWeapon(currentWeaponIndex);
            SwitchWeapon(currentWeaponIndex);
        }
    }

    public override void Spawned()
    {
        CreateWepons();
        SwitchWeapon(currentWeaponIndex);
        CreateAndWitchToDefault();
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.All, InvokeLocal = false)]
    public void RPC_CreateWeapons()
    {
        CreateWepons();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All, InvokeLocal = false)]
    public void RPC_SwitchWeapon(int index)
    {
        SwitchWeapon(index);
    }

    void CreateAndWitchToDefault()
    {
        RPC_CreateWeapons();
        RPC_SwitchWeapon(currentWeaponIndex);
    }
    void CreateWepons()
    {
        foreach (var weapon in weapons)
        {
            GameObject gm = Instantiate(weapon.gameObject, transform);
            gm.transform.localPosition = weapon.data.weaponPositionAtHand;
            gm.transform.localRotation = weapon.data.weaponRotationAthand;
            weaponsList.Add(gm);
        }
    }
    void SwitchWeapon(int index)
    {
        weaponsList.ForEach(x => x.gameObject.SetActive(false));
        weaponsList[index].SetActive(true);
    }
}
