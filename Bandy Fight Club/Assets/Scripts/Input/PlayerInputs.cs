using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    public InputMapSO inputMap;

    public bool inputsActive = true; //when disabled, all inputs return false

    public bool upPressed {get {return Input.GetKey(inputMap.up) && inputsActive;}}
    public bool upHeld {get {return Input.GetKey(inputMap.up) && inputsActive;}}

    public bool downPressed {get {return Input.GetKeyDown(inputMap.down) && inputsActive;}}
    public bool downHeld {get {return Input.GetKey(inputMap.down) && inputsActive;}}

    public bool leftPressed {get {return Input.GetKeyDown(inputMap.left) && inputsActive;}}
    public bool leftHeld {get {return Input.GetKey(inputMap.left) && inputsActive;}}

    public bool rightPressed {get {return Input.GetKeyDown(inputMap.right) && inputsActive;}}
    public bool rightHeld {get {return Input.GetKey(inputMap.right) && inputsActive;}}

    public bool jumpPressed {get {return Input.GetKeyDown(inputMap.jump) && inputsActive;}}
    public bool jumpHeld {get {return Input.GetKey(inputMap.jump) && inputsActive;}}

    public bool dodgePressed {get {return Input.GetKeyDown(inputMap.dodge) && inputsActive;}}
    public bool dodgeHeld {get {return Input.GetKey(inputMap.dodge) && inputsActive;}}

    public bool attackPressed {get {return Input.GetKeyDown(inputMap.attack) && inputsActive;}}
    public bool attackHeld {get {return Input.GetKey(inputMap.attack) && inputsActive;}}

    public bool heavyPressed {get {return Input.GetKeyDown(inputMap.heavy) && inputsActive;}}
    public bool heavyHeld {get {return Input.GetKey(inputMap.heavy) && inputsActive;}}

    /*void Update()
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
    }*/
}
