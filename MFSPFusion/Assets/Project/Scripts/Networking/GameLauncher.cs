using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLauncher : MonoBehaviour
{
    [SerializeField] Button connectBtn;

    FusionCallbacks fusionCallbacks;
    LevelManager levelManager;
    
    public static List<LobbyJoinedPlayers> joinedPlayers = new List<LobbyJoinedPlayers>();

    void Start()
    {
        connectBtn.onClick.RemoveAllListeners();
        connectBtn.onClick.AddListener(Connect);
        fusionCallbacks = GetComponent<FusionCallbacks>();
    }

    public static void AddPlayer(PlayerRef player, NetworkObject go)
    {
        LobbyJoinedPlayers pl = new LobbyJoinedPlayers();
        pl.PlayerRef = player;
        pl.netwokObject = go;
        GameLauncher.joinedPlayers.Add(pl);
    }

    void Connect()
    {
        SceneManager.LoadScene(2); // load to lobby scene
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            connectBtn = GameObject.Find("ConnectBtn").GetComponent<Button>();
        }
        if (scene.buildIndex == 2)
        {
            if (fusionCallbacks != null)
                fusionCallbacks.Launch();
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

    }
}
