using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode] 
public class PlayerMeshParent : MonoBehaviour
{
    public PlayerFunc playerFunc;
    public int facing;

    public Transform skeleton;

    public SpriteShapeMapper[] lines;
    public MeshSpriteStuff[] sprites;

    void Update()
    {
        facing = playerFunc.facing;

        skeleton.localScale = new Vector3(facing, skeleton.localScale.y, skeleton.localScale.z);

        foreach (SpriteShapeMapper line in lines)
        {
            if (facing > 0) line.flip = false;
            if (facing < 0) line.flip = true;
        }
        foreach (MeshSpriteStuff sprite in sprites)
        {
            if (facing > 0) sprite.flip = false;
            if (facing < 0) sprite.flip = true;
        }
    }
}
