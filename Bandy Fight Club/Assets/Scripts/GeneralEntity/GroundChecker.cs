using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public bool grounded;
    public BoxCollider2D colliderReference; //reference this collider when checkign for grounded
    public Vector2 groundCheckPos;
    public Vector2 groundedCheckSize = new Vector2 (1, 0.05f);
    public LayerMask layerMask = 6;
    public float groundHeight;

    public bool dontAutoGroundCheck; //only check for ground when prompted

    void FixedUpdate()
    {
        if (dontAutoGroundCheck) return;
        DoGroundCheckNow();
    }

    public void DoGroundCheckNow()
    {
        if (colliderReference)
        {
            groundedCheckSize = new Vector2(colliderReference.size.x, groundedCheckSize.y);
            groundCheckPos = new Vector2(colliderReference.transform.position.x + colliderReference.offset.x, colliderReference.transform.position.y + colliderReference.offset.y - colliderReference.size.y / 2);
        }

        grounded = CheckGrounded(groundCheckPos, groundedCheckSize, layerMask);

        RaycastHit2D raycastHit = Physics2D.BoxCast(new Vector2(groundCheckPos.x, groundCheckPos.y - groundedCheckSize.y / 2), groundedCheckSize, 0, Vector2.down, 0, layerMask);
        if (raycastHit.collider != null)
        {
            grounded = true;
            groundHeight = raycastHit.point.y;
        }
        else
        {
            grounded = false;
        }
    }

    public static bool CheckGrounded(Vector2 pos, Vector2 checkSize, LayerMask layermask)
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(new Vector2(pos.x, pos.y - checkSize.y / 2), checkSize, 0, Vector2.down, 0, layermask);

        return (raycastHit.collider != null);
    }
}
