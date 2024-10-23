using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] Camera camera;

    [SerializeField] float zOffset = -100;

    [SerializeField] float aspectRatio = 16f/9f;

    [SerializeField] float currentSize = 20;
    float horizontalSize;
    [SerializeField] float maxSize = 20;

    [SerializeField] Vector2 pos;
    [SerializeField] Vector2 targetPos;

    [SerializeField] bool doSideBounds = true;
    [SerializeField] Vector2 sideEdgeBounds;

    [SerializeField] List<CameraEffector> playerEffectors = new List<CameraEffector>();

    [SerializeField] DoubleInverseLerpData sizeMarginFromPlayerDistances; //change camera size margin around pllayers when they get closer
    float playerDistance;
    Vector2 playerBounds;

    void Update()
    {
        CalculateCameraEffectors();

        float size = playerDistance/2;
        size = sizeMarginFromPlayerDistances.Lerp(playerDistance)/2;
        print(sizeMarginFromPlayerDistances.Lerp(playerDistance));
        SetHorizontalSize(size);

        horizontalSize = currentSize*aspectRatio;
        camera.orthographicSize = currentSize;

        pos = targetPos;

        if (doSideBounds) pos.x = Mathf.Clamp(pos.x, sideEdgeBounds.x + horizontalSize, sideEdgeBounds.y - horizontalSize);

        transform.position = new Vector3(pos.x, pos.y, zOffset);
    }

    void CalculateCameraEffectors()
    {
        float totalX = 0;
        float totalWeight = 0;

        playerBounds = new Vector2(transform.position.x,transform.position.x); //highest and lowestest player pos, x is low, y is high
        int index = 0;
        foreach (CameraEffector e in playerEffectors)
        {
            if (e.target == null) continue;

            totalX += e.target.transform.position.x;
            totalWeight += e.weight;

            if (e.target.transform.position.x < playerBounds.x || index==0) playerBounds.x = e.target.transform.position.x;
            if (e.target.transform.position.x > playerBounds.y || index==0) playerBounds.y = e.target.transform.position.x;

            index++;
        }
        playerDistance = playerBounds.y - playerBounds.x;

        if (totalWeight == 0) return;

        targetPos.x = totalX / totalWeight;
    }

    void SetHorizontalSize(float size)
    {
        currentSize = size * 9 / 16;
    }
}
[System.Serializable]
public class CameraEffector
{
    public Transform target;
    public float weight;
}