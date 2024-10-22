using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;

    public StatUI ui;

    public float timeScale = 1;
    float timeScaleFRFR;

    public List<PlayerStatus> players = new List<PlayerStatus>();

    public InputMapSO[] inputMaps;

    public bool spawnPlayersOnStart = true;

    public List<PlayerInfo> playerInfos = new List<PlayerInfo>();

    public Transform[] spawnPoints;

    [SerializeField] bool isHitStop;
    [SerializeField] float hitStopSpeed = 0.05f;

    [SerializeField] Shaker cameraShaker;

    [SerializeField] bool doRespawnNow = false;

    void Awake()
    {
        CheckSingleton();

        if (ui == null) ui = FindObjectOfType<StatUI>();

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

        if (doRespawnNow)
        {
            doRespawnNow = false;
            DoRespawn();
        }

        if (isHitStop)
        {
            timeScaleFRFR = hitStopSpeed;
        }
        else
        {
            timeScaleFRFR = 1;
        }
        timeScaleFRFR *= timeScale;
        Time.timeScale = timeScaleFRFR;
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

            ui.SetPlayer(playerNum, player);

            player.playerFunc.facing = Mathf.RoundToInt(Mathf.Sign(spawnPoints[i].localScale.x));
        }
    }

    public void DoRespawn()
    {
        //destroy current players
        foreach (PlayerStatus player in players)
        {
            Destroy(player.gameObject);
        }
        players.Clear();

        SpawnPlayers();
    }

    public static void DoHitStopStatic(float time)
    {
        singleton.DoHitStop(time);
    }
    public void DoHitStop(float time)
    {
        isHitStop = true;
        StartCoroutine("EndHitStop", time);
    }

    IEnumerator EndHitStop(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        isHitStop = false;
    }

    public static void DoCameraShakeStatic(float strength)
    {
        singleton.DoCameraShake(strength);
    }
    public void DoCameraShake(float strength)
    {
        cameraShaker.DoShake(strength);
    }
}

[System.Serializable]
public class PlayerInfo
{
    public int playerNumber;
    public PlayerStatus characterPrefab;
    public PlayerStatus player;
}
