using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigQFunc : PlayerFunc
{
    public float attackOutOfAirDodgeSpeed = 24;
    public float attackOutOfAirDodgeAccelerationTime = 0.6f;

    public void OnAttack()
    {
        if (isDodging && !grounded) //if we attack out of dodge in the air, let us keep our speed
        {
            EndDodge();
            maintainSpeed = attackOutOfAirDodgeSpeed * dodgeDirection.x;
            DoAccelerationMult(0, attackOutOfAirDodgeAccelerationTime);
        }
    }
}
