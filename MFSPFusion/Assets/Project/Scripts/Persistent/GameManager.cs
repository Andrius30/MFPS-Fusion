using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct JoinedPlayers
{
    public PlayerRef PlayerRef;
    public NetworkObject netwokObject;
    public TemporaryPlayer temporaryPlayer;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<JoinedPlayers> createdPlayers = new List<JoinedPlayers>();

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);
    }

    public void AddPlayer(PlayerRef player, NetworkObject obj, TemporaryPlayer temp)
    {
        JoinedPlayers joinedPlayer = new JoinedPlayers();
        joinedPlayer.PlayerRef = player;
        joinedPlayer.netwokObject = obj;
        joinedPlayer.temporaryPlayer = temp;
        createdPlayers.Add(joinedPlayer);
    }

    public bool CheckIfPlayersReady()
    {
        foreach (JoinedPlayers joinedPlayer in createdPlayers)
        {
            if (!joinedPlayer.temporaryPlayer.isReady) return false;
        }
        return true;
    }
}
