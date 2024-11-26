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
    
    public bool AnyInputPressed()
    {
        if(Input.GetKeyDown(up)) return true;
        if(Input.GetKeyDown(down)) return true;
        if(Input.GetKeyDown(left)) return true;
        if(Input.GetKeyDown(right)) return true;
        if(Input.GetKeyDown(jump)) return true;
        if(Input.GetKeyDown(dodge)) return true;
        if(Input.GetKeyDown(attack)) return true;
        if(Input.GetKeyDown(heavy)) return true;
        return false;
    }
}
