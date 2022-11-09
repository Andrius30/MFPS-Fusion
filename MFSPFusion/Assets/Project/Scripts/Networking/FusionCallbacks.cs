using Fusion;
using Fusion.Sockets;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class FusionCallbacks : SimulationBehaviour, INetworkRunnerCallbacks
{
    public static NetworkRunner runner;
    public GameObject lobbyPlayerPrefab;
    public GameObject playerPrefab;

    [ReadOnly, SerializeField] LevelManager levelManager;

    LobbySpawner lobbySpawner;
    GameplaySpawner gameplaySpawner;
    PlayerRef localPlayer;
    Keyboard keyboard;

    void Start()
    {
        keyboard = Keyboard.current;
        levelManager = FindObjectOfType<LevelManager>();
    }

    public async void Launch()
    {
        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;

        runner.AddCallbacks(this);

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "",
            PlayerCount = 2, // FIXME: Should be 10 at release
            SceneManager = levelManager
        });
    }
    public void OnSceneLoadDone(NetworkRunner runner)
    {
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
    NetworkInputs networkInput = new NetworkInputs();
    void Update()
    {
        networkInput.mousex = Input.GetAxis("Mouse X");
        networkInput.mousey = Input.GetAxis("Mouse Y");
        if (keyboard.spaceKey.wasReleasedThisFrame)
            networkInput.buttons.Set(MyButtons.SpaceReleased, true);
        if (keyboard.rKey.wasPressedThisFrame)
            networkInput.buttons.Set(MyButtons.Ready, true);
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        networkInput.buttons.Set(MyButtons.Forward, keyboard.wKey.IsPressed());
        networkInput.buttons.Set(MyButtons.Backward, keyboard.sKey.IsPressed());
        networkInput.buttons.Set(MyButtons.Right, keyboard.dKey.IsPressed());
        networkInput.buttons.Set(MyButtons.Left, keyboard.aKey.IsPressed());
        networkInput.buttons.Set(MyButtons.Jump, keyboard.spaceKey.isPressed);
        networkInput.buttons.Set(MyButtons.LeftShiftHolding, keyboard.leftShiftKey.isPressed);
        networkInput.buttons.Set(MyButtons.LeftCtrl, keyboard.leftCtrlKey.isPressed);

        input.Set(networkInput);
        networkInput = default;
    }

    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
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
