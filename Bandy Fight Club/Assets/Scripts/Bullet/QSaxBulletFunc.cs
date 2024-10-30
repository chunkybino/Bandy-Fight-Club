using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QSaxBulletFunc : BulletFunc
{
    public int bounceCount = 0;

    public bool bounceState = false;

    public Vector2 bounceVel = new Vector2(10, 25);

    [SerializeField] float wallBounceForce;
    [SerializeField] float wallBounceVerticalBoost;

    public float throwRotationSpeed;
    public float bounceRotationSpeed;

    Vector2 lastFrameVel;
    Vector2 last2FrameVel;

    bool isGrappled;
    public bool grappleable = true;
    bool isGrappleParried;

    [SerializeField] float getGrappledTimeoutLifetime = 3f;

    [SerializeField] float grappleParrySpeed = 60;
    [SerializeField] float grappleParryDamageMult = 1.5f;

    void Start()
    {
        base.Start();

        lastFrameVel = rb.velocity;
    }

    void Update()
    {
        if (isGrappled) return;

        base.Update();

        doGrav = bounceState;

        float rotSpeed = throwRotationSpeed;
        if (bounceState) rotSpeed = bounceRotationSpeed;
        rotSpeed *= UFunc.Sign(direction.x);
        renderer.transform.localEulerAngles = renderer.transform.localEulerAngles + new Vector3(0,0,rotSpeed*Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (isGrappled) return;

        base.FixedUpdate();

        last2FrameVel = lastFrameVel;
        lastFrameVel = rb.velocity;
    }

    public override void HitDude(IDamageable damageable)
    {
        base.HitDude(damageable);

        if (isGrappleParried) Die();

        bounceState = true;
        direction = new Vector2(-direction.x,direction.y);
        rb.velocity = new Vector2(UFunc.Sign(direction.x),1) * bounceVel;
    }

    public override void HitWall(Collider2D wall)
    {
        if (isGrappleParried) Die();

        bounceState = true;

        Vector2 wallPoint = wall.ClosestPoint(transform.position);
        Vector2 wallDistance = wallPoint - new Vector2(transform.position.x,transform.position.y);

        Vector2 bounceVec = UFunc.BounceVector(last2FrameVel.normalized, wallDistance);

        rb.velocity = bounceVec.normalized*wallBounceForce;
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + wallBounceVerticalBoost);

        Vector2 translationVec = wallDistance.normalized*1.1f*(wallDistance.magnitude-(circleCollider.radius*transform.localScale.x));//print(wallPoint.x);print(transform.position.x);print(wallPoint.x-transform.position.x);print(wallDistance);print(translationVec);
        transform.position += new Vector3(translationVec.x,translationVec.y,0);

        direction = new Vector2(UFunc.Sign(rb.velocity.x),direction.y);
    }

    public void GetGrappled()
    {
        isGrappled = true;
        grappleable = false;
        rb.velocity = new Vector2(0,0);
        circleCollider.enabled = false;
        DoLifetime(getGrappledTimeoutLifetime);
    }

    public void GetGrappleParried(Vector2 parryDirection)
    {
        isGrappled = false;
        isGrappleParried = true;

        rb.velocity = parryDirection * grappleParrySpeed;
        direction = parryDirection;
        doGrav = false;
        bounceState = false;

        circleCollider.enabled = true;

        attackInfo.damage = Mathf.FloorToInt(attackInfo.damage * grappleParryDamageMult);

        ClearHitList(); 
    }
}
