using Fusion;
using System;
using UnityEngine;

[Serializable]
public struct LobbyJoinedPlayers
{
    public PlayerRef PlayerRef;
    public NetworkObject netwokObject;
}

public enum Teams
{
    NONE,
    Red,
    Blue
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
            instance = this;
        else
            Destroy(instance);
    }

    public bool CheckIfPlayersReady()
    {
        int count = 0;
        foreach (LobbyJoinedPlayers joinedPlayer in GameLauncher.joinedPlayers)
        {
            var temp = joinedPlayer.netwokObject.GetComponent<TemporaryPlayer>();
            if (!temp.isReady) return false;
            else count++;
        }
        //if (count >= 2)
        //    return true;
        return true; // FIXMEL false
    }
}
