using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    TeamHolder team {get; set;}

    bool GetHit(FullAttackInfo info);
}

[System.Serializable]
public class AttackInfo
{
    public int damage;
    public Knockback[] knockbacks;

    public float hitStopTime = 0;
    public float cameraShake = 0;
}
[System.Serializable]
public class FullAttackInfo
{
    public AttackInfo info;
    public Vector2 direction;
}
