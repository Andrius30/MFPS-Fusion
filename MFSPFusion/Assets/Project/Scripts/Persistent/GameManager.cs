using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct LobbyJoinedPlayers
{
    public PlayerRef PlayerRef;
    public NetworkObject netwokObject;
    public TemporaryPlayer temporaryPlayer;
}
[Serializable] 
public struct GameplayJoinedPlayer
{
    public PlayerRef player;
    public NetworkObject obj;

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

    public List<LobbyJoinedPlayers> lobbyCreatedPlayers = new List<LobbyJoinedPlayers>();
    public List<GameplayJoinedPlayer> gameplayCreatedPlayers = new List<GameplayJoinedPlayer>();

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);
    }

    public void AddLobbyPlayer(PlayerRef player, NetworkObject obj, TemporaryPlayer temp)
    {
        LobbyJoinedPlayers joinedPlayer = new LobbyJoinedPlayers();
        joinedPlayer.PlayerRef = player;
        joinedPlayer.netwokObject = obj;
        joinedPlayer.temporaryPlayer = temp;
        lobbyCreatedPlayers.Add(joinedPlayer);
    }
    public void RemoveLobbyPlayer(PlayerRef player)
    {
        LobbyJoinedPlayers joinedPlayer = new LobbyJoinedPlayers();
        joinedPlayer.PlayerRef = player;
        if(lobbyCreatedPlayers.Contains(joinedPlayer))
            lobbyCreatedPlayers.Remove(joinedPlayer);
    }
    public void AddGameplayPlayer(PlayerRef player, NetworkObject obj)
    {
        GameplayJoinedPlayer joinedPlayer = new GameplayJoinedPlayer();
        joinedPlayer.player = player;
        joinedPlayer.obj = obj;
        gameplayCreatedPlayers.Add(joinedPlayer);
    }
    public void RemoveGameplayPlayer(PlayerRef player)
    {
        GameplayJoinedPlayer joinedPlayer = new GameplayJoinedPlayer();
        joinedPlayer.player = player;
        if (gameplayCreatedPlayers.Contains(joinedPlayer))
            gameplayCreatedPlayers.Remove(joinedPlayer);
    }

    public bool CheckIfPlayersReady()
    {
        foreach (LobbyJoinedPlayers joinedPlayer in lobbyCreatedPlayers)
        {
            if (!joinedPlayer.temporaryPlayer.isReady) return false;
        }
        return true;
    }
}
