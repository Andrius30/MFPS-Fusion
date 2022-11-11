using Fusion;
using Fusion.Sockets;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum ConnectionStatus
{
    Disconnected,
    Connecting,
    Failed,
    Connected,
    Loading,
    Loaded
}
public class FusionCallbacks : SimulationBehaviour, INetworkRunnerCallbacks
{
    public static NetworkRunner runner;
    public GameObject lobbyPlayerPrefab;
    public GameObject playerPrefab;

    [ReadOnly, SerializeField] LevelManager levelManager;
    [SerializeField] ConnectionStatus status = ConnectionStatus.Disconnected;

    LobbySpawner lobbySpawner;
    GameplaySpawner gameplaySpawner;
    PlayerRef localPlayer;
    Keyboard keyboard;
    NetworkInputs networkInput = new NetworkInputs();

    void Start()
    {
        keyboard = Keyboard.current;
        levelManager = FindObjectOfType<LevelManager>();
    }
    void Update()
    {

        if (keyboard.spaceKey.wasReleasedThisFrame)
            networkInput.buttons.Set(MyButtons.SpaceReleased, true);
        if (keyboard.rKey.wasPressedThisFrame)
            networkInput.buttons.Set(MyButtons.Ready, true);
        if (keyboard.spaceKey.wasPressedThisFrame)
            networkInput.buttons.Set(MyButtons.Jump, true);
    }

    public void SetConnectionStatus(ConnectionStatus status)
    {
        switch (status)
        {
            case ConnectionStatus.Disconnected:
                LoadingScreen.onShowLoadingScreen?.Invoke(true, "Disconnected");
                break;
            case ConnectionStatus.Connecting:
                LoadingScreen.onShowLoadingScreen?.Invoke(true, "Connecting...");
                break;
            case ConnectionStatus.Connected:
                LoadingScreen.onShowLoadingScreen?.Invoke(false, "");
                break;
            case ConnectionStatus.Loading:
                LoadingScreen.onShowLoadingScreen?.Invoke(true, "Loading...");
                break;
            case ConnectionStatus.Loaded:
                LoadingScreen.onShowLoadingScreen?.Invoke(false, "");
                break;
        }
    }
    public async void Launch()
    {
        SetConnectionStatus(ConnectionStatus.Connecting);
        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;

        runner.AddCallbacks(this);

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "",
            PlayerCount = 10, 
            SceneManager = levelManager
        });
        SetConnectionStatus(ConnectionStatus.Connected);
    }
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        SetConnectionStatus(ConnectionStatus.Loaded);
        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            CreateGameplayPlayer(runner);
        }
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        FindAndAssignLobbySpawner();
        CreateLobbyPlayer(runner, player);
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {

    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        networkInput.buttons.Set(MyButtons.Forward, keyboard.wKey.IsPressed());
        networkInput.buttons.Set(MyButtons.Backward, keyboard.sKey.IsPressed());
        networkInput.buttons.Set(MyButtons.Right, keyboard.dKey.IsPressed());
        networkInput.buttons.Set(MyButtons.Left, keyboard.aKey.IsPressed());
        networkInput.buttons.Set(MyButtons.LeftShiftHolding, keyboard.leftShiftKey.isPressed);
        networkInput.buttons.Set(MyButtons.LeftCtrl, keyboard.leftCtrlKey.isPressed);


        networkInput.mousex = Input.GetAxisRaw("Mouse X");
        networkInput.mousey = Input.GetAxisRaw("Mouse Y") * -1;

        input.Set(networkInput);
        networkInput = default;
    }
    public void OnSceneLoadStart(NetworkRunner runner) => SetConnectionStatus(ConnectionStatus.Loading);

    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    void FindAndAssignLobbySpawner()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            if (lobbySpawner == null)
            {
                lobbySpawner = FindObjectOfType<LobbySpawner>();
            }
        }
    }
    void CreateLobbyPlayer(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            NetworkObject go = runner.Spawn(lobbyPlayerPrefab, lobbySpawner.GetSpawnPosition(), Quaternion.identity, player, (runner, go) =>
            {
                var temp = go.GetComponent<TemporaryPlayer>();
                GameLauncher.AddPlayer(player, go);
            });
        }
    }
    void CreateGameplayPlayer(NetworkRunner runner)
    {
        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            var sortedList = GameLauncher.joinedPlayers.OrderBy(x => x.PlayerRef.PlayerId);
            foreach (var player in sortedList)
            {
                Debug.Log($"Spaning {player.PlayerRef.PlayerId}");
                var spawnPos = new Vector3(0, 1.11f, 0);
                var obj = runner.Spawn(playerPrefab, spawnPos, Quaternion.identity, player.PlayerRef);
            }
        }
    }

    void OnDisable()
    {
        runner.RemoveCallbacks(this);

    }
}
