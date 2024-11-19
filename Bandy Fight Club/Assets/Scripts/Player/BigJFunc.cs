using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigJFunc : PlayerFunc, ICharacterFunc
{
    [SerializeField] float timeSinceSlash1;
    [SerializeField] float slash2ComboTime = 1.2f;
    [SerializeField] float timeSinceSlash2;
    [SerializeField] float slash3ComboTime = 1.2f;

    public BulletConfig piccoloBulletConfig;

    [SerializeField] Vector2 piccoloSideThrowDirection = new Vector2(1f,1.2f);
    [SerializeField] Vector2 piccoloUpThrowDirection = new Vector2(0.2f,1.8f);
    [SerializeField] Vector2 piccoloDownThrowDirection = new Vector2(0.8f,-0.3f);

    [SerializeField] Vector2 piccoloSideThrowInheritanceMult = new Vector2(0.6f,0.6f);
    [SerializeField] Vector2 piccoloUpThrowInheritanceMult = new Vector2(0.15f,0.7f);
    [SerializeField] Vector2 piccoloDownThrowInheritanceMult = new Vector2(0.6f,0.8f);
    [SerializeField] float piccoloVerticalVelInheritenceDegree = 0.5f; //raise the yvel add to this degree

    public JPiccoloBulletFunc currentPiccoloBullet;
    public JCloneFunc currentClone;
    public JCloneFunc clonePrefab;

    [SerializeField] float cloneTeleportAccelMult = 0.15f;
    [SerializeField] float cloneTeleportAccelTime = 0.5f;
    [SerializeField] float cloneTeleportAttackStun = 0.2f;

    [SerializeField] float upDownAttackCooldown = 0.4f;
    float upDownAttackCooldownTimer;

    void Awake()
    {
        if (characterFunc == null) characterFunc = this;
    }
    void Start()
    {
        base.Start();

        SpawnClone();
    }
    void Update()
    {
        base.Update();

        if (characterFunc == null) characterFunc = this;

        timeSinceSlash1 += Time.deltaTime;
        timeSinceSlash2 += Time.deltaTime;

        if (!playerAttack.isAttacking)
        {
            UFunc.TickTimer(ref upDownAttackCooldownTimer);
        }

        if (currentPiccoloBullet)
        {
            if (currentPiccoloBullet.isActive && inputs.heavyPressed)
            {
                if (inputVector.x != 0 || inputVector.y != 0) //if we hold a direction
                {
                    DoPiccoloClone(inputVector);
                }
                else
                {
                    currentPiccoloBullet.DoJump();
                }
            }
        }
        else if (currentClone.active)
        {
            if (inputs.heavyPressed && GetIsActionable() && currentClone.hitTarget)
            {
                DoCloneTeleport();
            }
        }
    }

    public string FindAttackName()
    {
        string attackName = "nope";

        if (inputs.upHeld) //up attack
        {
            if (upDownAttackCooldownTimer == 0)
            {
                attackName = "upslash";
                upDownAttackCooldownTimer = upDownAttackCooldown;
            }
        }
        else if (inputs.downHeld) //down attack
        {
            if (upDownAttackCooldownTimer == 0)
            {
                attackName = "downslash";
                upDownAttackCooldownTimer = upDownAttackCooldown;
            }
        }
        else ///side attacks
        {
            if (timeSinceSlash2 <= slash3ComboTime)
            {
                attackName = "slash3";
                timeSinceSlash2 = 1;
            }
            else if (timeSinceSlash1 <= slash2ComboTime)
            {
                attackName = "slash2";
                timeSinceSlash2 = 0;
                timeSinceSlash1 = 1;
            }
            else
            {
                attackName = "slash1";
                timeSinceSlash1 = 0;
                timeSinceSlash2 = 1;
            }
        }

        return attackName;
    }
    public string FindAttackHeavyName()
    {
        string attackName = "PiccoloThrow";

        return attackName;
    }

    public void PiccoloThrowAttack()
    {
        if (!currentPiccoloBullet && !currentClone.active)
        {
            ShootPiccolo();
        }
        else
        {
            //DoPiccoloClone(new Vector2(1,0));
        }
    } 
    public void ShootPiccolo()
    {
        BulletFunc bullet = BulletHandler.ShootBulletStatic(piccoloBulletConfig, transform.position, new Vector2(facing, 1), team);
        JPiccoloBulletFunc piccolo = bullet.GetComponent<JPiccoloBulletFunc>();
        if (piccolo) currentPiccoloBullet = piccolo;

        Vector2 throwDirection = piccoloSideThrowDirection;
        Vector2 velAdd = piccoloSideThrowInheritanceMult;
        if (inputVector.y > 0) {
            throwDirection = piccoloUpThrowDirection;
            velAdd = piccoloUpThrowInheritanceMult;
        } else if (inputVector.y < 0) {
            throwDirection = piccoloDownThrowDirection;
            velAdd = piccoloDownThrowInheritanceMult;
        }
        velAdd *= rb.velocity;

        if (rb.velocity.y > 0) velAdd = new Vector2(velAdd.x, Mathf.Pow(velAdd.y, piccoloVerticalVelInheritenceDegree));

        currentPiccoloBullet.InheritVel(throwDirection, velAdd);
    }

    public void DoPiccoloClone(Vector2 direction)
    {
        if (!currentPiccoloBullet) return;

        currentClone.transform.position = currentPiccoloBullet.transform.position;
        currentClone.direction = direction.normalized;
        currentClone.facing = Mathf.FloorToInt(Mathf.Sign(currentPiccoloBullet.rb.velocity.x)); //face in direciton of piccolo by default
        currentClone.Activate();

        currentPiccoloBullet.Die();
    }

    void SpawnClone()
    {
        currentClone = Instantiate(clonePrefab);
        currentClone.parentJ = this;
        currentClone.SetTeam(team);
        //currentClone.Deactivate();
        currentClone.Spawn(playerMesh.num+4);
    }

    void DoCloneTeleport()
    {
        transform.position = currentClone.transform.position;
        rb.velocity = currentClone.rb.velocity;
        DoAccelerationMult(cloneTeleportAccelMult, cloneTeleportAccelTime);

        currentClone.Deactivate();

        playerAttack.attackStunTimer = cloneTeleportAttackStun;
    }

    void OnDestroy()
    {
        Destroy(currentClone.gameObject);
    }

    public void GetCloneHit(FullAttackInfo info)
    {
        playerHitbox.Hit(info);
    }
}

