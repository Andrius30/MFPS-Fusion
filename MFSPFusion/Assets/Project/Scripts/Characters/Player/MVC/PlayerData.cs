using Fusion;
using UnityEngine;

public class PlayerData
{


    public void SaveTeam(PlayerRef player, Teams team)
    {
        PlayerPrefs.SetInt($"{player.PlayerId}", (int)team);
    }
    public Teams GetTeam(PlayerRef player)
    {
        return (Teams)PlayerPrefs.GetInt($"{player.PlayerId}");
    }
}
