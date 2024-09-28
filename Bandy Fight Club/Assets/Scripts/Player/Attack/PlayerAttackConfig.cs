using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackConfig : MonoBehaviour
{
    public string name {get{return gameObject.name;}}

    public string animationName;

    public bool cancelOnGroundLand;

    public float speedMult = 0;
}
