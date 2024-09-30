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
    public BoxCollider2D entityBounding;
    public BoxCollider2D platformBox;

    Vector2 normalHitboxSize;
    Vector2 normalHitboxOffset;
    Vector2 normalBoundingSize;
    Vector2 normalBoundingOffset;
    public BoxCollider2D hitboxSizeReference;

    public bool isIntangible;

    bool boundBoxSizeChanged = false;

    void Awake()
    {
        normalHitboxSize = hitbox.size;
        normalHitboxOffset = hitbox.offset;
        normalBoundingSize = boundingBox.size;
        normalBoundingOffset = boundingBox.offset;

        Physics2D.IgnoreCollision(boundingBox, entityBounding);
        Physics2D.IgnoreCollision(platformBox, entityBounding);
    }

    void Update()
    {
        float startGroundHeight = groundCheck.groundHeight;
        float startGroundDelta = groundCheck.groundHeight - platformBox.transform.position.y - platformBox.offset.y;

        Vector2 hitSize = normalHitboxSize;
        Vector2 hitOffset = normalHitboxOffset;
        Vector2 boundSize = normalBoundingSize;
        Vector2 boundOffset = normalBoundingOffset;

        if (hitboxSizeReference.gameObject.activeSelf)
        {
            hitSize = hitboxSizeReference.size;
            hitOffset = hitboxSizeReference.offset;

            boundSize = hitboxSizeReference.size;
            boundOffset = hitboxSizeReference.offset;
        }

        hitbox.size = hitSize;
        hitbox.offset = hitOffset;
        boundingBox.size = boundSize;
        boundingBox.offset = boundOffset;

        //entity bounding
        entityBounding.size = boundingBox.size;
        entityBounding.offset = boundingBox.offset;

        //platform bounding
        float platformBoxHeight = boundingBox.offset.y + ((platformBox.size.y-boundingBox.size.y)/2);
        float platformHeightChange = platformBoxHeight - platformBox.transform.localPosition.y;
        platformBox.size = new Vector2(boundingBox.size.x, platformBox.size.y);
        platformBox.transform.localPosition = new Vector2(boundingBox.offset.x, platformBoxHeight); //move platform boudning to bottom of normal bbounding

        //shiftup
        if (platformHeightChange < -0.1f) //if we get moved down
        {
            groundCheck.DoGroundCheckNow(false);
            float groundDelta = startGroundHeight - platformBox.transform.position.y - platformBox.offset.y;
            //print(groundDelta);
            if (groundDelta > 0 && startGroundDelta < 0)
            {
                CheckForGroundShiftUp();
            }
        }

        //intagibility
        isIntangible = playerFunc.isDodgeIntangible;
        entityBounding.enabled = !isIntangible; //disable entity bounding if intagible
    }

    void CheckForGroundShiftUp() //if we in the ground, shift us up
    {
        float groundHeight = groundCheck.groundHeight;
        float boundingBottomHeight = boundingBox.transform.position.y + boundingBox.offset.y - (boundingBox.size.y/2); //y pos of bottom fo bounding box
        float groundHeightDelta = boundingBottomHeight - groundHeight;
        if (groundHeightDelta < 0) //if our boundign box goes below the ground, move us up
        {
            rb.transform.position = new Vector3(rb.transform.position.x, rb.transform.position.y - groundHeightDelta, rb.transform.position.z);
        }
    }

    public bool GetHit(AttackInfo info)
    {
        if (!isIntangible)
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
