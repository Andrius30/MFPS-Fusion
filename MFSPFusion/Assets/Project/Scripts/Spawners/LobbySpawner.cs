using System.Collections.Generic;
using UnityEngine;

public class LobbySpawner : MonoBehaviour
{
    public List<SpawnPosition> spawnPositions;


    public Vector3 GetSpawnPosition()
    {
        foreach (var pos in spawnPositions)
        {
            if (pos.taken) continue;
            return pos.transform.position;
        }
        return Vector3.zero;
    }

}
