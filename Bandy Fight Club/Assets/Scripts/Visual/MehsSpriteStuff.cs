using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MeshSpriteStuff : MonoBehaviour
{
    public Transform sprite;

    public Transform referenceTransform;
    public Transform referenceTransformAnchor;

    public bool flip;

    Vector2 relativePos;
    Vector2 newPos;

    void Update()
    {
        relativePos = referenceTransform.position - referenceTransformAnchor.position;
        if (flip) relativePos = new Vector2(-relativePos.x,relativePos.y);
        newPos = UFunc.AddVector3ToVector2(transform.position, relativePos);

        sprite.position = new Vector3(newPos.x, newPos.y, sprite.position.z);
        print(gameObject.name + "   " + sprite.position.ToString() + "   " + newPos.ToString());
    }
    void LateUpdate()
    {
        print(gameObject.name + "   " + sprite.position.ToString() + "   " + newPos.ToString() + "    late");
    }
}
