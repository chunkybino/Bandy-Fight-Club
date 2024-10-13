using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAnimation : MonoBehaviour
{
    public Animator anim;

    public PlayerAttack playerAttack;
    public PlayerFunc playerFunc;
    public ICharacterFunc characterFunc;
    public Rigidbody2D rb;

    public string currentAnim = "Idle";

    public AnimatorEvent[] animatorEvents;

    void Awake()
    {
        characterFunc = playerFunc.characterFunc;
    }

    void Update()
    {
        if (characterFunc == null) characterFunc = playerFunc.characterFunc;

        if (playerFunc.GetDoAnimation())
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
        if (characterFunc != null)
        {
            string characterAnim = characterFunc.FindAnimationName();
            if (characterAnim != "")
            {
                return characterAnim;
            }
        }

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

    public void DoAnimatorEvent(string name)
    {
        for (int i = 0; i < animatorEvents.Length; i++)
        {
            if (animatorEvents[i].Try(name)) break; //try name for all events, will return true if valid event is found, break after finding event
        }
    }
}

[System.Serializable]
public class AnimatorEvent
{
    public string name;
    public UnityEvent theEvent;

    public bool Try(string tryName)
    {
        if (tryName == name)
        {
            theEvent?.Invoke();
            return true;
        }
        else
        {
            return false;
        }
    }
}
