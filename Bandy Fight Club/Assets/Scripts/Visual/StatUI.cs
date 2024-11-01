using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatUI : MonoBehaviour
{
    GameManager gameManager;

    public PlayerStatus[] players;

    public Slider[] healthBars;
    public Slider[] disadvantageBars;

    public TextMeshProUGUI[] scoreText;

    public TextMeshProUGUI winText;
    [SerializeField] bool isWin;
    [SerializeField] int winPlayer = 0;
    float winFlashTimer;
    [SerializeField] float winFlashFreq = 0.5f;

    [SerializeField] string[] playerWinColors = {"RED", "BLU"};

    void Awake()
    {
        gameManager = GameManager.singleton;
    }

    void Update()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == null) continue;

            if (i < healthBars.Length)
            {
                healthBars[i].value = Mathf.Round(players[i].health) / Mathf.Round(players[i].maxHealth); //round so it becomes a float, i think
            }
            if (i < disadvantageBars.Length)
            {
                disadvantageBars[i].value = players[i].playerFunc.disadvantage / players[i].playerFunc.maxDisavantage; //round so it becomes a float, i think
            }

            if (i < scoreText.Length && i < gameManager.playerInfos.Count)
            {
                scoreText[i].text = gameManager.playerInfos[i].score.ToString();
            }
        }

        if (isWin)
        {
            if (winFlashTimer > winFlashFreq) winFlashTimer -= 2*winFlashFreq;

            if (winPlayer < playerWinColors.Length) winText.text = playerWinColors[winPlayer].ToString() + " WINS";
            winText.gameObject.SetActive(winFlashTimer >= 0);
            winText.color = gameManager.teamOutlineColors.colors[winPlayer];

            winFlashTimer += Time.deltaTime;
        }
        else
        {
            winText.gameObject.SetActive(false);
        }
    }

    public void SetPlayer(int index, PlayerStatus player)
    {
        if (index < players.Length) 
        {
            players[index] = player;
        }
    }
    public void DoWin(bool activate, int playerNum = 0)
    {
        if (!activate) 
        {
            isWin = false;
            return;
        }

        isWin = true;
        winPlayer = playerNum;
        winFlashTimer = 0;
    }
}
