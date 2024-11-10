using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAnimation : MonoBehaviour
{
    public Animator anim;

    public PlayerStatus playerStatus;
    public PlayerAttack playerAttack;
    public PlayerFunc playerFunc;
    public ICharacterFunc characterFunc;
    public Rigidbody2D rb;

    public string currentAnim = "Idle";

    public AnimatorEvent[] animatorEvents;

    [SerializeField] SquashStretcher squasher;
    [SerializeField] SquashData[] squashDatas;

    [SerializeField] int blockAnimIndex = 0;
    [SerializeField] string[] blockAnimNames = new string[] {"Block"};

    float timeSinceWalk;
    [SerializeField] float skidTime = 0.1f;
    float walkDashTimer;
    [SerializeField] float walkDashTime = 0.1f;
    bool walking;

    float riseToFallTimer;
    [SerializeField] float riseToFallTime = 0.1f;
    bool riseToFall;

    float timeDead;
    [SerializeField] float groundDeathAnimDelay = 0.1f; //dont do the deaath anim for this much time

    [SerializeField] ParticleSystem walkParticles;
    [SerializeField] ParticleSystem walkParticlesPrefab;

    [SerializeField] ParticleSystem landParticles;
    [SerializeField] ParticleSystem landParticlesPrefab;
    [SerializeField] ParticleSystem landStrongParticles;
    [SerializeField] ParticleSystem landStrongParticlesPrefab;
    [SerializeField] float landStrongVelocityThreshold = -10;

    [SerializeField] ParticleSystem jumpParticles;

    [SerializeField] ParticleSystem dashParticles;
    [SerializeField] ParticleSystem dashParticlesPrefab;

    public int facing;
    public bool grounded;
    public bool isStun;
    public bool isBlocking;
    public bool isDodging;
    public float targetSideSpeed;

    void Awake()
    {
        if (playerFunc) characterFunc = playerFunc.characterFunc;
    }

    void Update()
    {
        if (characterFunc != null) characterFunc = playerFunc.characterFunc;

        if (playerFunc)
        {
            facing = playerFunc.facing;
            grounded = playerFunc.grounded;
            isStun = playerFunc.isStun;
            isBlocking = playerFunc.isBlocking;
            isDodging = playerFunc.isDodging;
            targetSideSpeed = playerFunc.targetSideSpeed;
        }


        currentAnim = "BaseBlank";
        if (playerFunc) if (playerFunc.GetDoAnimation())
        {
            currentAnim = FindAnimation();

            PlayAnim(currentAnim);
        }
        PlayAnim(currentAnim);

        walkParticles.transform.eulerAngles = new Vector3(0,(facing*90)+90,0);
    }

    string FindAnimation()
    {
        if (grounded) riseToFallTimer = 0;

        if (characterFunc != null)
        {
            string characterAnim = characterFunc.FindAnimationName();
            if (characterAnim != "")
            {
                return characterAnim;
            }
        }

        string animName = "Idle";

        if (playerStatus)
        {
            if (playerStatus.isDead)
            {
                timeDead += Time.deltaTime;

                if (grounded && timeDead >= groundDeathAnimDelay)
                {
                    animName = "Dead";
                }
                else
                {
                    if (rb.velocity.y > 0.1f)
                    {
                        animName = "Stun";
                    }
                    else
                    {
                        animName = "DeadFall";
                    }
                }

                return animName;
            }
        }

        if (isStun && isBlocking) return "BlockStun";
        if (isStun) return "Stun";
        if (isBlocking) return blockAnimNames[blockAnimIndex];
        if (isDodging) {
            walking = true;
            return "Dash";
        }

        bool doWalk = false;

        if (!grounded)
        {
            if (rb.velocity.y > 0.1f)
            {
                animName = "AirRise";
                riseToFallTimer = 0;
                riseToFall = false;
            }
            else
            {
                UFunc.TickTimer(ref riseToFallTimer);
                if (!riseToFall) 
                {
                    riseToFallTimer = riseToFallTime;
                    riseToFall = true;
                }

                if (riseToFallTimer == 0)
                {
                    animName = "AirDown";
                }
                else
                {
                    animName = "AirRiseToFall";
                }
            }
        }
        else
        {
            timeSinceWalk += Time.deltaTime;
            if (targetSideSpeed != 0) 
            {
                if (!walking) walkDashTimer = 0;
                doWalk = true;

                if (walkDashTimer < walkDashTime)
                {
                    animName = "WalkDash";
                    walkDashTimer += Time.deltaTime;
                }
                else
                {
                    animName = "Walk";
                }
                timeSinceWalk = 0;
            }
            else if (timeSinceWalk < skidTime)
            {
                animName = "WalkSkid";
            }
        }

        walking = doWalk;

        return animName;
    }

    public void PlayAnim(string name)
    {
        if (name == "") return;

        anim.Play(name, -1);
    }

    public void ExitAnim()
    {  
        playerAttack?.ExitAttackAnim();
    }
    public void ExitActionAnim()
    {
        anim.Play("ActionBlank", -1);
        ExitAnim();
    }

    public void OnLand()
    {
        if (playerFunc.lowestRecentYVel <= landStrongVelocityThreshold)
        {
            DoParticles("strongLand");
        }
        else
        {
            DoParticles("land");
        }
    }

    public void DoKnockbackBoost(float force)
    {
        rb.velocity = new Vector2(force * facing, rb.velocity.y);
    }
    public void DoVerticalBoost(float force)
    {
        rb.velocity = new Vector2(rb.velocity.x, force);
    }

    public void DoAnimatorEvent(string name)
    {
        for (int i = 0; i < animatorEvents.Length; i++)
        {
            if (animatorEvents[i].Try(name)) break; //try name for all events, will return true if valid event is found, break after finding event
        }
    }

    public void DoNewBlockAnim()
    {
        blockAnimIndex++;
        if (blockAnimIndex >= blockAnimNames.Length) blockAnimIndex = 0;
    }

    public void DoSquash(int index)
    {
        if (index >= squashDatas.Length) return;

        squasher.DoSquash(squashDatas[index]);
    }

    public void DoParticles(string name) {DoParticles(name, false);}
    public void DoParticlesSpawn(string name) {DoParticles(name, true);}

    public void DoParticles(string name, bool spawn)
    {
        switch (name)
        {
            case "walk":
                if (spawn) {
                    Spawn(walkParticlesPrefab, walkParticles.transform.position);
                } else {
                    walkParticles.Play();
                }
                break;
            case "land":
                if (spawn) {
                    Spawn(landParticlesPrefab, landParticles.transform.position);
                } else {
                    landParticles.Play();
                }
                break;
            case "strongLand":
                if (spawn) {
                    Spawn(landStrongParticlesPrefab, landStrongParticles.transform.position);
                } else {
                    landStrongParticles.Play();
                }
                break;
            case "jump":
                jumpParticles.Play();
                break;
            case "dash":
                if (spawn) {
                    Spawn(dashParticlesPrefab, dashParticles.transform.position);
                } else {
                    dashParticles.Play();
                }
                break;

        }

        void Spawn(ParticleSystem prefab, Vector3 pos)
        {
            ParticleSystem particle = Instantiate(prefab, pos, Quaternion.identity);
            particle.transform.eulerAngles = new Vector3(0,(facing*90)+90,0);
        }
    }
}

[System.Serializable]
public class AnimatorEvent
{
    public string name;
    public UnityEvent theEvent;

    public bool Try(string tryName)
    {
        if (tryName == name)
        {
            theEvent?.Invoke();
            return true;
        }
        else
        {
            return false;
        }
    }
}
