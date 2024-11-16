using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JCloneFunc : MonoBehaviour
{
    public Vector2 direction;

    public int team;

    public int facing = 1;

    public Rigidbody2D rb;
    [SerializeField] PlayerAnimation playerAnimation;
    [SerializeField] PlayerMeshParent meshParent;
    [SerializeField] Transform textureTransform;

    public Vector2 forceVector;
    public float forceMagnitude;
    public float attackForce = 24f;

    [SerializeField] float forceDecaySpeed = 1.5f;
    [SerializeField] AnimationCurve forceDecayCurve;
    [SerializeField] float attackForceTimer;

    [SerializeField] float attackBoostDelay = 0.25f;

    [SerializeField] float deactivateTime = 1;

    public bool active;

    [SerializeField] AttackHitbox cloneAttackHitbox;

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
        if (direction.x > 0) facing = 1;
        if (direction.x < 0) facing = -1;

        float yRot = UFunc.Sign(direction.y) * 90 * facing;
        if (direction.x != 0) yRot *= 0.5f;

        textureTransform.eulerAngles = new Vector3(0,0,yRot);
        textureTransform.localScale = new Vector3(facing,1,1);

        cloneAttackHitbox.ClearHitList();

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

    public void SetTeam(int theTeam)
    {
        meshParent.team = theTeam;
        team = theTeam;
        cloneAttackHitbox.team = theTeam;
    }

    public void DoAttack()
    {
        playerAnimation.PlayAnim("CloneAttack");
        Invoke("DoAttackBoost", attackBoostDelay);
    }
    public void DoAttackBoost()
    {   
        forceMagnitude = attackForce;

        attackForceTimer = 0;
    }
}
