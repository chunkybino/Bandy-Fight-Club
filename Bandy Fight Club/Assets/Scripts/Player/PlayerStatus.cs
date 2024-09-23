using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public PlayerFunc playerFunc;

    public TeamHolder team;

    public int health;
    public int maxHealth = 100;

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
}
