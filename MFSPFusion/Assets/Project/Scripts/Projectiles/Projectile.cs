using Fusion;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    public float projectileSpeed;
    public Vector3 direction;
    public float liveTime = 3f;

    [Networked] protected TickTimer liveTimer { get; set; }

    public override void Spawned()
    {
        liveTimer = TickTimer.CreateFromSeconds(Runner, liveTime);
    }

    public override void FixedUpdateNetwork()
    {
        transform.position += direction * (projectileSpeed * Runner.DeltaTime);
        if (liveTimer.ExpiredOrNotRunning(Runner))
        {
            Runner.Despawn(Object);
        }
    }


}
