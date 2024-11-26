using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;

    public StatUI ui;
    public CameraControl cameraControl;

    public float timeScale = 1;
    float timeScaleFRFR;

    public List<PlayerStatus> players = new List<PlayerStatus>();

    public InputMapSO[] inputMaps;
    public List<InputMapSO> playerOrderInputMaps;

    public bool spawnPlayersOnStart = true;

    public List<PlayerInfo> playerInfos = new List<PlayerInfo>();

    public List<PlayerInfo> allPlayerInfos = new List<PlayerInfo>();
    public bool do3Player;

    public PlayerStatus[] characterList;

    public TeamColorsSO teamOutlineColors;

    public Transform[] spawnPoints;

    [SerializeField] bool isHitStop;
    [SerializeField] float hitStopSpeed = 0.05f;

    [SerializeField] Shaker cameraShaker;

    [SerializeField] bool doRespawnNow = false;

    [SerializeField] Animator screenTransitions;
    [SerializeField] float respawnTransitionDelay = 1; //daley between the close and open

    [SerializeField] bool readyForNextRound;

    public UnityEvent onRespawn;

    [SerializeField] float startDelay = 2;
    public bool gameStarted;

    public int playerWinNum = -1;

    [SerializeField] float playerDieSlowMult = 0.2f; //when player die, so down time by this factor
    [SerializeField] float playerDieSlowTime = 2f;
    bool isPlayerDieSlowmo = false;
    [SerializeField] float playerDieFocusTime = 1.5f;
    [SerializeField] float playerDieZoomMult = 2;
    [SerializeField] float playerDieRemoveCameraEffectorDelay = 1.5f;

    bool winnerFinalized;
    bool isDraw;

    public bool isMenu;
    [SerializeField] MenuManager menu;

    [SerializeField] float startMatchTransitionTime = 1f;

    void Awake()
    {
        CheckSingleton();

        if (ui == null) ui = FindObjectOfType<StatUI>();

        DoPlayerInfo();
    }
    public void DoPlayerInfo()
    {
        playerInfos.Clear();
        playerInfos.Add(allPlayerInfos[0]); //add players 1 and 2
        playerInfos.Add(allPlayerInfos[1]);
        if (do3Player) playerInfos.Add(allPlayerInfos[2]); //third player
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

    void Start()
    {
        DoScreenTransition(true);
        isMenu = true;

        //StartCoroutine("StartGameCoroutine");
    }

    public void StartMatchTransition()
    {
        DoScreenTransition(false);
        for (int i = 0; i < allPlayerInfos.Count && i < menu.selectedCharacters.Length; i++)
        {
            allPlayerInfos[i].characterPrefab = characterList[menu.selectedCharacters[i]];
        }
        playerOrderInputMaps = menu.playerOrderedInputMaps;

        Invoke("StartMatch", startMatchTransitionTime);
    }
    void StartMatch()
    {
        isMenu = false;
        menu.ToGame();
        SpawnPlayers();
        DoScreenTransition(true);
        StartCoroutine("StartGameCoroutine");
    }

    void Update()
    {
        CheckSingleton();

        //menu.gameObject.SetActive(isMenu);
        ui.gameObject.SetActive(!isMenu);

        if (!isMenu) //game is game
        {
            if (doRespawnNow)
            {
                doRespawnNow = false;
                DoRespawn();
            }
            if (readyForNextRound && Input.GetKeyDown("space"))
            {
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
            if (isPlayerDieSlowmo) timeScaleFRFR *= playerDieSlowMult;
            timeScaleFRFR *= timeScale;
            Time.timeScale = timeScaleFRFR;
        }
    }

    public int AddPlayer(PlayerStatus player)
    {
        players.Add(player);
        return players.Count - 1; //return player number, first player = player 0
    }
    public InputMapSO GetInputMap(int playerNumber)
    {
        if (playerOrderInputMaps.Count == 0) return null; 
        return playerOrderInputMaps[playerNumber % players.Count];
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

            playerInfo.dead = false;

            ui.SetPlayer(playerNum, player);
            cameraControl.AddPlayerEffector(player.transform);

            player.playerFunc.facing = Mathf.RoundToInt(Mathf.Sign(spawnPoints[i].localScale.x));
        }
    }

    IEnumerator StartGameCoroutine()
    {
        screenTransitions.Play("BottomClosed");

        yield return new WaitForSeconds(startDelay);

        screenTransitions.Play("BottomOpen");

        StartGame();
    }
    public void StartGame()
    {
        gameStarted = true;
    }

    public void DoRespawn()
    {
        StartCoroutine("RespawnCoroutine");
        readyForNextRound = false;
    }
    IEnumerator RespawnCoroutine()
    {
        DoScreenTransition(false);

        yield return new WaitForSeconds(respawnTransitionDelay);

        gameStarted = false;

        playerWinNum = -1;
        ui.DoWin(false);

        winnerFinalized = false;
        isDraw = false;
        
        onRespawn?.Invoke();
        RespawnPlayers();
        DoScreenTransition(true);

        StartCoroutine("StartGameCoroutine");

        yield break;
    }
    public void DestroyPlayers()
    {
        //destroy current players
        foreach (PlayerStatus player in players)
        {
            Destroy(player.gameObject);
        }
        cameraControl.ResetEffectors();
        players.Clear();
    }
    public void RespawnPlayers()
    {
        DestroyPlayers();
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

    void DoScreenTransition(bool open)
    {
        if (open)
        {
            screenTransitions.Play("Open");
        }
        else
        {
            screenTransitions.Play("Close");
        }
    }

    public void PlayerDie(int playerThatDied)
    {
        playerInfos[playerThatDied].dead = true;

        int playersAlive = 0;
        int alivePlayer = 0;
        for (int i = 0; i < playerInfos.Count; i++)
        {
            if (!playerInfos[i].dead) 
            {
                playersAlive++;
                alivePlayer = i;
            }
        }

        if (playersAlive > 1)
        {
            StartCoroutine("PlayerDieSlowMo", playerThatDied);
        }
        else if (playersAlive == 1)
        {
            PlayerWin(alivePlayer, playerThatDied);
        }
        else if (!winnerFinalized) //draw if the last player dies fast enough
        {
            isDraw = true;

            playerWinNum = -1;

            cameraControl.doZoomMult = false;
            cameraControl.doFocusTarget = false;
        }
    }
    public void PlayerWin(int winPlayer, int diePlayer)
    {
        StartCoroutine("PlayerWinCoroutine", diePlayer);
        playerWinNum = winPlayer;
    }

    IEnumerator PlayerWinCoroutine(int playerDiedIndex)
    {
        isPlayerDieSlowmo = true;

        cameraControl.doZoomMult = true;
        cameraControl.zoomMult = playerDieZoomMult;
        cameraControl.doFocusTarget = true;
        cameraControl.targetFocusIndex = playerDiedIndex;

        yield return new WaitForSecondsRealtime(playerDieSlowTime);

        isPlayerDieSlowmo = false;

        winnerFinalized = true;
        IncreaseScore();

        yield return new WaitForSeconds(playerDieFocusTime);

        cameraControl.doZoomMult = false;
        cameraControl.doFocusTarget = false;

        DoRoundOver();
    }
    IEnumerator PlayerDieSlowMo(int playerDiedIndex)
    {
        isPlayerDieSlowmo = true;

        yield return new WaitForSecondsRealtime(playerDieSlowTime);

        isPlayerDieSlowmo = false;
        
        yield return new WaitForSeconds(playerDieRemoveCameraEffectorDelay);

        cameraControl.RemovePlayerEffector(playerDiedIndex);
    }

    void IncreaseScore()
    {
        for (int i = 0; i < playerInfos.Count; i++)
        {
            if (!playerInfos[i].dead) playerInfos[i].score++;
        }
    }

    void DoRoundOver()
    {
        readyForNextRound = true;
        screenTransitions.Play("BottomClose");

        ui.DoWin(true, playerWinNum);
    }

    public void ExitMatch()
    {
        DoScreenTransition(false);

        Invoke("ToMainMenu", startMatchTransitionTime);
    }
    void ToMainMenu()
    {
        gameStarted = false;

        DestroyPlayers();

        playerWinNum = -1;
        ui.DoWin(false);

        winnerFinalized = false;
        isDraw = false;

        isMenu = true;
        menu.ToMainMenu();

        DoScreenTransition(true);
    }
}

[System.Serializable]
public class PlayerInfo
{
    public int playerNumber;
    public PlayerStatus characterPrefab;
    public PlayerStatus player;

    public int score;
    public bool dead;
}

[CreateAssetMenu(fileName = "TeamColorSO", menuName = "ScriptableObjects/TeamColor")]
public class TeamColorsSO : ScriptableObject
{
    public Color[] colors;
    public string[] names;
    public Material[] materials;

    public Color drawColor;
    public string drawName;
}
