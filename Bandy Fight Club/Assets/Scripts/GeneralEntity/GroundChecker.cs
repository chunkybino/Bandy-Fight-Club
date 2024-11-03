using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public bool grounded;
    public bool groundGrounded;
    public bool platformGrounded;
    public BoxCollider2D colliderReference; //reference this collider when checkign for grounded
    public Vector2 groundCheckPos;
    public Vector2 groundedCheckSize = new Vector2 (1, 0.05f);
    public float checkSizeXMargin = 0.05f; //add a bit of a margin on the sides so we dont think we're grouinded when rubbing against a wall
    public float groundCheckDownRange = 0.05f;
    public LayerMask layerMask = 6;
    public LayerMask platformLayerMask;
    public float groundHeight;

    public bool dontAutoGroundCheck; //only check for ground when prompted

    public List<Collider2D> ignoreColliders = new List<Collider2D>();

    void FixedUpdate()
    {
        if (dontAutoGroundCheck) return;
        DoGroundCheckNow();
    }

    public void DoGroundCheckNow(bool onlyPlatformGroundAbove = true)
    {
        if (colliderReference)
        {
            groundedCheckSize = new Vector2(colliderReference.size.x, colliderReference.size.y);//groundedCheckSize.y);
            groundCheckPos = new Vector2(colliderReference.transform.position.x + colliderReference.offset.x, colliderReference.transform.position.y + colliderReference.offset.y);// - colliderReference.size.y / 2);
        }

        grounded = CheckGrounded(groundCheckPos, groundedCheckSize, layerMask);

        RaycastHit2D[] raycastHits = Physics2D.BoxCastAll(new Vector2(groundCheckPos.x, groundCheckPos.y - (groundCheckDownRange/2)), new Vector2(groundedCheckSize.x-checkSizeXMargin, groundCheckDownRange), 0, Vector2.down, 50, layerMask);
        
        grounded = false;
        groundGrounded = false;
        platformGrounded = false;
        foreach (RaycastHit2D hit in raycastHits)
        {
            if (ignoreColliders.Contains(hit.collider)) continue; //make sure its note one of our ignore colliders

            if (!grounded) //set ground height to the first ground we find
            {
                groundHeight = hit.point.y;
            }

            if (hit.point.y < groundCheckPos.y - (groundCheckDownRange/2) - (groundedCheckSize.y/2)) continue; //if the ground is too far

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground") || hit.collider.gameObject.tag == "Ground") //if its a ground ground
            {
                groundGrounded = true;
                grounded = true;
            }
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("PlatformGround")) //if its a platform
            {
                float colliderBottomHeight = groundCheckPos.y - (groundedCheckSize.y/2);
                if (colliderBottomHeight > groundHeight - 0.025f || !onlyPlatformGroundAbove) //only do if we arnt below the platform, if the variable is true
                {
                    platformGrounded = true;
                    grounded = true;
                }
            }
        }
    }

    public static bool CheckGrounded(Vector2 pos, Vector2 checkSize, LayerMask layermask)
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(new Vector2(pos.x, pos.y - checkSize.y / 2), checkSize, 0, Vector2.down, 0, layermask);

        return (raycastHit.collider != null);
    }
}
