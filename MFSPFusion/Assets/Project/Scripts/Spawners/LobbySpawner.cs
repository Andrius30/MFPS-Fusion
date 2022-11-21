using System.Collections.Generic;
using UnityEngine;

public class LobbySpawner : MonoBehaviour
{
    public List<SpawnPosition> spawnPositions;


    public SpawnPosition GetSpawnPosition()
    {
        foreach (var pos in spawnPositions)
        {
            if (pos.taken) continue;
            return pos;
        }
        return null;
    }
    public SpawnPosition GetRandomSpawnPosition()
    {
        int rand = Random.Range(0, spawnPositions.Count);
        return spawnPositions[rand];
    }
}
