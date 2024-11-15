using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JCloneFunc : MonoBehaviour
{
    public Vector2 direction;

    public Rigidbody2D rb;
    [SerializeField] PlayerAnimation playerAnimation;
    [SerializeField] PlayerMeshParent meshParent;

    public Vector2 forceVector;
    public float forceMagnitude;
    public float attackForce = 24f;

    [SerializeField] float attackForceDecaySpeed = 20f;
    [SerializeField] float currentForce;

    [SerializeField] float forceDecaySpeed = 1.5f;
    [SerializeField] AnimationCurve forceDecayCurve;
    [SerializeField] float attackForceTimer;

    [SerializeField] float attackBoostDelay = 0.25f;

    [SerializeField] float deactivateTime = 1;

    public bool active;

    void Update()
    {
        attackForceTimer += forceDecaySpeed*Time.deltaTime;

        rb.velocity = forceVector;

        forceMagnitude = attackForce * forceDecayCurve.Evaluate(attackForceTimer);
        forceVector = forceMagnitude * direction;
    }

    public void Spawn(int renderNum)
    {
        meshParent.num = renderNum;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        active = true;

        DoAttack();

        CancelInvoke("Deactivate");
        Invoke("Deactivate", deactivateTime);
    }
    public void Deactivate()
    {
        forceVector = new Vector2(0,0);

        active = false;
        gameObject.SetActive(false);
    }

    public void SetTeam(int team)
    {
        meshParent.team = team;
    }

    public void DoAttack()
    {
        Invoke("DoAttackBoost", attackBoostDelay);
    }
    public void DoAttackBoost()
    {   
        forceMagnitude = attackForce;

        attackForceTimer = 0;
    }
}
