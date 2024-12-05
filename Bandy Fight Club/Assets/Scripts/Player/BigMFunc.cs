using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigMFunc : PlayerFunc, ICharacterFunc
{
    [SerializeField] bool chargingThrust;
    [SerializeField] float timeChargingThrust;
    [SerializeField] string thrustAttackName = "Thrust";

    [SerializeField] bool isThrusting;
    [SerializeField] float baseThrustSpeed;
    [SerializeField] float baseThrustVerticalSpeed;
    [SerializeField] float thrustSpeedMult;
    [SerializeField] Transform thrustSpeedAnimMult;

    [SerializeField] int thrustChargeLevel;
    [SerializeField] float[] thrustLevelChargeThresholds = new float[3];
    [SerializeField] float[] thrustLevelSpeedMults = new float[3];
    [SerializeField] AttackInfo[] thrustLevelSAttackInfo = new AttackInfo[3];
    [SerializeField] AttackHitbox thrustHitbox;

    [SerializeField] float groundSlamSpeed;
    [SerializeField] bool isGroundSlam;
    [SerializeField] float groundedGroundSlamJumpHeight;
    [SerializeField] float airGroundSlamJumpHeight;
    [SerializeField] float groundSlamForwardBoost;

    void Awake()
    {
        if (characterFunc == null) characterFunc = this;
    }

    void Update()
    {
        base.Update();

        if (!playerAttack.isAttacking) 
        {
            chargingThrust = false;
        }

        if (chargingThrust)
        {
            timeChargingThrust += Time.deltaTime;
        } else {
            timeChargingThrust = 0;
        }

        if (chargingThrust && !inputs.attackHeld)
        {
            DoThrust();
        }
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

        if (!playerAttack.isAttacking) 
        {
            isThrusting = false;
        }

        if (isThrusting)
        {
            float vertical = 0;
            if (inputs.upHeld) vertical++;
            if (inputs.downHeld) vertical--;
            float mult = thrustSpeedMult * thrustSpeedAnimMult.localPosition.x;
            rb.velocity = new Vector2(baseThrustSpeed * facing, baseThrustVerticalSpeed * vertical) * mult;
        }
    }

    public string FindAttackName()
    {
        string attackName = "nope";
        
        if (inputs.downHeld)
        {
            attackName = "downAttack";
        }
        else if (inputs.leftHeld || inputs.rightHeld)
        {
            attackName = "slash1";
        }
        else
        {
            attackName = "thrustCharge";
        }

        return attackName;
    }

    public void StartStartThrustCharge()
    {
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y * 0.5f,0));
        enableJumpReleaseFall = false;
    }
    public void StartThrustCharge()
    {
        chargingThrust = true;
        timeChargingThrust = 0;
    }
    public void DoThrust()
    {
        chargingThrust = false;
        isThrusting = true;

        for (int i = 0; i < thrustLevelChargeThresholds.Length; i++)
        {
            if (timeChargingThrust > thrustLevelChargeThresholds[i]) 
            {
                thrustChargeLevel = i;
            } else {
                break;
            }
        }
        timeChargingThrust = 0;
        thrustSpeedMult = thrustLevelSpeedMults[thrustChargeLevel];
        thrustHitbox.attackInfo = thrustLevelSAttackInfo[thrustChargeLevel];

        playerAttack.AttackByName(thrustAttackName);
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
