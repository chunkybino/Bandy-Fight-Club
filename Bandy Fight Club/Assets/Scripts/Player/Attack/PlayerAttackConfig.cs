using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAttackConfig : MonoBehaviour
{
    public string attackName {get{return gameObject.name;}}

    public string animationName;

    public bool cancelOnGroundLand;

    public float speedMult = 0;

    public bool lockFacing = true;

    public UnityEvent onAttackStart;

    public float attackTime = 1;
}
