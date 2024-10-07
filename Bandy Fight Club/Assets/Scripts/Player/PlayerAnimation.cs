using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator anim;

    public PlayerAttack playerAttack;
    public PlayerFunc playerFunc;
    public Rigidbody2D rb;

    public string currentAnim = "Idle";

    void Update()
    {
        if (!playerAttack.isAttacking)
        {
            currentAnim = FindAnimation();

            PlayAnim(currentAnim);
        }
        else
        {
            PlayAnim("BaseBlank");
        }
    }

    string FindAnimation()
    {
        string animName = "Idle";

        if (!playerFunc.grounded)
        {
            if (rb.velocity.y > 0.1f)
            {
                animName = "AirRise";
            }
            else
            {
                animName = "AirDown";
            }
        }

        return animName;
    }

    public void PlayAnim(string name)
    {
        if (name == "") return;

        anim.Play(name, -1);
    }

    public void ExitAnim()
    {  
        playerAttack.ExitAttackAnim();
    }

    public void DoKnockbackBoost(float force)
    {
        rb.velocity = new Vector2(force * playerFunc.facing, rb.velocity.y);
    }
}
