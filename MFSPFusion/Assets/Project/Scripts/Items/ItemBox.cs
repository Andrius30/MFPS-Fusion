using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    [SerializeField] List<Transform> spawnPositions;
    [SerializeField] List<NetworkObject> items;

    void Spawn()
    {
        int i = 0;
        foreach (var tr in spawnPositions)
        {
            if (FusionCallbacks.runner.IsServer)
            {
                var obj = FusionCallbacks.runner.Spawn(items[i], tr.position, tr.rotation);
                //obj.transform.SetParent(transform);
                var rb = obj.GetComponent<Rigidbody>();
                rb.isKinematic = true;
                i++;
            }
        }
    }

    void OnEnable()
    {
        FusionCallbacks.onConnected += Spawn;
    }
    void OnDisable()
    {
        FusionCallbacks.onConnected -= Spawn;
    }
}
