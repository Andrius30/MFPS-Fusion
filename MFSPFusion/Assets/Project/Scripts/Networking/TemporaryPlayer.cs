using Fusion;
using System;
using UnityEngine;

public class TemporaryPlayer : NetworkBehaviour
{
    public Vector3 topRightBounds;
    public Vector3 topLeftBounds;
    public Vector3 BottomRightBounds;
    public Vector3 BottomLeftBounds;

    [Networked(OnChanged = nameof(OnChanged), OnChangedTargets = OnChangedTargets.All)]
    public NetworkBool isReady { get; set; }
    [Networked] public NetworkButtons inputs { get; set; }

    public static void OnChanged(Changed<TemporaryPlayer> changed)
    {
        changed.LoadNew();
        var newVal = changed.Behaviour.isReady;

        Debug.Log($"OnChanged called {newVal}");
        if (FusionCallbacks.runner.IsServer)
        {
            if (GameManager.instance.CheckIfPlayersReady())
            {
                LevelManager levelManager = FindObjectOfType<LevelManager>();
                levelManager.LoadLevel(3); // load to game scene
            }
        }

    }
    public override void FixedUpdateNetwork()
    {
        if (GetInput<NetworkInputs>(out var input) == false) return;
        if (input.buttons.IsSet(MyButtons.Ready))
        {
            isReady = !isReady;
            Debug.Log($"is ready {isReady}");
        }
    }

    void Update()
    {
        if (transform.position.x >= topRightBounds.x)
        {
            transform.position = new Vector3(topRightBounds.x, transform.position.y, transform.position.z);
        }
        else if (transform.position.x <= topLeftBounds.x)
        {
            transform.position = new Vector3(topLeftBounds.x, transform.position.y, transform.position.z);
        }
        else if (transform.position.z >= topRightBounds.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, topRightBounds.z);
        }
        else if (transform.position.z >= topLeftBounds.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, topLeftBounds.z);
        }
        else if (transform.position.x >= BottomRightBounds.x)
        {
            transform.position = new Vector3(BottomRightBounds.x, transform.position.y, transform.position.z);
        }
        else if (transform.position.x <= BottomLeftBounds.x)
        {
            transform.position = new Vector3(BottomLeftBounds.x, transform.position.y, transform.position.z);
        }
        else if (transform.position.z <= BottomRightBounds.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, BottomRightBounds.z);
        }
        else if (transform.position.z <= BottomLeftBounds.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, BottomLeftBounds.z);
        }

    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger");
    }
}
