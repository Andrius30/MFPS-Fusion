using Fusion;
using UnityEngine;

public class DestroySelf : NetworkBehaviour
{
    [SerializeField] float destroyTime;
    [Networked] public TickTimer destroyTimer { get; set; }

    void Start()
    {
        destroyTimer = TickTimer.CreateFromSeconds(Runner, destroyTime);
    }

    public override void FixedUpdateNetwork()
    {
        if (destroyTimer.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
    }
}
