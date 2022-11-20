using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public List<Weapon> allWeapons;
    public List<PlayerController> redTeam = new List<PlayerController>();
    public List<PlayerController> blueTeam = new List<PlayerController>();

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
        return true; // FIXME: false
    }
    public Weapon GetWeaponByID(int id)
    {
        foreach (var weapon in allWeapons)
        {
            if(weapon.weaponID== id) return weapon;
        }
        return null;
    }
    public void RespawnPlayer(BaseCharacter controller) => StartCoroutine(RespawnRoutine(controller));
    IEnumerator RespawnRoutine(BaseCharacter controller)
    {
        yield return new WaitForSeconds(5f);
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex == 2) // lobby
        {
            var lobbySpawner = FindObjectOfType<LobbySpawner>();
            controller.transform.position = lobbySpawner.GetSpawnPosition();
        }
        else if (sceneIndex == 3) // gameplay
        {
            var gameplaySpawner = FindObjectOfType<GameplaySpawner>();
            controller.transform.position = gameplaySpawner.GetSpawnPosition(controller.playerTeam);
        }
        controller.initHealth = controller.maxHealth;
        controller.statsScreen.SetHealthStats(controller.initHealth, controller.maxHealth);
        controller.gameObject.SetActive(true);
    }
}
