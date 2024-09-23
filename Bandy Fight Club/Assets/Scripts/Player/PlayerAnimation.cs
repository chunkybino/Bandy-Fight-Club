using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator anim;

    public PlayerAttack playerAttack;

    public void PlayAnim(string name)
    {
        anim.Play(name, -1);
    }

    public void ExitAnim()
    {  
        playerAttack.ExitAttackAnim();
    }
}
