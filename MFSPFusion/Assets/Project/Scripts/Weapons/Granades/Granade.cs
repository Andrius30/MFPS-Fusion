using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : Weapon
{
    [SerializeField] float explodeTime = 3f;
    [SerializeField] float explotionRadius = 10f;
    [Networked] TickTimer explodeTimer { get; set; }
    public bool isLaunched = false;
    public bool isSpawned = false;

    Launcher launcher;

    public override void Spawned()
    {
        if (isEquiped && isSpawned)
        {
            if (launcher == null)
                launcher = transform.parent.GetComponent<Launcher>();
            launcher.enabled = true;
        }
        isSpawned = true;
    }
    public void OnLauched()
    {
        explodeTimer = TickTimer.CreateFromSeconds(Runner, explodeTime);
        isLaunched = true;
    }

    public override void FixedUpdateNetwork()
    {
        if (!isLaunched) return;
        if (explodeTimer.ExpiredOrNotRunning(Runner))
        {
            List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
            HitOptions options = HitOptions.IncludePhysX | HitOptions.IgnoreInputAuthority | HitOptions.SubtickAccuracy;
            Runner.LagCompensation.OverlapSphere(transform.position, explotionRadius, Object.InputAuthority, hits, -1, options);
            foreach (var collider in hits)
            {
                BaseCharacter character = collider.GameObject.GetComponent<BaseCharacter>();
                if (character != null)
                {
                    character.ApplyDamage(data.weaponDamage);
                }
            }
            Runner.Despawn(Object);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explotionRadius);
    }
    void OnEnable()
    {
        if (!isSpawned || isLaunched) return;
        if (isEquiped)
        {
            if (launcher == null)
                launcher = transform.parent.GetComponent<Launcher>();
            transform.SetParent(launcher.transform);
            transform.localPosition = data.weaponPositionAtHand;
            transform.localRotation = data.weaponRotationAthand;
            launcher.enabled = true;
        }
    }
    void OnDisable()
    {
        if (!isSpawned || isLaunched) return;
        if (launcher == null && transform.parent != null)
            launcher = transform.parent.GetComponent<Launcher>();
        if (launcher != null)
            launcher.enabled = false;
    }
}
