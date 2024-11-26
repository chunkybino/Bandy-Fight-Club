using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    public InputMapSO inputMap;
    public ControllerInput controller;
    bool doController;

    public bool inputsActive = true; //when disabled, all inputs return false

    public bool upPressed;
    public bool upHeld;

    public bool downPressed;
    public bool downHeld;

    public bool leftPressed;
    public bool leftHeld;

    public bool rightPressed;
    public bool rightHeld;

    public bool jumpPressed;
    public bool jumpHeld;

    public bool dodgePressed;
    public bool dodgeHeld;
    public bool dodgeReleased;

    public bool attackPressed;
    public bool attackHeld;

    public bool heavyPressed;
    public bool heavyHeld;

    void Awake()
    {
        doController = inputMap.doControllerInput;

        if (doController) 
        {
            controller = new ControllerInput();

            controller.Gameplay.Left.performed += c => leftHeld = c.ReadValue<bool>();
            controller.Gameplay.Left.canceled += c => leftHeld = c.ReadValue<bool>();
        }
    }

    void Update()
    {
        bool active = inputsActive;
        if (!GameManager.singleton.gameStarted) active = false;

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
            //leftHeld = controller.Gameplay.Left.performed && active;
        }
    }

    void HoldLeft()
    {
        print("true");
        leftHeld = true;
    }

    void OnEnable()
    {
        controller.Gameplay.Enable();
    }
}
