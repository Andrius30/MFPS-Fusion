using Fusion;
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
    public float weaponDamage;
    public float reloadTime;
    public Sprite weaponIcon;
    public LayerMask weaponMask;
    public ShootType shootType = ShootType.Raycast;

    #region Recoil
    [FoldoutGroup("Recoil")] public float recoilX;
    [FoldoutGroup("Recoil")] public float recoilY;
    [FoldoutGroup("Recoil")] public float recoilZ;
 
    #endregion

    [ShowIf("shootType", ShootType.Raycast)]
    public GameObject hitScanProjectilePrefab;
    public GameObject muzleFlashPrefab;

    #region Bullet impacts
    [FoldoutGroup("Bullet impacts")] public NetworkObject bulletImpactConretePrefab;
    [FoldoutGroup("Bullet impacts")] public NetworkObject bulletImpactSandPrefab;
    [FoldoutGroup("Bullet impacts")] public NetworkObject bulletImpactSoftBodyPrefab;
    [FoldoutGroup("Bullet impacts")] public NetworkObject bulletImpactWoodPrefab;
    [FoldoutGroup("Bullet impacts")] public NetworkObject bulletImpactMetalPrefab;
    [FoldoutGroup("Bullet impacts")] public NetworkObject bulletImpactDirtPrefab;
    [FoldoutGroup("Bullet impacts")] public NetworkObject bulletImpactBloodPrefab;
    #endregion

    [ShowIf("shootType", ShootType.Rigidbody)]
    public Rigidbody projectilePrefab;
    [ShowIf("shootType", ShootType.Rigidbody)]
    public float projectileSpeed;

    internal void PlayImpactEffect(NetworkRunner runner, LagCompensatedHit hit, Surface surface)
    {
        switch (surface.surfaceType)
        {
            case SurfaceType.Concrete:
                CreatehitEffect(runner, hit, bulletImpactConretePrefab);
                break;
            case SurfaceType.Sand:
                CreatehitEffect(runner, hit, bulletImpactSandPrefab);
                break;
            case SurfaceType.SoftBody:
                CreatehitEffect(runner, hit, bulletImpactSoftBodyPrefab);
                break;
            case SurfaceType.Wood:
                CreatehitEffect(runner, hit, bulletImpactWoodPrefab);
                break;
            case SurfaceType.Metal:
                CreatehitEffect(runner, hit, bulletImpactMetalPrefab);
                break;
            case SurfaceType.Dirt:
                CreatehitEffect(runner, hit, bulletImpactDirtPrefab);
                break;
            case SurfaceType.Blood:
                CreatehitEffect(runner, hit, bulletImpactBloodPrefab);
                break;
        }
    }
    void CreatehitEffect(NetworkRunner runner, LagCompensatedHit hit, NetworkObject prefab)
    {
        runner.Spawn(prefab, hit.Point, Quaternion.identity);
    }
}
