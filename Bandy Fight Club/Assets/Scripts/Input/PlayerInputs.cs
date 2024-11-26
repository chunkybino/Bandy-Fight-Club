using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    public InputMapSO inputMap;
    public ControllerInput controller;
    bool doController;

    InputAction upAction;
    InputAction downAction;
    InputAction leftAction;
    InputAction rightAction;
    InputAction jumpAction;
    InputAction dodgeAction;
    InputAction attackAction;
    InputAction heavyAction;

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

    void Awake()
    {
        doController = inputMap.doControllerInput;

        if (doController) 
        {
            controller = new ControllerInput();

            upAction = controller.Gameplay.Up;
            downAction = controller.Gameplay.Down;
            leftAction = controller.Gameplay.Left;
            rightAction = controller.Gameplay.Right;
            jumpAction = controller.Gameplay.Jump;
            dodgeAction = controller.Gameplay.Dodge;
            attackAction = controller.Gameplay.Attack;
            heavyAction = controller.Gameplay.Heavy;
        }
    }

    void Update()
    {
        bool active = inputsActive;
        if (!GameManager.singleton.gameStarted) active = false;

        if (active)
        {
            if (!doController)
            {
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
            else
            {
                upPressed = upAction.WasPressedThisFrame();
                upHeld = upAction.IsPressed();

                downPressed = downAction.WasPressedThisFrame();
                downHeld = downAction.IsPressed();

                leftPressed = leftAction.WasPressedThisFrame();
                leftHeld = leftAction.IsPressed();

                rightPressed = rightAction.WasPressedThisFrame();
                rightHeld = rightAction.IsPressed();

                jumpPressed = jumpAction.WasPressedThisFrame();
                jumpHeld = jumpAction.IsPressed();

                dodgePressed = dodgeAction.WasPressedThisFrame();
                dodgeHeld = dodgeAction.IsPressed();
                dodgeReleased = dodgeAction.WasReleasedThisFrame();

                attackPressed = attackAction.WasPressedThisFrame();
                attackHeld = attackAction.IsPressed();

                heavyPressed = heavyAction.WasPressedThisFrame();
                heavyHeld = heavyAction.IsPressed();print(heavyHeld);
            }
        }
        else
        {
            upPressed = false;
            upHeld = false;
            downPressed = false;
            downHeld = false;
            leftPressed = false;
            leftHeld = false;
            rightPressed = false;
            rightHeld = false;
            jumpPressed = false;
            jumpHeld = false;
            dodgePressed = false;
            dodgeHeld = false;
            dodgeReleased = false;
            attackPressed = false; 
            attackHeld = false;
            heavyPressed = false;
            heavyHeld = false;
        }
    }

    void HoldLeft()
    {
        print("true");
        leftHeld = true;
    }

    void OnEnable()
    {
        if (controller != null) controller.Gameplay.Enable();
    }
}
