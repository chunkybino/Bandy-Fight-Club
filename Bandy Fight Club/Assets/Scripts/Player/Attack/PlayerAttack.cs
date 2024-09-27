using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public PlayerStatus playerStatus;
    public PlayerFunc playerFunc;
    public PlayerInputs inputs;

    public ICharacterFunc characterFunc;

    public PlayerAnimation playerAnim;

    public bool isAttacking;
    public string attackName;

    public string[] attackNames = new string[0];

    public PlayerAttackConfig currentAttack;

    public PlayerAttackConfig[] attacks;

    [SerializeField] int attackIndex = 0;
    [SerializeField] float attackIndexResetTime = 0.4f; //how long after isAttacking ends
    
    [SerializeField] float attackBufferTime = 0.15f;
    float attackBufferTimer;

    [SerializeField] GameObject canAttackAnimator;

    void Awake()
    {
        characterFunc = playerFunc.characterFunc;
    }

    void Update()
    {
        if (characterFunc == null) characterFunc = playerFunc.characterFunc;

        UFunc.TickTimer(ref attackBufferTimer);
        if (inputs.attackPressed) attackBufferTimer = attackBufferTime;

        if (attackBufferTimer > 0)
        {
            bool canAttack = !isAttacking;
            if (canAttackAnimator.activeSelf) canAttack = true;

            if (canAttack)
            {
                attackBufferTimer = 0;
                Attack();
            }
        }

        if (!isAttacking)
        {
            Invoke("ResetAttackIndex", attackIndexResetTime);
        }
        else
        {
            CancelInvoke("ResetAttackIndex");
        }
    }

    void Attack()
    {
        string attackName = characterFunc.FindAttackName();
        PlayerAttackConfig attack = FindAttackByName(attackName);

        if (attack)
        {
            DoAttack(attack);
        }

        /*string attack = attackNames[attackIndex];
        attackIndex++;
        if (attackIndex >= attackNames.Length) attackIndex = 0;*/
    }

    PlayerAttackConfig FindAttackByName(string name)
    {
        foreach (PlayerAttackConfig attack in attacks)
        {
            //print(attack.gameObject.name.ToString() + "    " + name);
            if (attack.gameObject.name == name)
            {
                return attack;
            }
        }
        return null;
    }
    void DoAttack(PlayerAttackConfig attack)
    {
        currentAttack = attack;

        playerAnim.PlayAnim(attack.animationName);
        isAttacking = true;

        playerFunc.OnAttack();
    }

    public void ExitAttackAnim()
    {
        isAttacking = false;
    }

    public void ResetAttackIndex()
    {
        attackIndex = 0;
    }
}
