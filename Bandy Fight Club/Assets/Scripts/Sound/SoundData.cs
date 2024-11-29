using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundData
{
    public string name;

    public AudioClip clip;
    [Range(0f,1f)] public float volume = 0.5f;
    [Range(0f,3f)] public float pitch = 1;

    public bool doRandomSound;
    public AudioClip[] clips;

    public bool doRandomVolume;
    public Vector2 randVolumeRange;

    public bool doRandomPitch;
    public Vector2 randPitchRange;
}
