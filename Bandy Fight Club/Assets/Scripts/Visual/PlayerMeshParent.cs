using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode] 
public class PlayerMeshParent : MonoBehaviour
{
    public PlayerFunc playerFunc;
    public int facing;

    public Transform anchor;

    public int team;

    public BeanBody skeleton;
    public Transform flatTex;

    public SpriteShapeMapper[] lines;
    public MeshTest[] meshes;
    public MeshSpriteStuff[] sprites;

    public Transform[] unmoveTransforms; //dont move to far away

    public bool instanceMeshes;

    public Camera textureCam;
    public Vector2 pos = new Vector2(100,-1000);

    public RenderTexture renderTex;
    public int renderTexSize = 1024;
    public MeshRenderer meshRenderer;

    public bool doIt;

    [SerializeField] RenderTextureMaterials materialTextures;
    public int num;

    static int players = 0;

    [SerializeField] bool countPlayers = true;

    public Color outlineColor;
    public float outlineThickness = 0.01f;
    public float outlineAlpha = 1;

    void Awake()
    {
        if (countPlayers) players = 0;
    }

    void Start()
    {
        if (countPlayers)
        {
            num = players;
            players++;
        }

        pos += new Vector2(num*20, 0);

        if (playerFunc) team = playerFunc.playerStatus.team.team;

        if (GameManager.singleton)
        {
            if (GameManager.singleton.teamOutlineColors.colors.Length > team) 
            {
                outlineColor = GameManager.singleton.teamOutlineColors.colors[team];
            }
        }

        if (playerFunc) anchor = playerFunc.transform;

        DoTextureMaterial();
    }

    void Update()
    {
        if (doIt)
        {
            doIt = false;
            DoTextureMaterial();
        }

        transform.position = pos;
        if (textureCam) textureCam.transform.position = new Vector3(pos.x,pos.y,-100);

        //outline color
        MaterialPropertyBlock matBlock = new MaterialPropertyBlock();
        matBlock.SetTexture("_MainTex", renderTex);
        matBlock.SetColor("_Color", outlineColor);
        matBlock.SetFloat("_Thickness", outlineThickness);
        matBlock.SetFloat("_Opacity", outlineAlpha);
        meshRenderer.SetPropertyBlock(matBlock, 0);

        UpdateFacing();

        foreach (Transform t in unmoveTransforms)
        {
            t.localPosition = anchor.position + new Vector3(-pos.x,-pos.y,0);
        }

        if (instanceMeshes)
        {
            InstanceMeshes();
        }
    }

    public void DoTextureMaterial()
    {
        renderTex = materialTextures.renderTexQueue[num];

        MaterialPropertyBlock matBlock1 = new MaterialPropertyBlock();
        MaterialPropertyBlock matBlock2 = new MaterialPropertyBlock();

        matBlock1.SetTexture("_MainTex", renderTex);
        matBlock1.SetColor("_Color", outlineColor);
        matBlock1.SetFloat("_Thickness", outlineThickness);
        matBlock1.SetFloat("_Opacity", outlineAlpha);
        
        matBlock2.SetTexture("_MainTex", renderTex);

        meshRenderer.SetPropertyBlock(matBlock1, 0);
        meshRenderer.SetPropertyBlock(matBlock2, 1);

        //textureMaterial.mainTexture = renderTex;

        textureCam.targetTexture = renderTex;
    }

    public void UpdateFacing()
    {
        if (playerFunc) facing = playerFunc.facing;
        transform.localScale = new Vector3(facing, transform.localScale.y, transform.localScale.z);
        float skeletonScale = facing;
        if (!skeleton.shouldFlip) skeletonScale = 1;
        skeleton.transform.localScale = new Vector3(skeletonScale, skeleton.transform.localScale.y, skeleton.transform.localScale.z);
    }

    public void InstanceMeshes()
    {
        instanceMeshes = false;
        foreach (MeshTest mesh in meshes)
        {
            mesh.instanceMesh = true;
        }
    }
}

[System.Serializable]
[CreateAssetMenu(fileName = "RenderTextures", menuName = "ScriptableObjects/RenderTextures")]
public class RenderTextureMaterials : ScriptableObject
{
    public RenderTexture[] renderTexQueue;
}
