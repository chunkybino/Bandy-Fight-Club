using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigMFunc : PlayerFunc, ICharacterFunc
{
    [SerializeField] float groundSlamSpeed;
    [SerializeField] bool isGroundSlam;
    [SerializeField] float groundedGroundSlamJumpHeight;
    [SerializeField] float airGroundSlamJumpHeight;
    [SerializeField] float groundSlamForwardBoost;

    void Awake()
    {
        if (characterFunc == null) characterFunc = this;
    }

    void FixedUpdate()
    {
        base.FixedUpdate();

        if (isGroundSlam)
        {
            rb.velocity = new Vector2(0, -groundSlamSpeed);

            if (grounded)
            {
                isGroundSlam = false;
                playerAttack.CancelAttack();
            }
        }
    }

    public string FindAttackName()
    {
        string attackName = "nope";
        
        if (inputs.downHeld)
        {
            attackName = "downAttack";
        }
        else
        {
            attackName = "slash1";
        }

        return attackName;
    }

    public void GroundSlamStart()
    {
        if (grounded)
        {
            JumpToHeight(groundedGroundSlamJumpHeight);
            rb.velocity = new Vector2(groundSlamForwardBoost * facing, rb.velocity.y);
        }
        else
        {
            JumpToHeight(airGroundSlamJumpHeight);
            rb.velocity = new Vector2(groundSlamForwardBoost * facing, rb.velocity.y);
        }
    }
    public void DoGroundSlam()
    {
        isGroundSlam = true;
    }

    public void GroundLand()
    {
        if (isGroundSlam)
        {
            isGroundSlam = false;
            playerAttack.CancelAttack();
        }
    }

    public override void OnAttackEnd()
    {
        base.OnAttackEnd();
        isGroundSlam = false;
    }

    public override bool GetDisableEntityCollision()
    {
        if (base.GetDisableEntityCollision()) return true;
        return isGroundSlam;
    }
    public override bool GetFallThroughPlatform()
    {
        return isGroundSlam;
    }
}
