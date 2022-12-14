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
    public static Action onConnected;
    public static NetworkRunner runner;
    public GameObject lobbyPlayerPrefab;
    public GameObject playerPrefab;

    [ReadOnly, SerializeField] LevelManager levelManager;
    [SerializeField] ConnectionStatus status = ConnectionStatus.Disconnected;

    LobbySpawner lobbySpawner;
    [SerializeField] NetworkPoolManager networkPool;
    GameplaySpawner gameplaySpawner;
    PlayerRef localPlayer;
    Keyboard keyboard;
    Mouse mouse;
    NetworkInputs networkInput = new NetworkInputs();

    void Start()
    {
        keyboard = Keyboard.current;
        mouse = Mouse.current;
        levelManager = FindObjectOfType<LevelManager>();
    }
    void Update()
    {
        #region Keyboard
        if (keyboard.spaceKey.wasReleasedThisFrame) // jump end
            networkInput.buttons.Set(MyButtons.SpaceReleased, true);
        if (keyboard.gKey.wasPressedThisFrame) // player ready button
            networkInput.buttons.Set(MyButtons.Ready, true);
        if (keyboard.spaceKey.wasPressedThisFrame) // jump
            networkInput.buttons.Set(MyButtons.Jump, true);
        if (keyboard.fKey.wasPressedThisFrame)
            networkInput.buttons.Set(MyButtons.DropWeapon, true);
        if (keyboard.eKey.wasPressedThisFrame)
            networkInput.buttons.Set(MyButtons.PickKey, true);
        #endregion

        #region Mouse
        if (mouse.leftButton.wasPressedThisFrame) // player attack button
            networkInput.buttons.Set(MyButtons.Fire, true);
        if (mouse.leftButton.wasReleasedThisFrame)
            networkInput.buttons.Set(MyButtons.FireUp, true);
        networkInput.mousex = Input.GetAxisRaw("Mouse X");
        networkInput.mousey = Input.GetAxisRaw("Mouse Y");
        networkInput.scrollWheel = Input.GetAxisRaw("Mouse ScrollWheel"); 
        #endregion

        SwitchWeaponWithKeys();
    }

    void SwitchWeaponWithKeys()
    {
        if (keyboard.digit1Key.wasPressedThisFrame)
        {
            networkInput.buttons.Set(MyButtons.Keyboard1Key, true);
        }
        if (keyboard.digit2Key.wasPressedThisFrame)
        {
            networkInput.buttons.Set(MyButtons.Keyboard2Key, true);
        }
        if (keyboard.digit3Key.wasPressedThisFrame)
        {
            networkInput.buttons.Set(MyButtons.Keyboard3Key, true);
        }
        if (keyboard.digit4Key.wasPressedThisFrame)
        {
            networkInput.buttons.Set(MyButtons.Keyboard4Key, true);
        }
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
            ObjectPool = networkPool,
            SceneManager = levelManager
        });
        SetConnectionStatus(ConnectionStatus.Connected);
        onConnected?.Invoke();
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
        LobbyJoinedPlayers lb = new LobbyJoinedPlayers();
        lb.PlayerRef = player;
        foreach (var pl in GameLauncher.joinedPlayers)
        {
            if (pl.PlayerRef.PlayerId == player.PlayerId)
            {
                runner.Despawn(pl.netwokObject);
                GameLauncher.joinedPlayers.Remove(pl);
                return;
            }
        }
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        networkInput.buttons.Set(MyButtons.Forward, keyboard.wKey.IsPressed());
        networkInput.buttons.Set(MyButtons.Backward, keyboard.sKey.IsPressed());
        networkInput.buttons.Set(MyButtons.Right, keyboard.dKey.IsPressed());
        networkInput.buttons.Set(MyButtons.Left, keyboard.aKey.IsPressed());
        networkInput.buttons.Set(MyButtons.LeftShiftHolding, keyboard.leftShiftKey.isPressed);
        networkInput.buttons.Set(MyButtons.LeftCtrl, keyboard.leftCtrlKey.isPressed);

        networkInput.buttons.Set(MyButtons.FireHold, mouse.leftButton.isPressed);

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
            SpawnPosition spawnPosition = lobbySpawner.GetSpawnPosition();
            NetworkObject go = runner.Spawn(lobbyPlayerPrefab, spawnPosition.transform.position, Quaternion.identity, player, (runner, go) =>
            {
                spawnPosition.taken = true;
                var temp = go.GetComponent<TemporaryPlayer>();
                var playerController = go.GetComponent<PlayerController>();
                playerController.thisPlayer = player;
                if (GameManager.instance.redTeam.Count <= GameManager.instance.blueTeam.Count)
                {
                    playerController.playerData.SaveTeam(player, Teams.Red);
                    playerController.playerTeam = Teams.Red;
                    GameManager.instance.redTeam.Add(playerController);
                }
                else
                {
                    playerController.playerData.SaveTeam(player, Teams.Blue);
                    playerController.playerTeam = Teams.Blue;
                    GameManager.instance.blueTeam.Add(playerController);
                }

                GameLauncher.AddPlayer(player, go);
            });
        }
    }
    void CreateGameplayPlayer(NetworkRunner runner)
    {
        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            var spawner = FindObjectOfType<GameplaySpawner>();
            var sortedList = GameLauncher.joinedPlayers.OrderBy(x => x.PlayerRef.PlayerId);
            foreach (var player in sortedList)
            {
                var obj = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player.PlayerRef);
                var playerController = obj.GetComponent<PlayerController>();
                playerController.playerTeam = playerController.playerData.GetTeam(player.PlayerRef);
                playerController.thisPlayer = player.PlayerRef;
                obj.transform.position = spawner.GetSpawnPosition(playerController.playerTeam);
            }
            GameLauncher.joinedPlayers.Clear();
        }
    }

    void OnDisable()
    {
        runner.RemoveCallbacks(this);

    }
}
