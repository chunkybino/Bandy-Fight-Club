using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    TeamHolder team {get; set;}

    bool receiveHitFromAll {get {return false;}}

    List<string> damageableTags {get {return new List<string>();}}

    AttackHitReturnInfo GetHit(FullAttackInfo info);

    AttackHitReturnInfo GetHit(AttackInfo info, Vector2 direction)
    {
        FullAttackInfo fullInfo = new FullAttackInfo() {info = info, direction = direction};
        return GetHit(fullInfo);
    }
}

[System.Serializable]
public class AttackInfo
{
    public int damage;
    public Knockback[] knockbacks;

    public float blockStun = 0.4f;
    public float stunTime;

    public float advantageDamage = 0;

    public bool onlyPerfBlock = false;
    public bool unblockable = false;

    public float hitStopTime = 0;
    public float cameraShake = 0;

    public bool doDeflect = false;
    public Vector2 deflectForce;

    public List<string> tags;

    public AttackInfo dataCopy {get{return CloneYourself();}}

    public AttackInfo CloneYourself()
    {
        return new AttackInfo()
        {
            damage = damage,
            knockbacks = CloneKnockback(),

            blockStun = blockStun,
            stunTime = stunTime,

            advantageDamage = advantageDamage,

            onlyPerfBlock = onlyPerfBlock,
            unblockable = unblockable,

            hitStopTime = hitStopTime,
            cameraShake = cameraShake,

            doDeflect = doDeflect,
            deflectForce = deflectForce,

            tags = tags
        };

        Knockback[] CloneKnockback()
        {
            Knockback[] k = new Knockback[knockbacks.Length];
            for (int i = 0; i < knockbacks.Length; i++) {k[i] = knockbacks[i];}
            return k;
        }
    }
}
[System.Serializable]
public class FullAttackInfo
{
    public AttackInfo info;
    public Vector2 direction;
    public int team;
}
