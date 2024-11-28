using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InputMapSO", menuName = "ScriptableObjects/InputMap")]
public class InputMapSO : ScriptableObject
{
    public string up = "w";
    public string down = "s";
    public string left = "a";
    public string right = "d";

    public string jump = "1";

    public string dodge = "2";

    public string attack = "3";
    public string heavy = "4";

    public bool doControllerInput;

    public Sprite icon;
    
    public bool AnyInputPressed()
    {
        if (!doControllerInput) 
        {
            if(Input.GetKeyDown(up)) return true;
            if(Input.GetKeyDown(down)) return true;
            if(Input.GetKeyDown(left)) return true;
            if(Input.GetKeyDown(right)) return true;
            if(Input.GetKeyDown(jump)) return true;
            if(Input.GetKeyDown(dodge)) return true;
            if(Input.GetKeyDown(attack)) return true;
            if(Input.GetKeyDown(heavy)) return true;
        }
        if (doControllerInput) 
        {
            /*ControllerInput controller = new ControllerInput();
            controller.Gameplay.Enable();

            Debug.Log("y");

            if(controller.Gameplay.Up.WasPressedThisFrame()) return true;
            if(controller.Gameplay.Down.WasPressedThisFrame()) return true;
            if(controller.Gameplay.Left.WasPressedThisFrame()) return true;
            if(controller.Gameplay.Right.WasPressedThisFrame()) return true;
            if(controller.Gameplay.Jump.WasPressedThisFrame()) return true;
            if(controller.Gameplay.Dodge.WasPressedThisFrame()) return true;
            if(controller.Gameplay.Attack.WasPressedThisFrame()) return true;
            if(controller.Gameplay.Heavy.WasPressedThisFrame()) return true;*/
        }

        return false;
    }

    /*public bool GetUpInput()
    {
        if (doControllerInput) 
        {
            ControllerInput controller = new ControllerInput();
            controller.Gameplay.Enable();
            controller.Gameplay.Up.WasPressedThisFrame();
        }
        return Input.GetKeyDown(up);
    }
    public bool GetDownInput()
    {
        if (doControllerInput) 
        {
            ControllerInput controller = new ControllerInput();
            controller.Gameplay.Enable();
            controller.Gameplay.Down.WasPressedThisFrame();
        }
        return Input.GetKeyDown(down);
    }*/
}
