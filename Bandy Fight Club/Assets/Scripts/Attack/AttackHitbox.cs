using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] TeamHolder teamHolder;
    public int team;

    public float rehitTime = 0.5f;
    List<Collider2D> rehitList = new List<Collider2D>();
    public IEnumerator rehitCorountine;

    public AttackInfo attackInfo;

    public int direction;
    [SerializeField] Transform directionReference;

    public PlayerFunc playerFunc;
    public KnockbackHandler knockbackHandler;

    public UnityEvent<AttackHitReturnInfo> onHit;

    public AudioManager audio;
    public string hitSoundName = "hit";
    public float hitSoundVolume = 1;
    public float hitSoundPitch = 1;

    void OnTriggerStay2D(Collider2D item)
    {
        IDamageable damageable = item.GetComponent<IDamageable>();

        if (damageable == null) return; //if we find a hitbox
        if (rehitList.Contains(item)) return; //avoid rehits

        if (teamHolder) team = teamHolder.team;

        if (!damageable.receiveHitFromAll)
        {
            if (team == damageable.team.team) return; //if we on the same team, if this hitbox isnt assigned to a team, hit everyone
        }

        direction = UFunc.Sign(directionReference.localScale.x);

        HitVaild(item, damageable);
    }

    public virtual void HitVaild(Collider2D item, IDamageable damageable)
    {
        FullAttackInfo fullInfo = new FullAttackInfo() {info = attackInfo, direction = new Vector2(direction,1), team = team};
        AttackHitReturnInfo hitInfo = damageable.GetHit(fullInfo);

        if (hitInfo.hit) 
        {
            HitDude(hitInfo);

            onHit?.Invoke(hitInfo);

            rehitCorountine = removeFromHitListCorountine(item);
            StartCoroutine(rehitCorountine);
        }
    }

    public virtual void HitDude(AttackHitReturnInfo info)
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

        if (audio) audio.Play(hitSoundName, hitSoundVolume, hitSoundPitch);
    }

    public IEnumerator removeFromHitListCorountine(Collider2D target)
    {
        rehitList.Add(target);
        yield return new WaitForSeconds(rehitTime);
        rehitList.Remove(target);
    }

    public void ClearHitList()
    {
        rehitList.Clear();
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
