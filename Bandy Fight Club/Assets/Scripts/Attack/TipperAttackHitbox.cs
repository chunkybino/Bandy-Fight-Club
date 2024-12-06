using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipperAttackHitbox : AttackHitbox
{
    public AttackInfo tipAttackInfo;

    [SerializeField] Transform[] tipPoints;

    [SerializeField] float tipSoundVolMult = 1.25f;
    [SerializeField] float tipSoundPitchMult = 1.4f;

    bool tip;

    AttackInfo trueAttackInfo;

    public override void HitVaild(Collider2D item, IDamageable damageable)
    {
        tip = false;
        trueAttackInfo = attackInfo;
        foreach (Transform t in tipPoints)
        {
            if (item.OverlapPoint(t.position))
            {
                tip = true;
                trueAttackInfo = tipAttackInfo;
                break;
            }
        }

        FullAttackInfo fullInfo = new FullAttackInfo() {info = trueAttackInfo, direction = new Vector2(direction,1), team = team};
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
        float hitStop = trueAttackInfo.hitStopTime;
        if (info.doHitStopOverride) hitStop = info.hitStopOverride;
        if (trueAttackInfo.hitStopTime > 0) GameManager.DoHitStopStatic(hitStop);
        if (trueAttackInfo.cameraShake > 0) GameManager.DoCameraShakeStatic(trueAttackInfo.cameraShake * info.shakeMult);

        if (info.doReturnKnockback && knockbackHandler != null)
        {
            knockbackHandler.GetKnockbacked(info.returnKnockback, -direction);
        }
        if (playerFunc != null)
        {
            playerFunc.AddDisadvantage(info.returnDisadvantage);
            if (info.doReturnAttackStun) playerFunc.playerAttack.DoAttackStun(info.returnAttackStun);
        }

        if (audio) 
        {
            float vMult = 1;
            float pMult = 1;
            if (tip)
            {
                vMult = tipSoundVolMult;
                pMult = tipSoundPitchMult;
            }
            audio.Play(hitSoundName, hitSoundVolume*vMult, hitSoundPitch*pMult);
        }
    }
}
