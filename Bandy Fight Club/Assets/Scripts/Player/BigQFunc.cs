using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigQFunc : PlayerFunc, ICharacterFunc
{
    public float attackOutOfAirDodgeSpeed = 24;
    public float attackOutOfAirDodgeAccelerationTime = 0.4f;
    public float attackOutOfAirDodgeAccelerationMult = 0.15f;
    public bool doAttackOutOfAirDodgeAcceleration;

    public BulletConfig saxBulletConfig;

    public QSaxBulletFunc currentSaxBullet;
    [SerializeField] float saxGrappleSpeed = 50f;
    [SerializeField] Vector2 saxGrappleVel;
    [SerializeField] bool isSaxGrappling;
    [SerializeField] bool isSaxGrapplingMovement;
    [SerializeField] float saxGrappleSnapDistance; //when we finishe grapple, snap to the sax location if we are close enough
    [SerializeField] float saxGrappleDelay = 0.2f;
    [SerializeField] float saxPreGrappleVelDamp = 0.5f; //multiply vel by this every second, 
    float timeSaxGrappling;
    Vector2 saxGrapplePos;
    Vector2 saxDistance;
    [SerializeField] LineRenderer saxGrappleLine;
    [SerializeField] Transform saxGrappleLineFirePoint;

    [SerializeField] float saxThrowStartVerticalBoost = 10;

    [SerializeField] Vector2 saxGrappleVecBoost;
    [SerializeField] float saxGrappleVerticalBoost;
    [SerializeField] float grappleSaxBoostAccelerationMult = 0.2f;
    [SerializeField] float grappleSaxBoostAccelerationTime = 0.2f;

    [SerializeField] float saxParryVerticalBoost = 8;
    [SerializeField] float saxParryVerticalVelMult = 0.5f;
    [SerializeField] float saxParryHitstop = 0.15f;
    [SerializeField] float saxParryCameraShake = 0.5f;

    [SerializeField] Transform renderToRotate; //rotate this when grappling, rotate towards sax

    [SerializeField] bool attackChargeHeld;
    [SerializeField] float timeAttackCharging;
    [SerializeField] float attackChargeNeeded = 0.5f;

    [SerializeField] ParticleSystem chargeParticles;
    [SerializeField] float chargeParticleTime = 0.35f;

    [SerializeField] string saxGrappleBoostSound;
    [SerializeField] float saxGrappleBoostSoundVol = 1;
    [SerializeField] float saxGrappleBoostSoundPitch = 1;

    void Awake()
    {
        if (characterFunc == null) characterFunc = this;
    }
    void Update()
    {
        base.Update();

        if (characterFunc == null) characterFunc = this;

        saxGrappleLine.gameObject.SetActive(isSaxGrappling);
        if (isSaxGrappling)
        {
            timeSaxGrappling += Time.deltaTime;

            DoGrappleSaxLine();
        }
        else
        {
            timeSaxGrappling = 0;
        }

        if (isSaxGrapplingMovement) //rorate to target
        {
            float angle = facing * UFunc.VecToAngle(saxDistance);
            if (facing == 1) angle += 180;
            renderToRotate.transform.eulerAngles = new Vector3(0,0,angle);
        }
        else
        {
            renderToRotate.transform.eulerAngles = new Vector3(0,0,0);
        }

        //charge attacks
        if (!inputs.attackHeld || !GetIsActionable())
        {
            attackChargeHeld = false;

            if (chargeParticles.isPlaying) chargeParticles.Stop();
        }
        if (attackChargeHeld)
        {
            if (playerAttack.canAttack) timeAttackCharging += Time.deltaTime;

            if (timeAttackCharging >= attackChargeNeeded) 
            {
                DoChargeAttack();
                attackChargeHeld = false;
                timeAttackCharging = 0;
            }

            if (!chargeParticles.isPlaying) chargeParticles.Play();
        }

        if (attackChargeNeeded-timeAttackCharging < chargeParticleTime && playerAttack.canAttack && attackChargeHeld)
        {
            if (!chargeParticles.isPlaying) chargeParticles.Play();
        }
        else
        {
            if (chargeParticles.isPlaying) chargeParticles.Stop();
        }
    }
    void FixedUpdate()
    {
        if (!isSaxGrappling)
        {
            base.FixedUpdate();
        }
        else
        {
            saxDistance = new Vector2(transform.position.x, transform.position.y) - saxGrapplePos;

            if (isSaxGrapplingMovement)
            {
                rb.velocity = saxGrappleVel;
            }
            else
            {
                rb.velocity *= Mathf.Pow(saxPreGrappleVelDamp, Time.fixedDeltaTime); //slow down exponentially
            }
        }
    }

    public override void OnAttack()
    {
        bool dodging = isDodging; //get the variable before it gets set by the base script

        base.OnAttack();

        if (dodging && !grounded) //if we attack out of dodge in the air, let us keep our speed
        {
            EndDodge();
            doDodgeMovement = false;
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
            else if (inputs.downHeld)
            {
                attackName = "Slide";
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

        attackChargeHeld = true;
        timeAttackCharging = 0;

        return attackName;
    }
    public string FindAttackHeavyName()
    {
        string attackName = "SaxThrow";

        return attackName;
    }
    public void DoChargeAttack()
    {
        string name = "SpinKick";

        if (inputs.upHeld)
        {
            name = "SuperHighKick";
        }
        else if (inputs.downHeld)
        {
            name = "Helicopter";
        }


        playerAttack.AttackByName(name);
    }

    public string FindAnimationName()
    {
        if (isSaxGrappling) 
        {
            if (!isSaxGrapplingMovement) //before we;'ve started mvoing
            {
                return "SendGrapple";
            }
            else //once we've started moving
            {
                return "GrapplePull";
            }   
        }

        return "";
    }

    public void SaxThrowAttack()
    {
        if (!currentSaxBullet) //throw sax if we havent already
        {
            playerAnimation.PlayAnim("ThrowSax");

            if (!grounded) rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, saxThrowStartVerticalBoost)); //boost upa  bit when we throw sax when not grounded
        }
        else
        {
            if (currentSaxBullet.grappleable) 
            {
                playerAnimation.ExitActionAnim();
                GrappleToSaxStart();
            }
            else
            {
                playerAttack.CancelAttack();
            }
        }
    }
    public void ShootSax()
    {
        BulletFunc bullet = BulletHandler.ShootBulletStatic(saxBulletConfig, transform.position, new Vector2(facing, 0), team);
        QSaxBulletFunc sax = bullet.GetComponent<QSaxBulletFunc>();
        if (sax) currentSaxBullet = sax;
    }

    void GrappleToSaxStart()
    {
        if (!currentSaxBullet) return;

        currentSaxBullet.GetGrappled();
        currentSaxBullet.SetTeam(team);

        isSaxGrappling = true;

        facing = UFunc.Sign(currentSaxBullet.transform.position.x - transform.position.x);

        enableJumpReleaseFall = false;

        playerAnimation.ExitAnim();

        Invoke("GrappleToSax", saxGrappleDelay);
    }
    void GrappleToSax()
    {
        if (!currentSaxBullet) 
        {
            EndSaxGrapple();
            return;
        }

        //playerAnimation.PlayAnim("AirKick");

        isSaxGrapplingMovement = true;

        Vector2 saxDistance = currentSaxBullet.transform.position - transform.position; 
        saxGrappleVel = saxDistance.normalized * saxGrappleSpeed;

        float grappleTime = saxDistance.magnitude/saxGrappleSpeed;
        Invoke("EndSaxGrapple", grappleTime);
    }
    void EndSaxGrapple()
    {
        isSaxGrappling = false;
        isSaxGrapplingMovement = false;

        if (!currentSaxBullet) return;

        HitSaxGrapple();
    }
    void HitSaxGrapple()
    {
        Vector2 saxDistance = currentSaxBullet.transform.position - transform.position;
        if (saxDistance.magnitude <= saxGrappleSnapDistance) transform.position = currentSaxBullet.transform.position; //snap to sax location

        enableJumpReleaseFall = false;
        playerAttack.SetAttackingSpeedMult(1);

        if (inputs.attackHeld)
        {
            Parry();
            playerAttack.attackBufferTimer = 0;
        } 
        else
        {
            Boost();
            currentSaxBullet.Die();
        }

        void Boost()
        {
            Vector2 boost = saxGrappleVecBoost * saxGrappleVel.normalized;
            boost += new Vector2(0, saxGrappleVerticalBoost);
            rb.velocity = boost;
            maintainSpeed = boost.x;

            DoAccelerationMult(grappleSaxBoostAccelerationMult, grappleSaxBoostAccelerationTime);

            audio.Play(saxGrappleBoostSound, saxGrappleBoostSoundVol, saxGrappleBoostSoundPitch);
        }
        void Parry()
        {
            UpdateFacing();
            currentSaxBullet.GetGrappleParried(new Vector2(facing, verticalInputDirection).normalized);
            float verticalVel = Mathf.Sqrt(Mathf.Abs(rb.velocity.y)*saxParryVerticalVelMult) * UFunc.Sign(rb.velocity.y); //take the abs value cause of sqrt, then multiply by sign
            rb.velocity = new Vector2(rb.velocity.x, verticalVel + saxParryVerticalBoost);

            GameManager.DoHitStopStatic(saxParryHitstop);
            GameManager.DoCameraShakeStatic(saxParryCameraShake);
        }
    }
    void DoGrappleSaxLine()
    {
        if (!saxGrappleLine) return;

        if (currentSaxBullet) saxGrapplePos = currentSaxBullet.transform.position;

        float progress = Mathf.Clamp01(timeSaxGrappling/saxGrappleDelay);

        Vector3 firePos = saxGrappleLineFirePoint.position - transform.position; //offset from player
        firePos.x *= facing;
        firePos += transform.position; //add position back

        Vector3 pos2 = UFunc.LerpVector3(firePos, saxGrapplePos, progress);

        saxGrappleLine.positionCount = 2;
        saxGrappleLine.SetPosition(0, firePos);
        saxGrappleLine.SetPosition(1, pos2);
    }

    public override void GetStunned(float time)
    {
        base.GetStunned(time);

        //if we're grappling
        if (isSaxGrappling)
        {
            isSaxGrappling = false;
            isSaxGrapplingMovement = false;
            if (currentSaxBullet) currentSaxBullet.Die();
        }
    }

    public override bool GetCanAttack()
    {
        if (isSaxGrappling)
        {
            return false;
        }
        else
        {
            return base.GetCanAttack();
        }
    }
    public override bool GetFallThroughPlatform()
    {
        return isSaxGrappling; //go through platform while grappling
    }
    public override bool GetDoAnimation()
    {
        if (isSaxGrappling) return true;
        return base.GetDoAnimation();
    }
}
