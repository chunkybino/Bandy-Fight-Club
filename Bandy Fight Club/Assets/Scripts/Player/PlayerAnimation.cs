using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator anim;

    public PlayerAttack playerAttack;
    public PlayerFunc playerFunc;
    public Rigidbody2D rb;

    public void PlayAnim(string name)
    {
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
