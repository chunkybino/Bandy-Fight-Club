using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JCloneFunc : MonoBehaviour
{
    public BigJFunc parentJ;

    public Vector2 direction;

    public int team;

    public int facing = 1;

    public Rigidbody2D rb;
    [SerializeField] PlayerAnimation playerAnimation;
    [SerializeField] PlayerMeshParent meshParent;
    [SerializeField] Transform textureTransform;
    [SerializeField] SquashStretcher squasher;

    public Vector2 forceVector;
    public float forceMagnitude;
    public float attackForce = 24f;

    [SerializeField] float forceDecaySpeed = 1.5f;
    [SerializeField] AnimationCurve forceDecayCurve;
    [SerializeField] float attackForceTimer;

    [SerializeField] float attackBoostDelay = 0.25f;

    [SerializeField] float deactivateTime = 1;

    public bool active;
    public float timeActive;

    [SerializeField] AttackHitbox cloneAttackHitbox;

    public bool hitTarget;

    [SerializeField] ParticleSystem attackParticles;

    [SerializeField] float teleportShaderTime = 0.05f;
    [SerializeField] float teleportShaderStrength = 15f;
    [SerializeField] float teleportShaderTimer = 1;
    [SerializeField] AnimationCurve teleportShaderCurve;
    [SerializeField] AnimationCurve teleportXScaleCurve;

    bool doUnTeleportShader;

    [SerializeField] SquashData teleportSquash;

    [SerializeField] MeshRenderer meshRenderer;

    void Start()
    {
        cloneAttackHitbox.onHit.AddListener(OnAttackHit);
    }

    void Update()
    {
        if (active) timeActive += Time.deltaTime;

        attackForceTimer += forceDecaySpeed*Time.deltaTime;

        rb.velocity = forceVector;

        forceMagnitude = attackForce * forceDecayCurve.Evaluate(attackForceTimer);
        forceVector = forceMagnitude * direction;

        if (!doUnTeleportShader && timeActive >= deactivateTime-teleportShaderTime)
        {
            teleportShaderTimer = 0;
            doUnTeleportShader = true;
        }

        if (teleportShaderTimer <= 1)
        {
            teleportShaderTimer += Time.deltaTime/teleportShaderTime;

            float progress = teleportShaderTimer;
            if (doUnTeleportShader) progress = 1-progress;

            float str = Mathf.Lerp(0, teleportShaderStrength, teleportShaderCurve.Evaluate(progress));

            MaterialPropertyBlock matBlock = new MaterialPropertyBlock();
            matBlock.SetTexture("_MainTex", meshParent.renderTex);
            matBlock.SetFloat("_Strength", teleportShaderStrength-str);
            meshRenderer.SetPropertyBlock(matBlock, 1);

            textureTransform.localScale = new Vector3(facing*teleportXScaleCurve.Evaluate(progress),1,1);
        }
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
        timeActive = 0;

        hitTarget = false;

        teleportShaderTimer = 0;
        doUnTeleportShader = false;
        squasher.DoSquash(teleportSquash);

        DoAttack();

        attackParticles.Play();

        CancelInvoke("Deactivate");
        Invoke("Deactivate", deactivateTime);
    }
    public void Deactivate()
    {
        forceVector = new Vector2(0,0);

        hitTarget = false;

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

    public void OnAttackHit(AttackHitReturnInfo info)
    {
        hitTarget = true;
        if (parentJ != null)
        {
            parentJ.AddDisadvantage(info.returnDisadvantage);
        }
    }
}
