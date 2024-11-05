using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BulletFunc : MonoBehaviour, IDamageable
{
    public BulletConfig config;
    public Rigidbody2D rb;
    //[SerializeField] Collider2D collider;

    public CircleCollider2D circleCollider;
    [SerializeField] Collider2D bounding;

    [SerializeField] LayerMask layerMask;

    [SerializeField] TeamHolder bulletTeam;
    public TeamHolder team {get{return bulletTeam;} set{}}
    public bool receiveHitFromAll {get {return true;}}

    //bool hurtPlayer;
    //bool hurtEnemy;
    public AttackInfo attackInfo;

    //public bool doAttackDirectionOverride;
    //public Vector2 attackDirectionOverride;

    public SpriteRenderer renderer;
    [SerializeField] bool rotateInMovementDirection;
    [SerializeField] float speedPerStrech = 30; //strech render in direction more the faster we go, speed needed to stech render scale by 1
    [SerializeField] Vector2 renderScale;
    /*[SerializeField] TrailRenderer trailRenderer;
    [SerializeField] float trailWidth = 0.4f;
    [SerializeField] ParticleSystem hitParticles;
    [SerializeField] float hitParticlesScale = 0.6f;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] float deathParticlesScale = 0.6f;*/

    public Vector2 direction = new Vector2(1,0);
    public float speed = 8;

    public bool doGrav = false;
    public float gravity = 0;

    public int currentPierce;

    public bool dieOnWall;

    public bool deflectable = true;
    public float deflectSpeedAdd = 8;
    [SerializeField] float deflectCooldown = 0.1f;
    [SerializeField] bool swapTeamOnDeflect = true;
    [SerializeField] bool changeColorOnDeflectTeamSwap = true;

    public bool disableHitbox;

    //public UnityEvent<EnemyHitbox> onEnemyHit;

    [SerializeField] List<IDamageable> hitTargetsList = new List<IDamageable>();
    [Tooltip("basivacally rehit time but for the player")]
    [SerializeField] float reHitTime = 0.5f;
    IEnumerator hitListCoroutine;

    /*bool isDead;

    public UnityEvent onDeflect;
    bool gotDeflectedThisHit; //when detecting hits, when we get deflected, set this to true so it doesnt ake bullet pierce

    [SerializeField] float deflectHitStop = 0.15f;

    [SerializeField] AttackTelegraph telegraph;*/

    Vector2 lastFramPos;

    public SpriteRenderer outlineRender;
    public SpriteRenderer disguiseOutlineRender;

    public void Start()
    {
        speed = config.speed;
        currentPierce = config.pierce;
        transform.localScale = new Vector3(config.size,config.size,1);
        if (renderer) renderScale = renderer.transform.localScale;
        DoLifetime(config.lifetime);

        lastFramPos = transform.position;

        /*if (trailRenderer) trailRenderer.widthMultiplier = trailWidth;

        hurtPlayer = config.hurtPlayer;
        hurtEnemy = config.hurtEnemy;

        if (telegraph)
        {
            telegraph.prediction = config.prediction;
            telegraph.weight = config.predictionWeight;

            telegraph.FindStuffInRange();
        }*/

        CheckCollisionNow();

        rb.velocity = direction.normalized * speed;

        DoTeamColor();
    }
    void DoTeamColor()
    {
        if (GameManager.singleton.teamOutlineColors.materials.Length > team.team) 
        {
            if (outlineRender) outlineRender.material = GameManager.singleton.teamOutlineColors.materials[team.team];
            if (disguiseOutlineRender) disguiseOutlineRender.material = GameManager.singleton.teamOutlineColors.materials[team.team];
        }
    }

    public void Update()
    {
        if (renderer && rotateInMovementDirection)
        {
            renderer.transform.eulerAngles = new Vector3(0,0,UFunc.VecToAngle(rb.velocity));
            renderer.transform.localScale = new Vector2(1 + rb.velocity.magnitude/speedPerStrech, 1) * renderScale;
        }
    }

    public void FixedUpdate()
    {
        if (!doGrav)
        {
            //rb.velocity = direction.normalized * speed;
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - gravity*Time.fixedDeltaTime);
        }

        StartCoroutine("LateFixedUpdate");
    }

    IEnumerator LateFixedUpdate()
    {
        yield return new WaitForFixedUpdate();

        CheckRaycastCollision();
        lastFramPos = transform.position;
    }

    void OnTriggerStay2D(Collider2D item)
    {
        CheckCollision(item);
    }
    void CheckCollisionNow()
    {
        if (circleCollider)
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, circleCollider.radius, new Vector2(1,0), 0, layerMask);

            foreach (RaycastHit2D hit in hits)
            {
                CheckCollision(hit.collider);
            }
        }
    } 
    void CheckRaycastCollision()
    {
        if (circleCollider)
        {
            Vector2 castDirection = new Vector2(transform.position.x, transform.position.y) - lastFramPos;

            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, circleCollider.radius, castDirection, castDirection.magnitude, layerMask);

            foreach (RaycastHit2D hit in hits)
            {
                CheckCollision(hit.collider);
            }
        }
    }
    void CheckCollision(Collider2D item)
    {
        bool foundHit = false;
        bool hitWall = false;

        if (item.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            foundHit = true;
            hitWall = true;
        }

        IDamageable damageable = item.GetComponent<IDamageable>();
        TryDamageable(damageable);

        if (foundHit)
        {
            /*if (hitParticles)
            {
                ParticleSystem particle = Instantiate(hitParticles, transform.position, Quaternion.identity);
                particle.transform.localScale = new Vector2(hitParticlesScale,hitParticlesScale);
            }*/

            if (hitWall)
            {
                HitWall(item);
            }
        }

        void TryDamageable(IDamageable dam)
        {
            if (dam == null) return;
            if (hitTargetsList.Contains(dam)) return;
            if (team) if (team.team == damageable.team.team) return; //if we on the same team, if this hitbox isnt assigned to a team, hit everyone
            
            foundHit = true;
            HitDude(dam);
        }
    }

    IEnumerator AddToHitList(IDamageable targetToAdd)
    {
        hitTargetsList.Add(targetToAdd);
        yield return new WaitForSeconds(reHitTime);
        hitTargetsList.Remove(targetToAdd);
    }
    public void ClearHitList()
    {
        hitTargetsList.Clear();
    }

    public virtual void HitDude(IDamageable damageable)
    {
        Vector2 attackDirection = direction;
        //if (doAttackDirectionOverride) attackDirection = attackDirectionOverride;

        AttackHitReturnInfo hitInfo = damageable.GetHit(attackInfo, direction);

        DoHitStopCameraShake(hitInfo);

        if (currentPierce == 0) Die();
        currentPierce -= 1;

        hitListCoroutine = AddToHitList(damageable);
        StartCoroutine(hitListCoroutine);
    }

    public void DoHitStopCameraShake(AttackHitReturnInfo info)
    {
        if (attackInfo.hitStopTime > 0) GameManager.DoHitStopStatic(attackInfo.hitStopTime);
        if (attackInfo.cameraShake > 0) GameManager.DoCameraShakeStatic(attackInfo.cameraShake * info.shakeMult);
    }

    public virtual void HitWall(Collider2D wall)
    {
        if (dieOnWall) Die();
    }


    public void Die()
    {
        Destroy(gameObject); //give some time for the trail to go away
    }

    void OnDestroy()
    {
        BulletHandler.singleton.RemoveBulletFromList(this);
    }

    public AttackHitReturnInfo GetHit(FullAttackInfo info)
    {
        if (!deflectable || !info.info.doDeflect) return new AttackHitReturnInfo();

        GetDeflected(info.info.deflectForce * info.direction);
        if (swapTeamOnDeflect) {
            team.team = info.team;
            if (changeColorOnDeflectTeamSwap) DoTeamColor();
        }

        return new AttackHitReturnInfo();
    }
    public void GetDeflected(Vector2 deflectForce)
    {
        DoDeflectSpeed(deflectForce);

        DoLifetime(config.lifetime);

        deflectable = false;
        Invoke("EnableDeflectable", deflectCooldown);

        circleCollider.enabled = false;
        Invoke("EnableHitbox", deflectCooldown);
    }
    public virtual void DoDeflectSpeed(Vector2 dir) //for override, if we want to override just hte speed part
    {
        direction = new Vector2(dir.x, -direction.y);
        speed += deflectSpeedAdd;
        rb.velocity = direction.normalized * speed;
    }
    void EnableDeflectable()
    {
        deflectable = true;
    }

    void EnableHitbox()
    {
        circleCollider.enabled = true;
    }

    public void SetTeam(int num)
    {
        team.team = num;
        DoTeamColor();
    }

    /*public void ChangeDamage(float amount)
    {
        attackInfo.damage += amount;
    }
    public void ChangeGuardDamage(float amount)
    {
        attackInfo.guardDamage += amount;
    }
    public void ChangePierce(int amount)
    {
        currentPierce += amount;
    }*/

    public void DoLifetime(float time)
    {
        CancelInvoke("Die");
        Invoke("Die", time);
    }
}
