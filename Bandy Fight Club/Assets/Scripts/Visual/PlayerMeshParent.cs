using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode] 
public class PlayerMeshParent : MonoBehaviour
{
    public PlayerFunc playerFunc;
    public int facing;

    public Transform skeleton;
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
    public Material textureMaterial;
    public Material referenceMaterial;

    public bool doIt;

    [SerializeField] RenderTextureMaterials materialTextures;
    [SerializeField] int num;

    static int players = 0;

    void Awake()
    {
        players = 0;
    }

    void Start()
    {
        num = players;
        players++;
        pos += new Vector2(num*10, 0);

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

        UpdateFacing();

        //transform.localScale = new Vector3(facing, transform.localScale.y, transform.localScale.z);

        //flatTex.localScale = new Vector3(facing, skeleton.localScale.y, skeleton.localScale.z);

        /*foreach (SpriteShapeMapper line in lines)
        {
            if (facing > 0) line.flip = false;
            if (facing < 0) line.flip = true;
        }
        foreach (MeshSpriteStuff sprite in sprites)
        {
            if (facing > 0) sprite.flip = false;
            if (facing < 0) sprite.flip = true;
        }*/

        foreach (Transform t in unmoveTransforms)
        {
            t.localPosition = playerFunc.transform.position + new Vector3(-pos.x,-pos.y,0);
        }

        if (instanceMeshes)
        {
            InstanceMeshes();
        }
    }

    public void DoTextureMaterial()
    {
        textureMaterial = materialTextures.renderMatQueue[num];
        renderTex = materialTextures.renderTexQueue[num];

        meshRenderer.material = textureMaterial;
        textureMaterial.mainTexture = renderTex;

        textureCam.targetTexture = renderTex;
    }

    public void UpdateFacing()
    {
        facing = playerFunc.facing;
        transform.localScale = new Vector3(facing, transform.localScale.y, transform.localScale.z);
        skeleton.localScale = new Vector3(facing, skeleton.localScale.y, skeleton.localScale.z);
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

[CreateAssetMenu(fileName = "RenderTextures", menuName = "ScriptableObjects/RenderTextures")]
public class RenderTextureMaterials : ScriptableObject
{
    public Material[] renderMatQueue;
    public RenderTexture[] renderTexQueue;
}
