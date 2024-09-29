using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour, IDamageable
{
    public Rigidbody2D rb;
    public PlayerStatus playerStatus;
    public PlayerFunc playerFunc;
    public GroundChecker groundCheck;

    public BoxCollider2D hitbox;
    public BoxCollider2D boundingBox;

    Vector2 normalHitboxSize;
    Vector2 normalHitboxOffset;
    Vector2 normalBoundingSize;
    Vector2 normalBoundingOffset;
    public BoxCollider2D hitboxSizeReference;

    [SerializeField] float groundDistance;

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

    void FixedUpdate()
    {
        if (groundCheck.grounded)
        {
            //if we in the ground, shift us up
            float groundHeight = groundCheck.groundHeight;
            float boundingBottomHeight = boundingBox.transform.position.y + boundingBox.offset.y - boundingBox.size.y/2; //y pos of bottom fo bounding box
            float groundHeightDelta = boundingBottomHeight - groundHeight;
            if (groundHeightDelta < 0) //if our boundign box goes below the ground, move us up
            {
                rb.transform.position = new Vector3(rb.transform.position.x, rb.transform.position.y - groundHeightDelta, rb.transform.position.z);
            }
        }
    }

    public bool GetHit(AttackInfo info)
    {
        if (!playerFunc.isDodgeIntangible)
        {
            playerStatus.GetHit(info);
            return true;
        }
        else
        {
            return false;
        }
    }

    public TeamHolder team {get{return playerStatus.team;} set{}}
}
