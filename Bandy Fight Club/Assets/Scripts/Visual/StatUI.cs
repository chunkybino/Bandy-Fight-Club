using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatUI : MonoBehaviour
{
    public PlayerStatus[] players;

    public Slider[] healthBars;
    public Slider[] disadvantageBars;

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
        }
    }

    public void SetPlayer(int index, PlayerStatus player)
    {
        if (index < players.Length) 
        {
            players[index] = player;
        }
    }
}
