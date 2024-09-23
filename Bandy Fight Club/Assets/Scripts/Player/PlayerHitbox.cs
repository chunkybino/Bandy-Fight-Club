using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour, IDamageable
{
    public PlayerStatus playerStatus;

    public void GetHit(AttackInfo info)
    {
        print(info.damage);
        playerStatus.GetHit(info);
    }

    public TeamHolder team {
        get{return playerStatus.team;} 
        set{}
    }
}
