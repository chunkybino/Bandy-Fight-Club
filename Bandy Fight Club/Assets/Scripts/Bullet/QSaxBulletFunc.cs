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

    [SerializeField] float deflectVerticalDirectionMult = 0.5f;

    [SerializeField] string saxClashTag = "sax"; //bounce off of bullet with this tag
    [SerializeField] float saxClashSpeed = 120f;
    [SerializeField] float saxClashDamageMult = 5f;
    bool isSaxClash;
    [SerializeField] float saxClashHitStop = 0.5f;
    [SerializeField] float saxClashCameraShake = 3f;
    [SerializeField] Knockback saxClashKnockback;

    [SerializeField] string saxClashSoundName;

    [SerializeField] List<string> hitSounds = new List<string>();
    [SerializeField] float hitSoundsVol = 1;
    [SerializeField] float hitSoundsPitch = 1;

    [SerializeField] List<string> getParrySounds = new List<string>();
    [SerializeField] float getParryVol = 1;
    [SerializeField] float getParryPitch = 1;

    [SerializeField] List<string> hitParrySounds = new List<string>();
    [SerializeField] float hitParryVol = 1;
    [SerializeField] float hitParryPitch = 1;

    [SerializeField] string deflectSound;
    [SerializeField] float deflectSoundVol = 1;
    [SerializeField] float deflectSoundPitch = 1;

    [SerializeField] string bounceSound;
    [SerializeField] float bounceSoundVol = 1;
    [SerializeField] float bounceSoundPitch = 1;
    [SerializeField] string bounceSound2;
    [SerializeField] float bounceSound2Vol = 1;
    [SerializeField] float bounceSound2Pitch = 1;

    void Start()
    {
        base.Start();

        lastFrameVel = rb.velocity;
    }

    public override void Update()
    {
        if (isGrappled) return;

        base.Update();

        doGrav = bounceState;

        float rotSpeed = throwRotationSpeed;
        if (bounceState) rotSpeed = bounceRotationSpeed;
        rotSpeed *= UFunc.Sign(direction.x);
        renderer.transform.localEulerAngles = renderer.transform.localEulerAngles + new Vector3(0,0,rotSpeed*Time.deltaTime);
    }

    public override void FixedUpdate()
    {
        if (isGrappled) return;

        base.FixedUpdate();

        last2FrameVel = lastFrameVel;
        lastFrameVel = rb.velocity;
    }

    public override void HitDude(IDamageable damageable)
    {
        base.HitDude(damageable);

        if (isGrappleParried) 
        {
            GameManager.singleton.audio.PlayList(hitParrySounds, hitParryVol, hitParryPitch);
            Die();
        }
        else
        {
            GameManager.singleton.audio.PlayList(hitSounds, hitSoundsVol, hitSoundsPitch);
        }

        if (damageable.damageableTags.Contains(saxClashTag)) return;

        if (isSaxClash) Die(); //do this after we check for a clash, cuase sometimes the bullets multihit and kill themslves on eacheother

        bounceState = true;
        direction = new Vector2(-direction.x,direction.y);
        rb.velocity = new Vector2(UFunc.Sign(direction.x),1) * bounceVel;
    }

    public override void HitWall(Collider2D wall)
    {
        if (isGrappleParried || isSaxClash) Die();

        bounceState = true;

        Vector2 wallPoint = wall.ClosestPoint(transform.position);
        Vector2 wallDistance = wallPoint - new Vector2(transform.position.x,transform.position.y);

        Vector2 bounceVec = UFunc.BounceVector(last2FrameVel.normalized, wallDistance);

        rb.velocity = bounceVec.normalized*wallBounceForce;
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + wallBounceVerticalBoost);

        Vector2 translationVec = wallDistance.normalized*1.1f*(wallDistance.magnitude-(circleCollider.radius*transform.localScale.x));//print(wallPoint.x);print(transform.position.x);print(wallPoint.x-transform.position.x);print(wallDistance);print(translationVec);
        transform.position += new Vector3(translationVec.x,translationVec.y,0);

        direction = new Vector2(UFunc.Sign(rb.velocity.x),direction.y);

        GameManager.singleton.audio.Play(bounceSound, bounceSoundVol, bounceSoundPitch);
        GameManager.singleton.audio.Play(bounceSound2, bounceSound2Vol, bounceSound2Pitch);
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

        GameManager.singleton.audio.PlayList(getParrySounds, getParryVol, getParryPitch);

        ClearHitList(); 
    }

    public override AttackHitReturnInfo GetHit(FullAttackInfo info)
    {
        if (info.info.tags.Contains(saxClashTag))
        {
            isSaxClash = true;

            rb.velocity = rb.velocity.normalized * saxClashSpeed * -1;
            direction = new Vector2(UFunc.Sign(rb.velocity.x),direction.y);

            team.team = 5; //hit everyone

            GameManager.DoHitStopStatic(saxClashHitStop);
            GameManager.DoCameraShakeStatic(saxClashCameraShake);

            attackInfo.damage = Mathf.FloorToInt(attackInfo.damage * saxClashDamageMult);
            attackInfo.knockbacks[0] = saxClashKnockback;

            GameManager.singleton.audio.Play(saxClashSoundName);

            return null;
        }
        else
        {
            return base.GetHit(info);
        }
    }
    public override void DoDeflectSpeed(Vector2 dir)
    {
        if (doGrav)
        {
            dir = dir.normalized;
            dir = new Vector2(dir.x*speed + (deflectSpeedAdd*Mathf.Sign(dir.x)), dir.y*speed);
            dir = new Vector2(dir.x, dir.y + rb.velocity.y*deflectVerticalDirectionMult);
            rb.velocity = dir;
        }
        else
        {
            base.DoDeflectSpeed(dir);
        }

        GameManager.singleton.audio.Play(deflectSound, deflectSoundVol, deflectSoundPitch);
    }
}
