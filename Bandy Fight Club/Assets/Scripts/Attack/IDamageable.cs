using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    TeamHolder team {get; set;}

    bool GetHit(AttackInfo info);
}

[System.Serializable]
public class AttackInfo
{
    public int damage;
}
