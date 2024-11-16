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
    [HideInInspector] public float attackBufferTimer;
    float attackHeavyBufferTimer;

    [SerializeField] GameObject canAttackAnimator;

    public float attackingSpeedMult = 1;

    public float attackStunTimer;
    bool isAttackStun {get{return attackStunTimer > 0;}}

    [SerializeField] float disadvantageAttackStunMult = 0.5f; //get attakced stunned for longer with more disadvantage

    void Awake()
    {
        characterFunc = playerFunc.characterFunc;

        playerFunc.onGroundLand.AddListener(OnGroundLand);
    }

    void Update()
    {
        if (characterFunc == null) characterFunc = playerFunc.characterFunc;

        UFunc.TickTimer(ref attackBufferTimer);
        if (inputs.attackPressed) attackBufferTimer = attackBufferTime;
        UFunc.TickTimer(ref attackHeavyBufferTimer);
        if (inputs.heavyPressed) attackHeavyBufferTimer = attackBufferTime;

        bool canAttack = playerFunc.GetCanAttack();
        if (canAttackAnimator.activeSelf) canAttack = true;

        if (canAttack) UFunc.TickTimer(ref attackStunTimer);

        if (canAttack && !isAttackStun)
        {
            if (attackBufferTimer > 0)
            {
                attackBufferTimer = 0;
                Attack();
            }
            if (attackHeavyBufferTimer > 0)
            {
                attackHeavyBufferTimer = 0;
                AttackHeavy();
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
    }
    void AttackHeavy()
    {
        string attackName = characterFunc.FindAttackHeavyName();
        PlayerAttackConfig attack = FindAttackByName(attackName);

        if (attack)
        {
            DoAttack(attack);
        }
    }

    PlayerAttackConfig FindAttackByName(string name)
    {
        foreach (PlayerAttackConfig attack in attacks)
        {
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

        SetAttackingSpeedMult(attack.speedMult);

        attack.onAttackStart?.Invoke();

        playerFunc.OnAttack();

        CancelInvoke("EndAttack");
        Invoke("EndAttack", attack.attackTime);
    }
    public void EndAttack()
    {
        CancelInvoke("EndAttack");

        isAttacking = false;

        playerFunc.OnAttackEnd();
    }
    public void ExitAttackAnim()
    {
        EndAttack();
    }

    public void ResetAttackIndex()
    {
        attackIndex = 0;
    }

    public void CancelAttack()
    {
        isAttacking = false;
        CancelInvoke("EndAttack");
        playerAnim.PlayAnim("ActionBlank");

        playerFunc.OnAttackEnd();
    }

    public void DoAttackStun(float stun)
    {
        attackStunTimer = stun + (playerFunc.disadvantage*disadvantageAttackStunMult);
    }

    void OnGroundLand()
    {
        if (currentAttack && isAttacking)
        {
            if (currentAttack.cancelOnGroundLand)
            {
                CancelAttack();
            }
        }
    }

    public void SetAttackingSpeedMult(float mult)
    {
        attackingSpeedMult = mult;
    }
}
