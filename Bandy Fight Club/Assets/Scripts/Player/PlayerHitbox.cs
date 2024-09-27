using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour, IDamageable
{
    public PlayerStatus playerStatus;

    public BoxCollider2D hitbox;
    public BoxCollider2D boundingBox;

    Vector2 normalHitboxSize;
    Vector2 normalHitboxOffset;
    Vector2 normalBoundingSize;
    Vector2 normalBoundingOffset;
    public BoxCollider2D hitboxSizeReference;

    void Awake()
    {
        normalHitboxSize = hitbox.size;
        normalHitboxOffset = hitbox.offset;
        normalBoundingSize = boundingBox.size;
        normalBoundingOffset = boundingBox.offset;
    }

    void Update()
    {
        if (hitboxSizeReference.gameObject.activeSelf)
        {
            hitbox.size = hitboxSizeReference.size;
            hitbox.offset = hitboxSizeReference.offset;
            boundingBox.size = hitboxSizeReference.size;
            boundingBox.offset = hitboxSizeReference.offset;
        }
        else
        {
            hitbox.size = normalHitboxSize;
            hitbox.offset = normalHitboxOffset;
            boundingBox.size = normalBoundingSize;
            boundingBox.offset = normalBoundingOffset;
        }
    }

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
