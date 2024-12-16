using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "SoundListSO", menuName = "ScriptableObjects/SoundList")]
public class SoundsListSO : ScriptableObject
{
    public SoundData[] sounds;

    public SoundData FindByName(string name)
    {
        foreach (SoundData s in sounds)
        {
            if (s.name == name) return s;
        }
        return null;
    }
}
