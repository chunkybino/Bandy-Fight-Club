using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    public InputMapSO inputMap;

    public bool inputsActive = true; //when disabled, all inputs return false

    public bool upPressed;// {get {return Input.GetKey(inputMap.up) && inputsActive;}}
    public bool upHeld;// {get {return Input.GetKey(inputMap.up) && inputsActive;}}

    public bool downPressed;// {get {return Input.GetKeyDown(inputMap.down) && inputsActive;}}
    public bool downHeld;// {get {return Input.GetKey(inputMap.down) && inputsActive;}}

    public bool leftPressed;// {get {return Input.GetKeyDown(inputMap.left) && inputsActive;}}
    public bool leftHeld;// {get {return Input.GetKey(inputMap.left) && inputsActive;}}

    public bool rightPressed;// {get {return Input.GetKeyDown(inputMap.right) && inputsActive;}}
    public bool rightHeld;// {get {return Input.GetKey(inputMap.right) && inputsActive;}}

    public bool jumpPressed;// {get {return Input.GetKeyDown(inputMap.jump) && inputsActive;}}
    public bool jumpHeld;// {get {return Input.GetKey(inputMap.jump) && inputsActive;}}

    public bool dodgePressed;// {get {return Input.GetKeyDown(inputMap.dodge) && inputsActive;}}
    public bool dodgeHeld;// {get {return Input.GetKey(inputMap.dodge) && inputsActive;}}
    public bool dodgeReleased;

    public bool attackPressed;// {get {return Input.GetKeyDown(inputMap.attack) && inputsActive;}}
    public bool attackHeld;// {get {return Input.GetKey(inputMap.attack) && inputsActive;}}

    public bool heavyPressed;// {get {return Input.GetKeyDown(inputMap.heavy) && inputsActive;}}
    public bool heavyHeld;// {get {return Input.GetKey(inputMap.heavy) && inputsActive;}}

    void Update()
    {
        bool active = inputsActive;
        if (!GameManager.singleton.gameStarted) active = false;

        upPressed = Input.GetKeyDown(inputMap.up) && active;
        upHeld = Input.GetKey(inputMap.up) && active;

        downPressed = Input.GetKeyDown(inputMap.down) && active;
        downHeld = Input.GetKey(inputMap.down) && active;

        leftPressed = Input.GetKeyDown(inputMap.left) && active;
        leftHeld = Input.GetKey(inputMap.left) && active;

        rightPressed = Input.GetKeyDown(inputMap.right) && active;
        rightHeld = Input.GetKey(inputMap.right) && active;

        jumpPressed = Input.GetKeyDown(inputMap.jump) && active;
        jumpHeld = Input.GetKey(inputMap.jump) && active;

        dodgePressed = Input.GetKeyDown(inputMap.dodge) && active;
        dodgeHeld = Input.GetKey(inputMap.dodge) && active;
        dodgeReleased = Input.GetKeyUp(inputMap.dodge) && active;

        attackPressed = Input.GetKeyDown(inputMap.attack) && active;
        attackHeld = Input.GetKey(inputMap.attack) && active;

        heavyPressed = Input.GetKeyDown(inputMap.heavy) && active;
        heavyHeld = Input.GetKey(inputMap.heavy) && active;
    }
}
