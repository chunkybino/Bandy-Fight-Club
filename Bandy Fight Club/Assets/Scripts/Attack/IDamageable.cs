using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    TeamHolder team {get; set;}

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

    public float hitStopTime = 0;
    public float cameraShake = 0;

    public AttackInfo dataCopy {get{return CloneYourself();}}

    public AttackInfo CloneYourself()
    {
        return new AttackInfo()
        {
            damage = damage,
            knockbacks = knockbacks,

            blockStun = blockStun,
            stunTime = stunTime,

            hitStopTime = hitStopTime,
            cameraShake = cameraShake
        };
    }
}
[System.Serializable]
public class FullAttackInfo
{
    public AttackInfo info;
    public Vector2 direction;
}
