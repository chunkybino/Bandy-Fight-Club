using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatUI : MonoBehaviour
{
    GameManager gameManager;

    public List<PlayerStatus> players = new List<PlayerStatus>();

    public Slider[] healthBars;
    public Slider[] whiteBars;
    public Slider[] disadvantageBars;
    public float[] hitTimers;
    public float[] whitePreHitValues;

    [SerializeField] AnimationCurve hitWhiteCurve;
    [SerializeField] float hitWhiteSpeed = 1;

    public TextMeshProUGUI[] scoreText;

    [SerializeField] Transform ThirdPlayerUI;

    public TextMeshProUGUI winText;
    [SerializeField] bool isWin;
    [SerializeField] int winPlayer = 0;
    float winFlashTimer;
    [SerializeField] float winFlashFreq = 0.5f;

    [SerializeField] TextMeshProUGUI continueText;

    void Awake()
    {
        gameManager = GameManager.singleton;
    }

    void Update()
    {
        ThirdPlayerUI.gameObject.SetActive(players.Count >= 3); //if we have 3 players, show the ui

        continueText.gameObject.SetActive(isWin);

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == null) continue;

            if (players[i].hit)
            {
                players[i].hit = false;
                whitePreHitValues[i] = whiteBars[i].value;
                hitTimers[i] = 0;
            }
            else
            {
                hitTimers[i] += Time.deltaTime * hitWhiteSpeed;
            }

            if (i < healthBars.Length)
            {
                healthBars[i].value = Mathf.Round(players[i].health) / Mathf.Round(players[i].maxHealth); //round so it becomes a float, i think
            }
            if (i < whiteBars.Length)
            {
                float t = hitWhiteCurve.Evaluate(hitTimers[i]);
                whiteBars[i].value = Mathf.Lerp(whitePreHitValues[i], healthBars[i].value, t);
            }
            if (i < disadvantageBars.Length)
            {
                disadvantageBars[i].value = players[i].playerFunc.disadvantage / players[i].playerFunc.maxDisavantage; //round so it becomes a float, i think
            }

            if (i < scoreText.Length && i < gameManager.playerInfos.Count)
            {
                scoreText[i].text = gameManager.playerInfos[i].score.ToString();
            
                Color color = Color.white;
                if (i == gameManager.playerWinNum && isWin) color = gameManager.teamOutlineColors.colors[winPlayer];
                scoreText[i].color = color;
            }
        }

        if (isWin)
        {
            if (winFlashTimer > winFlashFreq) winFlashTimer -= 2*winFlashFreq;

            if (winPlayer < gameManager.teamOutlineColors.names.Length) 
            {
                string txt = gameManager.teamOutlineColors.drawName; //if index is negative, that means theeres a tie
                if (winPlayer >= 0) txt = gameManager.teamOutlineColors.names[winPlayer].ToString() + " WINS";

                winText.text = txt;
            }
            winText.gameObject.SetActive(winFlashTimer >= 0);

            Color clr = gameManager.teamOutlineColors.drawColor; //if index is negative, that means theeres a tie
            if (winPlayer >= 0) clr = gameManager.teamOutlineColors.colors[winPlayer];
            winText.color = clr;

            winFlashTimer += Time.deltaTime;
        }
        else
        {
            winText.gameObject.SetActive(false);
        }
    }

    public void SetPlayer(int index, PlayerStatus player)
    {
        if (index < players.Count) 
        {
            players[index] = player;
        }
        else
        {
            players.Add(player);
        }
    }
    public void DoWin(bool activate, int playerNum = 0)
    {
        if (!activate) 
        {
            isWin = false;
            continueText.gameObject.SetActive(false);
            return;
        }

        isWin = true;
        winPlayer = playerNum;
        winFlashTimer = 0;
    }
}
