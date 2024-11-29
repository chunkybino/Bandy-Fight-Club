using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStatus : MonoBehaviour
{
    public GameManager gameManager;
    public PlayerFunc playerFunc;
    public PlayerAttack playerAttack;
    public PlayerInputs inputs;

    public int playerNumber;

    public TeamHolder team;

    public int health;
    public int maxHealth = 100;

    public bool isDead = false;

    public float stunBlockDamageMult = 0.5f;

    public UnityEvent onAwake;

    [SerializeField] ParticleSystem hitParticles;

    public AudioManager audio;
    [SerializeField] string hurtSoundName;

    void Awake()
    {
        gameManager = GameManager.singleton;

        onAwake?.Invoke();
    }

    void Start()
    {
        health = maxHealth;
    }

    void ChangeHealth(int amount)
    {
        health = Mathf.Clamp(health+amount,0,maxHealth);

        if (health == 0) Die();
    }

    public void GetHit(AttackInfo info)
    {
        int hurt = -info.damage;
        if (playerFunc.stunBlocking) hurt = Mathf.FloorToInt(hurt * stunBlockDamageMult);
        ChangeHealth(hurt);

        ParticleSystem particle = Instantiate(hitParticles, transform.position, Quaternion.identity);
        particle.transform.eulerAngles = new Vector3(0,0,Random.Range(0,360));

        audio.Play(hurtSoundName);

        playerAttack.OnHit();
    }

    public void Spawn(int playerNum)
    {
        gameManager = GameManager.singleton;

        gameObject.SetActive(true);

        playerNumber = playerNum;
        inputs.inputMap = gameManager.GetInputMap(playerNumber);
        team.team = playerNumber;
    }

    public void Die()
    {
        isDead = true;
        inputs.inputsActive = false;

        gameManager.PlayerDie(playerNumber);
    }
}
