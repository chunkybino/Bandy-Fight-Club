using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "RenderTextures", menuName = "ScriptableObjects/RenderTextures")]
public class RenderTextureMaterials : ScriptableObject
{
    public RenderTexture[] renderTexQueue;
}
