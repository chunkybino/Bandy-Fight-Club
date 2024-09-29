using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;

    public float timeScale = 1;

    public List<PlayerStatus> players = new List<PlayerStatus>();

    public InputMapSO[] inputMaps;

    public bool spawnPlayersOnStart = true;

    public List<PlayerInfo> playerInfos = new List<PlayerInfo>();

    public Transform[] spawnPoints;

    void Awake()
    {
        CheckSingleton();

        if (spawnPlayersOnStart) SpawnPlayers();
    }
    void CheckSingleton()
    {
        if (!singleton) 
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        CheckSingleton();

        Time.timeScale = timeScale;
    }

    public int AddPlayer(PlayerStatus player)
    {
        players.Add(player);
        return players.Count - 1; //return player number, first player = player 0
    }
    public InputMapSO GetInputMap(int playerNumber)
    {
        if (inputMaps.Length == 0) return null; 
        return inputMaps[playerNumber % players.Count];
    }

    public void SpawnPlayers()
    {
        for (int i = 0; i < playerInfos.Count; i++)
        {
            PlayerInfo playerInfo = playerInfos[i];
            Vector3 spawnPos = new Vector3(0,0,0);
            if (spawnPoints.Length > i) spawnPos = spawnPoints[i].position;

            PlayerStatus player = Instantiate(playerInfo.characterPrefab, spawnPos, Quaternion.identity);
            int playerNum = AddPlayer(player);
            player.Spawn(playerNum);

            playerInfo.player = player;
            playerInfo.playerNumber = playerNum;
        }
    }
}

[System.Serializable]
public class PlayerInfo
{
    public int playerNumber;
    public PlayerStatus characterPrefab;
    public PlayerStatus player;
}
