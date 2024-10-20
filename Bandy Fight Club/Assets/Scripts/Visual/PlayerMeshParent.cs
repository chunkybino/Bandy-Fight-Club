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

    void Update()
    {
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
