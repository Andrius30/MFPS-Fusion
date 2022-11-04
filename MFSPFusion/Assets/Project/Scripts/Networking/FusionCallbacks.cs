using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FusionCallbacks : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkRunner runner;
    public static PlayerRef localPlayer;

    [SerializeField] LevelManager levelManager;

    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    public async void Launch()
    {
        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;

        await runner.StartGame(new StartGameArgs()
        {

            GameMode = GameMode.AutoHostOrClient,
            SessionName = "",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = levelManager
        });
        levelManager.LoadLevel(2); // load to lobby scene
    }
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (levelManager == null)
                levelManager = FindObjectOfType<LevelManager>();
        }
        Debug.Log($"Scene loaded successfully");
        if (runner.IsServer)
        {

        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        localPlayer = player;
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {

    }

    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
}
