using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigQFunc : PlayerFunc, ICharacterFunc
{
    public float attackOutOfAirDodgeSpeed = 24;
    public float attackOutOfAirDodgeAccelerationTime = 0.4f;
    public float attackOutOfAirDodgeAccelerationMult = 0.15f;
    public bool doAttackOutOfAirDodgeAcceleration;

    void Awake()
    {
        if (characterFunc == null) characterFunc = this;
    }
    void Update()
    {
        base.Update();

        if (characterFunc == null) characterFunc = this;
    }

    public override void OnAttack()
    {
        if (isDodging && !grounded) //if we attack out of dodge in the air, let us keep our speed
        {
            EndDodge();
            maintainSpeed = attackOutOfAirDodgeSpeed * dodgeDirection.x;

            doAttackOutOfAirDodgeAcceleration = true;
            CancelInvoke("EndAttackOutOfAirDodgeAcceleration");
            Invoke("EndAttackOutOfAirDodgeAcceleration", attackOutOfAirDodgeAccelerationTime);
        }
    }

    void EndAttackOutOfAirDodgeAcceleration()
    {
        CancelInvoke("doAttackOutOfAirDodgeAcceleration");
        doAttackOutOfAirDodgeAcceleration = false;
    }

    public override float CalculateAccelerationMultOverride()
    {
        float mult = CalculateAccelerationMult();
        if (doAttackOutOfAirDodgeAcceleration) mult *= attackOutOfAirDodgeAccelerationMult;
        return mult;
    }

    public string FindAttackName()
    {
        string attackName = "nope";

        if (grounded)
        {
            if (inputs.upHeld)
            {
                attackName = "Kick";
            }
            else
            {
                attackName = "Knee";
            }
        }
        else
        {
            attackName = "AirKick";
        }

        return attackName;
    }
}
