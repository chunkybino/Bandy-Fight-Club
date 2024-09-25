using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public PlayerStatus playerStatus;
    public PlayerFunc playerFunc;
    public PlayerInputs inputs;

    public PlayerAnimation playerAnim;

    public bool isAttacking;
    public string attackName;

    public string[] attackNames = new string[0];

    [SerializeField] int attackIndex = 0;
    [SerializeField] float attackIndexResetTime = 0.4f; //how long after isAttacking ends
    
    [SerializeField] float attackBufferTime = 0.15f;
    float attackBufferTimer;

    [SerializeField] GameObject canAttackAnimator;

    void Update()
    {
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
        string attack = attackNames[attackIndex];
        attackIndex++;
        if (attackIndex >= attackNames.Length) attackIndex = 0;

        playerAnim.PlayAnim(attack);
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
