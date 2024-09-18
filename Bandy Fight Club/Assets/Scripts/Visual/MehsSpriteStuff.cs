using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MeshSpriteStuff : MonoBehaviour
{
    public Transform sprite;

    public Transform referenceTransform;
    public Transform referenceTransformAnchor;

    void Update()
    {
        Vector2 newPos = transform.position + referenceTransform.position - referenceTransformAnchor.position;
        sprite.position = new Vector3(newPos.x, newPos.y, sprite.position.z);
    }
}
