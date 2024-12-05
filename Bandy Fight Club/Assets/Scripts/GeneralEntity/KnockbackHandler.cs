using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KnockbackHandler : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    //[SerializeField] PlayerStatus playerStatus;
    //[SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerFunc playerFunc;

    //[SerializeField] EnemyStatus enemyStatus;
    //[SerializeField] EnemyFunc enemyFunc;

    [SerializeField] GroundChecker groundCheck;

    public Vector2 baseKnockbackResistance = new Vector2(1,1);
    public Vector2 knockbackResistanceMult = new Vector2(1,1);

    [SerializeField] Knockback doKnockbackToDo;
    [SerializeField] bool doKnock;
    [SerializeField] Vector2 knockDirection = new Vector2(1,0);

    void Start()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
    }

    void LateUpdate()
    {
        if (doKnock)
        {
            doKnock = false;
            GetKnockbacked(doKnockbackToDo,knockDirection);
        }
    }

    public void GetKnockbacked(Knockback knockback)
    {
        GetKnockbacked(knockback, new Vector2(1,1));
    }
    public void GetKnockbacked(Knockback knockback, float direction) //for if were facing left or right or smthing like that
    {
        GetKnockbacked(knockback, new Vector2(direction,1));
    }
    public void GetKnockbacked(Knockback knockback, Vector2 direction)
    {
        Vector2 newVel = rb.velocity;
        
        if (knockback.normalizeVel)
        {
            direction = direction.normalized;
        }
        else
        {
            direction = new Vector2(UFunc.FloatToDirection(direction.x), UFunc.FloatToDirection(direction.y));
        }
        
        Vector2 resistance = knockbackResistanceMult * baseKnockbackResistance;
        switch (knockback.ignoreBaseKnockbackResistance)
        {
            case Knockback.GroundedKnockback.X:
                resistance = new Vector2(knockbackResistanceMult.x, resistance.y);
                break;
            case Knockback.GroundedKnockback.Y:
                resistance = new Vector2(resistance.x, knockbackResistanceMult.y);
                break;
            case Knockback.GroundedKnockback.Both:
                resistance = knockbackResistanceMult;
                break;
        }
               
        newVel = new Vector2(
        VelCalc(knockback.xVelType, rb.velocity.x, resistance.x * ((knockback.vel.x * direction.x) + knockback.fixedVel.x)),
        VelCalc(knockback.yVelType, rb.velocity.y, resistance.y * ((knockback.vel.y * direction.y) + knockback.fixedVel.y)));

        if (playerFunc)
        {
            if (knockback.yVelType != Knockback.KnockbackType.None) playerFunc.DisableJumpReleaseFall();
        }

        if (groundCheck)
        {
            if (groundCheck.grounded)
            {
                switch (knockback.dontDoAgainstGroundedTarget)
                {
                    case Knockback.GroundedKnockback.X:
                        newVel = new Vector2(rb.velocity.x, newVel.y);
                        break;
                    case Knockback.GroundedKnockback.Y:
                        newVel = new Vector2(newVel.x, rb.velocity.y);
                        break;
                    case Knockback.GroundedKnockback.Both:
                        newVel = new Vector2(rb.velocity.x, rb.velocity.y);
                        break;
                }
            }
        }

        rb.velocity = newVel;

        if (playerFunc) playerFunc.DoAccelerationMult(knockback.acceleration, knockback.accelerationTime);

        float VelCalc(Knockback.KnockbackType knockbackType, float currentVel, float knockbackVel)
        {
            float thisDirection = UFunc.Sign(knockbackVel);
            switch (knockbackType)
            {
                case Knockback.KnockbackType.None:
                    return currentVel;
                    break;
                case Knockback.KnockbackType.Add:
                    return currentVel + knockbackVel;
                    break;
                case Knockback.KnockbackType.Set:
                    return knockbackVel;
                    break;
                case Knockback.KnockbackType.Min:
                    if (thisDirection == 0) return currentVel;
                    return thisDirection * Mathf.Max(knockbackVel*thisDirection, currentVel*thisDirection);
                    break;
                case Knockback.KnockbackType.Max:
                    if (thisDirection == 0) return currentVel;
                    return thisDirection * Mathf.Min(knockbackVel*thisDirection, currentVel*thisDirection);
                    break;
                case Knockback.KnockbackType.Lo:
                    return Mathf.Max(knockbackVel, currentVel);
                    break;
                case Knockback.KnockbackType.Hi:
                    return Mathf.Min(knockbackVel, currentVel);
                    break;
                default:
                    return currentVel;
                    break;
            }
        }
    }
}

[System.Serializable]
public struct Knockback
{
    public string name;
    public Vector2 vel;
    public Vector2 fixedVel;
    [Tooltip("none = keep current vel, set = set vel, add = add to current vel, " +
    "min = set vel to this unless its already larger (further from zero), max = set vel to this unless its already smaller (further from zero) " +
    "lo = same as min but non absolute, hi = same as max but non absolute")]
    public KnockbackType xVelType;
    [Tooltip("check xveltype tooltip")]
    public KnockbackType yVelType;
    public bool normalizeVel;
    [Tooltip("EX. x = dont do x velocity against grounded target")]
    public GroundedKnockback dontDoAgainstGroundedTarget;

    public GroundedKnockback ignoreBaseKnockbackResistance;

    public float acceleration;
    public float accelerationTime;

    public enum KnockbackType {None, Set, Add, Min, Max, Lo, Hi}
    public enum GroundedKnockback {None, X, Y, Both}
}
