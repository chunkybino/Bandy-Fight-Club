using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] Camera camera;

    [SerializeField] float zOffset = -100;

    [SerializeField] float aspectRatio = 16f/9f;

    [SerializeField] float currentSize = 20;
    float horizontalSize {get {return currentSize*aspectRatio;}}
    [SerializeField] float maxSize = 20;

    [SerializeField] Vector2 pos;

    [SerializeField] Vector2 sideEdgeBounds;
    Vector2 sideEdgePos {get {return new Vector2(transform.position.x-horizontalSize,transform.position.x+horizontalSize);}}

    void Update()
    {
        camera.orthographicSize = currentSize;

        if (sideEdgePos.x < sideEdgeBounds.x) pos.x = sideEdgeBounds.x + horizontalSize;
        if (sideEdgePos.y > sideEdgeBounds.y) pos.x = sideEdgeBounds.y + horizontalSize;

        transform.position = new Vector3(pos.x, pos.y, zOffset);
    }
}