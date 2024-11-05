using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] TeamHolder team;

    [SerializeField] float rehitTime = 0.5f;
    List<Collider2D> rehitList = new List<Collider2D>();
    IEnumerator rehitCorountine;

    [SerializeField] AttackInfo attackInfo;

    [SerializeField] int direction;
    [SerializeField] Transform directionReference;

    [SerializeField] PlayerFunc playerFunc;
    [SerializeField] KnockbackHandler knockbackHandler;

    void OnTriggerStay2D(Collider2D item)
    {
        IDamageable damageable = item.GetComponent<IDamageable>();

        if (damageable == null) return; //if we find a hitbox
        if (rehitList.Contains(item)) return; //avoid rehits

        if (!damageable.receiveHitFromAll)
        {
            if (team) if (team.team == damageable.team.team) return; //if we on the same team, if this hitbox isnt assigned to a team, hit everyone
        }

        direction = UFunc.Sign(directionReference.localScale.x);

        FullAttackInfo fullInfo = new FullAttackInfo() {info = attackInfo, direction = new Vector2(direction,1), team = team.team};
        AttackHitReturnInfo hitInfo = damageable.GetHit(fullInfo);

        if (hitInfo.hit) 
        {
            HitDude(hitInfo);

            rehitCorountine = removeFromHitListCorountine(item);
            StartCoroutine(rehitCorountine);
        }
    }

    void HitDude(AttackHitReturnInfo info)
    {
        float hitStop = attackInfo.hitStopTime;
        if (info.doHitStopOverride) hitStop = info.hitStopOverride;
        if (attackInfo.hitStopTime > 0) GameManager.DoHitStopStatic(hitStop);
        if (attackInfo.cameraShake > 0) GameManager.DoCameraShakeStatic(attackInfo.cameraShake * info.shakeMult);

        if (info.doReturnKnockback && knockbackHandler != null)
        {
            knockbackHandler.GetKnockbacked(info.returnKnockback, -direction);
        }
        if (playerFunc != null)
        {
            playerFunc.AddDisadvantage(info.returnDisadvantage);
            if (info.doReturnAttackStun) playerFunc.playerAttack.DoAttackStun(info.returnAttackStun);
        }
    }

    IEnumerator removeFromHitListCorountine(Collider2D target)
    {
        rehitList.Add(target);
        yield return new WaitForSeconds(rehitTime);
        rehitList.Remove(target);
    }
}

public class AttackHitReturnInfo
{
    public bool hit = false;
    public bool blocked = false;
    public bool perfBlocked = false;

    public bool doReturnKnockback;
    public Knockback returnKnockback;

    public float returnDisadvantage;

    public bool doReturnAttackStun;
    public float returnAttackStun;

    public bool doHitStopOverride;
    public float hitStopOverride;

    public float shakeMult = 1;
}
