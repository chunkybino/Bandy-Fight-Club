using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "TeamColorSO", menuName = "ScriptableObjects/TeamColor")]
public class TeamColorsSO : ScriptableObject
{
    public Color[] colors;
    public string[] names;
    public Material[] materials;

    public Color drawColor;
    public string drawName;
}

