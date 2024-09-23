using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    public InputsMapping inputMap;

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

    public bool attackPressed;
    public bool attackHeld;

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

[CreateAssetMenu(fileName = "InputMapSO", menuName = "ScriptableObjects/InputMap")]
public class InputsMapping : ScriptableObject
{
    public string up = "w";
    public string down = "s";
    public string left = "a";
    public string right = "d";

    public string jump = "1";

    public string dodge = "2";

    public string attack = "3";
}
