using System.Collections.Generic;
using UnityEngine;

public class GameplaySpawner : MonoBehaviour
{
    public List<Transform> redSpawnPositions;
    public List<Transform> blueSpawnPositions;

    public Vector3 GetSpawnPosition(Teams team)
    {
        switch (team)
        {
            case Teams.Red:
                return redSpawnPositions[Random.Range(0, redSpawnPositions.Count)].position;
            case Teams.Blue:
                return blueSpawnPositions[Random.Range(0, blueSpawnPositions.Count)].position;
        }
        return Vector3.zero;
    }

}
