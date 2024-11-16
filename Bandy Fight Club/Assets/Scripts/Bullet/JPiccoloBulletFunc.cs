using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JPiccoloBulletFunc : BulletFunc
{
    [SerializeField] float rotationSpeed;
    [SerializeField] Transform rotationTranform; //the target of rotationSpeed

    public float timeActive;
    public float activateTime = 0.25f;
    public bool isActive {get{return timeActive >= activateTime;}}

    [SerializeField] float jumpForce;

    public bool canJump = true;
    [SerializeField] ParticleSystem jumpParticles;

    public override void Update()
    {
        base.Update();

        float rotSpeed = rotationSpeed;
        rotSpeed *= UFunc.Sign(direction.x);
        rotationTranform.localEulerAngles = rotationTranform.localEulerAngles + new Vector3(0,0,rotSpeed*Time.deltaTime);

        timeActive += Time.deltaTime;
    }

    public void InheritVel(Vector2 throwMult, Vector2 velAdd)
    {
        speed = config.speed;
        rb.velocity = direction * speed;

        rb.velocity *= throwMult;

        rb.velocity += velAdd;
    }

    public void DoJump()
    {
        if (!canJump) return;

        rb.velocity = new Vector2(0, jumpForce);
        canJump = false;

        jumpParticles.Play();
    }

    public override void SetStartSpeed()
    {
        //overwrite this funcition because otherwise it will voerrite the inherited velocity
    }

    public override void GetDeflected(Vector2 deflectForce)
    {
        base.GetDeflected(deflectForce);

        canJump = false;
    }
}
