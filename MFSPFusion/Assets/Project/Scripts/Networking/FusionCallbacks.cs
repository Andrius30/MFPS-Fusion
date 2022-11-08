using Fusion;
using Fusion.Sockets;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        Debug.Log($"Scene loaded successfully");
        if (runner.IsServer)
        {
            if (SceneManager.GetActiveScene().buildIndex == 3)
            {
                var obj = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, localPlayer);
                GameManager.instance.AddGameplayPlayer(localPlayer, obj);
            }
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (localPlayer == default)
            localPlayer = player;

        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            if (lobbySpawner == null)
            {
                lobbySpawner = FindObjectOfType<LobbySpawner>();
            }
        }
        if (runner.IsServer)
        {
            NetworkObject go = runner.Spawn(lobbyPlayerPrefab, lobbySpawner.GetSpawnPosition(), Quaternion.identity, player, (runner, go) =>
            {
                var temp = go.GetComponent<TemporaryPlayer>();
                GameManager.instance.AddLobbyPlayer(player, go, temp);

            });



        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        GameManager.instance.RemoveGameplayPlayer(player);
        GameManager.instance.RemoveLobbyPlayer(player);
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {

        NetworkInputs networkInput = new NetworkInputs();

        networkInput.buttons.Set(MyButtons.Forward, keyboard.wKey.IsPressed());
        networkInput.buttons.Set(MyButtons.Backward, keyboard.sKey.IsPressed());
        networkInput.buttons.Set(MyButtons.Right, keyboard.dKey.IsPressed());
        networkInput.buttons.Set(MyButtons.Left, keyboard.aKey.IsPressed());
        networkInput.buttons.Set(MyButtons.Jump, keyboard.spaceKey.isPressed);
        networkInput.buttons.Set(MyButtons.SpaceReleased, keyboard.spaceKey.wasReleasedThisFrame);
        networkInput.buttons.Set(MyButtons.LeftCtrl, keyboard.leftCtrlKey.isPressed);
        networkInput.buttons.Set(MyButtons.LeftCtrlReleased, keyboard.leftCtrlKey.wasReleasedThisFrame);

        networkInput.mousex = Input.GetAxisRaw("Mouse X");
        networkInput.mousey = Input.GetAxisRaw("Mouse Y");

        networkInput.buttons.Set(MyButtons.Ready, Keyboard.current.rKey.wasPressedThisFrame);

        input.Set(networkInput);
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

    void OnDisable()
    {
        runner.RemoveCallbacks(this);

    }
}
