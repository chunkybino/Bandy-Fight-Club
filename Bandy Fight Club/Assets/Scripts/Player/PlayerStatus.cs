using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStatus : MonoBehaviour
{
    public GameManager gameManager;
    public PlayerFunc playerFunc;
    public PlayerInputs inputs;

    public int playerNumber;

    public TeamHolder team;

    public int health;
    public int maxHealth = 100;

    public UnityEvent onAwake;

    void Awake()
    {
        gameManager = GameManager.singleton;

        //Spawn();

        onAwake?.Invoke();
    }

    void Start()
    {
        health = maxHealth;
    }

    void ChangeHealth(int amount)
    {
        health = Mathf.Clamp(health+amount,0,maxHealth);
    }

    public void GetHit(AttackInfo info)
    {
        ChangeHealth(-info.damage);
    }

    public void Spawn(int playerNum)
    {
        //playerNumber = gameManager.AddPlayer(this);
        playerNumber = playerNum;
        inputs.inputMap = gameManager.GetInputMap(playerNumber);
        team.team = playerNumber;
    }
}
