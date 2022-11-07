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

    void Start()
    {
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
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
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
                GameManager.instance.AddPlayer(player, go, temp);

            });



        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {

    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        NetworkInputs networkInput = new NetworkInputs();

        networkInput.buttons.Set(MyButtons.Forward, Input.GetKey(KeyCode.W));
        networkInput.buttons.Set(MyButtons.Backward, Input.GetKey(KeyCode.S));
        networkInput.buttons.Set(MyButtons.Right, Input.GetKey(KeyCode.D));
        networkInput.buttons.Set(MyButtons.Left, Input.GetKey(KeyCode.A));
        networkInput.buttons.Set(MyButtons.Jump, Input.GetKey(KeyCode.Space));
        networkInput.buttons.Set(MyButtons.SpaceReleased, Input.GetKeyUp(KeyCode.Space));

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
