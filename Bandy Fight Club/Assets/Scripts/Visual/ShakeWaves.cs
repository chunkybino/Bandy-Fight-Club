using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "ShakeWaves", menuName = "ScriptableObjects/ShaveWaves")]
public class ShakeWaves : ScriptableObject
{
    public WaveData[] shakeWaves;

    [System.Serializable]
    public struct WaveData
    {
        public float freq;
        public float amp;
    }
}
