using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    [SerializeField] List<Transform> spawnPositions;
    [SerializeField] List<NetworkObject> items;

    void Spawn()
    {
        Debug.Log($"Start called {FusionCallbacks.runner.IsServer}");
        int i = 0;
        foreach (var tr in spawnPositions)
        {
            if (FusionCallbacks.runner.IsServer)
            {
                Debug.Log($"Spawn items {items[i].name}");
                var obj = FusionCallbacks.runner.Spawn(items[i], tr.position, tr.rotation);
                var rb = obj.GetComponent<Rigidbody>();
                rb.isKinematic = true;
                i++;
            }
        }
    }

    void OnEnable()
    {
        FusionCallbacks.onPlayerJoined += Spawn;
    }
    void OnDisable()
    {
        FusionCallbacks.onPlayerJoined -= Spawn;
    }
}
