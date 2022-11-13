using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public enum ShootType
{
    Raycast,
    Rigidbody
}

[CreateAssetMenu(fileName = "Weapon data", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public Vector3 weaponPositionAtHand;
    public Quaternion weaponRotationAthand;

    public string weaponName;
    public float cooldown;
    public float weaponRange;
    public LayerMask weaponMask;
    public ShootType shootType = ShootType.Raycast;

    [ShowIf("shootType", ShootType.Rigidbody)]
    public Rigidbody projectilePrefab;
    [ShowIf("shootType", ShootType.Rigidbody)]
    public float projectileSpeed;

}
