using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour, IDamageable
{
    public Rigidbody2D rb;
    public PlayerStatus playerStatus;
    public PlayerFunc playerFunc;
    public GroundChecker groundCheck;
    public KnockbackHandler knockbackHandler;

    public Shaker renderShaker;
    public float hitShakeStrength = 1;

    public BoxCollider2D hitbox;
    public BoxCollider2D boundingBox;
    public BoxCollider2D entityBounding;
    public BoxCollider2D platformBox;

    Vector2 normalHitboxSize;
    Vector2 normalHitboxOffset;
    Vector2 normalBoundingSize;
    Vector2 normalBoundingOffset;
    public BoxCollider2D hitboxSizeReference;

    public bool isIntangible;

    bool boundBoxSizeChanged = false;

    [SerializeField] float dropThroughPlatformTime = 0.1f;
    float dropThroughPlatformTimer;
    bool dropThroughPlatform;

    [SerializeField] float guardingCameraShakeMult = 0.5f;

    [SerializeField] Knockback perfBlockReturnKnockback; //when we perf block, knock the enemy back a bit

    [SerializeField] ParticleSystem stunBlockParticles;
    [SerializeField] float stunBlockParticleSize = 0.5f;

    public float boundingTopBound {get{return transform.position.y + boundingBox.offset.y + boundingBox.size.y/2;}}
    public float boundingBottomBound {get{return transform.position.y + boundingBox.offset.y - boundingBox.size.y/2;}}

    void Awake()
    {
        normalHitboxSize = hitbox.size;
        normalHitboxOffset = hitbox.offset;
        normalBoundingSize = boundingBox.size;
        normalBoundingOffset = boundingBox.offset;

        Physics2D.IgnoreCollision(boundingBox, entityBounding);
        Physics2D.IgnoreCollision(platformBox, entityBounding);
    }

    void Update()
    {
        float startGroundHeight = groundCheck.groundHeight;
        float startGroundDelta = groundCheck.groundHeight + (platformBox.size.y/2) - platformBox.transform.position.y;

        Vector2 hitSize = normalHitboxSize;
        Vector2 hitOffset = normalHitboxOffset;
        Vector2 boundSize = normalBoundingSize;
        Vector2 boundOffset = normalBoundingOffset;

        if (hitboxSizeReference.gameObject.activeSelf)
        {
            hitSize = hitboxSizeReference.size;
            hitOffset = hitboxSizeReference.offset;

            boundSize = hitboxSizeReference.size;
            boundOffset = hitboxSizeReference.offset;
        }

        hitbox.size = hitSize;
        hitbox.offset = hitOffset;
        boundingBox.size = boundSize;
        boundingBox.offset = boundOffset;

        //entity bounding
        entityBounding.size = boundingBox.size;
        entityBounding.offset = boundingBox.offset;

        //platform bounding
        float platformBoxHeight = boundingBox.offset.y - (boundingBox.size.y/2);
        float platformHeightChange = platformBoxHeight - platformBox.transform.localPosition.y;
        platformBox.size = new Vector2(boundingBox.size.x, platformBox.size.y);
        platformBox.transform.localPosition = new Vector2(boundingBox.offset.x, platformBoxHeight + (platformBox.size.y/2)); //move platform boudning to bottom of normal bbounding

        UFunc.TickTimer(ref dropThroughPlatformTimer);
        dropThroughPlatform = dropThroughPlatformTimer > 0;
        if (playerFunc.GetFallThroughPlatform()) dropThroughPlatform = true;
        platformBox.enabled = !dropThroughPlatform;

        //shiftup
        if (platformHeightChange < -0.1f && !dropThroughPlatform) //if we get moved down
        {
            groundCheck.DoGroundCheckNow(false);
            float groundDelta = startGroundHeight + (platformBox.size.y/2) - platformBox.transform.position.y;
            if (groundDelta > 0 && startGroundDelta < 0)
            {
                CheckForGroundShiftUp();
            }
        }

        //intagibility
        isIntangible = playerFunc.GetIsIntangible();//isDodgeIntangible;
        entityBounding.enabled = !playerFunc.GetDisableEntityCollision();

        //disable sutff if dead
        if (playerStatus.isDead)
        {
            hitbox.enabled = false;
            entityBounding.enabled = false;
        }
    }

    void CheckForGroundShiftUp() //if we in the ground, shift us up
    {
        float groundHeight = groundCheck.groundHeight;
        float boundingBottomHeight = boundingBox.transform.position.y + boundingBox.offset.y - (boundingBox.size.y/2); //y pos of bottom fo bounding box
        float groundHeightDelta = boundingBottomHeight - groundHeight;
        if (groundHeightDelta < 0) //if our boundign box goes below the ground, move us up
        {
            rb.transform.position = new Vector3(rb.transform.position.x, rb.transform.position.y - groundHeightDelta, rb.transform.position.z);
        }
    }

    public AttackHitReturnInfo GetHit(FullAttackInfo fullInfo)
    {
        AttackHitReturnInfo returnInfo = new AttackHitReturnInfo();

        AttackInfo info = fullInfo.info;
        if (!isIntangible)
        {
            bool block = playerFunc.isBlocking;
            if (info.onlyPerfBlock && !playerFunc.isPerfBlocking) block = false;
            if (info.unblockable) block = false;

            if (!block || playerFunc.isStun) //get hity=
            {
                Hit();
            }
            else //if we guard
            {
                Blocked();
            }

            returnInfo.hit = true;
        }
        else
        {
            returnInfo.hit = false;
        }

        return returnInfo;

        void Hit()
        {
            playerStatus.GetHit(info);
            playerFunc.GetStunned(info.stunTime);
            foreach (Knockback knockback in info.knockbacks)
            {
                knockbackHandler.GetKnockbacked(knockback, fullInfo.direction);
            }
            renderShaker.DoShake(hitShakeStrength, fullInfo.direction);

            if (playerFunc.stunBlocking)
            {
                ParticleSystem particle = Instantiate(stunBlockParticles, transform.position, Quaternion.identity);
                particle.transform.localScale = new Vector2(stunBlockParticleSize, stunBlockParticleSize);
            }
        }
        void Blocked()
        {
            foreach (Knockback knockback in info.knockbacks)
            {
                knockbackHandler.GetKnockbacked(knockback, fullInfo.direction);
            }
            returnInfo.blocked = true;
            returnInfo.shakeMult = guardingCameraShakeMult;

            playerFunc.GetBlockStuned(info.blockStun);

            if (playerFunc.isPerfBlocking)
            {
                returnInfo.doReturnKnockback = true;
                returnInfo.returnDisadvantage = playerFunc.perfBlockDisadvantageReturn;

                returnInfo.doReturnAttackStun = true;
                returnInfo.returnAttackStun = playerFunc.perfBlockAttackStun;
            }
            if (playerFunc.isBlockCounterSuccess)
            {
                returnInfo.returnDisadvantage = playerFunc.blockCounterDisadvantageReturn;

                returnInfo.doReturnAttackStun = true;
                returnInfo.returnAttackStun = playerFunc.blockCounterAttackStun;

                returnInfo.doReturnKnockback = true;
                returnInfo.returnKnockback = playerFunc.blockCounterReturnKnockback;

                returnInfo.doHitStopOverride = true;
                returnInfo.hitStopOverride = playerFunc.blockCounterHitStop;
            }

            playerFunc.OnBlock(info.advantageDamage);
        }
    }

    public void DropThroughPlatform()
    {
        dropThroughPlatformTimer = dropThroughPlatformTime;
        platformBox.enabled = false;
    }

    public TeamHolder team {get{return playerStatus.team;} set{}}
}
