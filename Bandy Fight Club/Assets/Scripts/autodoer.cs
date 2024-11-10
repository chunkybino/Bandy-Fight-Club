using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class autodoer : MonoBehaviour
{
    //throwaway script i used to go to all the meshes in the render and changes their anchor reference from the player transform 
    //to the render transform, for seperating the render into its own prefab sperate from the player

    [SerializeField] bool doo;

    void Update()
    {
        if (doo)
        {
            doo = false;
            DOO();
        }
    }

    void DOO()
    {
        DoMe(transform);

        void DoMe(Transform me)
        {
            SpriteShapeMapper shape = me.GetComponent<SpriteShapeMapper>();
            MeshTest mesh = me.GetComponent<MeshTest>();
            MeshSpriteStuff sprite = me.GetComponent<MeshSpriteStuff>();

            if (shape) shape.pointAnchor = transform;
            if (mesh) mesh.pointAnchor = transform;
            if (sprite) sprite.referenceTransformAnchor = transform;

            for (int i = 0; i < me.childCount; i++)
            {
                Transform me2 = me.GetChild(i);
                DoMe(me2);
            }
        }
    }
}
