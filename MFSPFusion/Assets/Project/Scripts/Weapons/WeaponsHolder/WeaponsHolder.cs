using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsHolder : NetworkBehaviour
{
    public static Action onSceneLoadDone;

    public List<Weapon> weaponsList = new List<Weapon>();

    [Networked] public int weaponHolderId { get; set; }
    public Launcher launcher;

    [Networked(OnChanged = nameof(OnChangedSwitchWeapon))] public int currentWeaponIndex { get; set; }
    public int maxGranades = 1;
    public int currentGranades = 0;

    static void OnChangedSwitchWeapon(Changed<WeaponsHolder> changed)
    {
        var newIndex = changed.Behaviour.CurrentWeaponIndex;
        changed.Behaviour.SwitchWeapon(newIndex);
    }

    public int CurrentWeaponIndex => currentWeaponIndex;

    public override void FixedUpdateNetwork()
    {
        if (GetInput<NetworkInputs>(out var input) == false) return;
        SwitchWithKeys(input);
        SwitchWithMouseWheel(input);

    }

    void SwitchWithMouseWheel(NetworkInputs input)
    {
        float wheel = input.scrollWheel;
        if (wheel > 0)
        {
            currentWeaponIndex++;
            if (currentWeaponIndex > weaponsList.Count - 1)
            {
                currentWeaponIndex = 0;
            }
        }
        else if (wheel < 0)
        {
            currentWeaponIndex--;
            if (currentWeaponIndex < 0)
            {
                currentWeaponIndex = weaponsList.Count - 1;
            }
        }
    }
    void SwitchWithKeys(NetworkInputs input)
    {
        if (input.buttons.IsSet(MyButtons.Keyboard1Key))
        {
            currentWeaponIndex = 0;
        }
        if (input.buttons.IsSet(MyButtons.Keyboard2Key))
        {
            currentWeaponIndex = 1;
        }
        if (input.buttons.IsSet(MyButtons.Keyboard3Key))
        {
            currentWeaponIndex = 2;
        }
    }

    public Weapon GetWeaponById(int id)
    {
        foreach (var weapon in weaponsList)
        {
            if (weapon.weaponID == id)
                return weapon;
        }
        return null;
    }
    public override void Spawned()
    {
        weaponHolderId = Object.InputAuthority.PlayerId;

        CreateWepons();
        StartCoroutine(DelayedSwitch());
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, InvokeLocal = false)]
    public void RPC_SwitchWeapon(int index)
    {
        SwitchWeapon(index);
    }
    public void PickWeapon(Weapon weapon)
    {
        if (Runner.IsServer)
        {
            if (weapon is Granade)
            {
                if (currentGranades + 1 > maxGranades) return;
                currentGranades++;
            }
            Weapon wep = null;

            foreach (var wp in GameManager.instance.allWeapons)
            {
                if (wp.weaponID == weapon.weaponID)
                {
                    wep = Runner.Spawn(wp, Vector3.zero, Quaternion.identity, Object.InputAuthority);
                    break;
                }
            }
            Runner.Despawn(weapon.Object);
            if (wep == null) return;
            wep.isEquiped = true;
            wep.gameObject.layer = 0;
            wep.transform.SetParent(transform);
            wep.transform.localPosition = wep.data.weaponPositionAtHand;
            wep.transform.localRotation = wep.data.weaponRotationAthand;
            wep.ToggleComponents(wep.Object, false);
            weaponsList.Add(wep);
            SwitchWeapon(weaponsList.Count - 1);
            RPC_SwitchWeapon(weaponsList.Count - 1);
        }
    }

    IEnumerator DelayedSwitch()
    {
        yield return new WaitForSeconds(.3f);
        SwitchWeapon(currentWeaponIndex);
        RPC_SwitchWeapon(currentWeaponIndex);
    }
    void CreateWepons()
    {
        if (!Runner.IsServer) return;
        foreach (var weapon in GameManager.instance.allWeapons)
        {
            if(!weapon.isDefault) continue;
            var gm = Runner.Spawn(weapon, inputAuthority: Object.InputAuthority);
            gm.transform.SetParent(transform);
            gm.transform.localPosition = weapon.data.weaponPositionAtHand;
            gm.transform.localRotation = weapon.data.weaponRotationAthand;
            Weapon wep = gm.GetComponent<Weapon>();
            wep.isEquiped = true;
            weaponsList.Add(wep);
        }
    }
    public void SwitchWeapon(int index)
    {
        if (weaponsList.Count <= 0) return;
        if (index < weaponsList.Count)
        {
            weaponsList.ForEach(x => x.gameObject.SetActive(false));
            weaponsList[index].gameObject.SetActive(true);
            BaseGun.onWeaponSwitched?.Invoke();
        }
    }

}
