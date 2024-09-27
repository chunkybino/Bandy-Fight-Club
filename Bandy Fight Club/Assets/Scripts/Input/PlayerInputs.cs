using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    public InputMapSO inputMap;

    public bool upPressed;// {get {return Input.GetKey(inputMap.up);}}
    public bool upHeld;// {get {return Input.GetKey(inputMap.up);}}

    public bool downPressed;// {get {return Input.GetKeyDown(inputMap.down);}}
    public bool downHeld;// {get {return Input.GetKey(inputMap.down);}}

    public bool leftPressed;// {get {return Input.GetKeyDown(inputMap.left);}}
    public bool leftHeld;// {get {return Input.GetKey(inputMap.left);}}

    public bool rightPressed;// {get {return Input.GetKeyDown(inputMap.right);}}
    public bool rightHeld;// {get {return Input.GetKey(inputMap.right);}}

    public bool jumpPressed;// {get {return Input.GetKeyDown(inputMap.jump);}}
    public bool jumpHeld;// {get {return Input.GetKey(inputMap.jump);}}

    public bool dodgePressed;// {get {return Input.GetKeyDown(inputMap.dodge);}}
    public bool dodgeHeld;// {get {return Input.GetKey(inputMap.dodge);}}

    public bool attackPressed;// {get {return Input.GetKeyDown(inputMap.attack);}}
    public bool attackHeld;// {get {return Input.GetKey(inputMap.attack);}}

    void Update()
    {
        upPressed = Input.GetKeyDown(inputMap.up);
        upHeld = Input.GetKey(inputMap.up);

        downPressed = Input.GetKeyDown(inputMap.down);
        downHeld = Input.GetKey(inputMap.down);

        leftPressed = Input.GetKeyDown(inputMap.left);
        leftHeld = Input.GetKey(inputMap.left);

        rightPressed = Input.GetKeyDown(inputMap.right);
        rightHeld = Input.GetKey(inputMap.right);

        jumpPressed = Input.GetKeyDown(inputMap.jump);
        jumpHeld = Input.GetKey(inputMap.jump);

        dodgePressed = Input.GetKeyDown(inputMap.dodge);
        dodgeHeld = Input.GetKey(inputMap.dodge);

        attackPressed = Input.GetKeyDown(inputMap.attack);
        attackHeld = Input.GetKey(inputMap.attack);
    }
}
