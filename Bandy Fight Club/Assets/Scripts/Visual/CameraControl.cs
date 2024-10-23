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

    [SerializeField] float posPowerDegree = 0.5f; //raise our target position to this power

    [SerializeField] bool doSideBounds = true;
    [SerializeField] Vector2 sideEdgeBounds;

    [SerializeField] float playerToBoundLerp = 0.5f;
    [SerializeField] AnimationCurve playerDistanceToBoundLerpCurve;
    [SerializeField] float centerToPlayerLerp = 0.5f; //lerp the center of the camera to the middle player pos
    [SerializeField] AnimationCurve centerDistanceToCenterPlayerLerpCurve;

    [SerializeField] List<CameraEffector> playerEffectors = new List<CameraEffector>();

    //[SerializeField] DoubleInverseLerpData sizeMarginFromPlayerDistances; //change camera size margin around pllayers when they get closer
    float edgeDistance;
    float playerDistance;
    float playerEdgeDistanceRatio {get {return playerDistance/edgeDistance;}}

    float leftPlayerBound;
    float rightPlayerBound;
    float middlePlayerPos {get {return (leftPlayerBound+rightPlayerBound)/2;}}

    [SerializeField] Vector2 targetCameraEdges;
    [SerializeField] Vector2 cameraEdges;
 
    float cameraPosSmoothVel;
    Vector2 cameraEdgeSmoothVel;
    [SerializeField] float cameraPosSmoothTime = 1;
    [SerializeField] float cameraEdgeSmoothTime = 1;

    void Start()
    {
        cameraEdges = sideEdgeBounds;

        GameManager.singleton.onRespawn.AddListener(OnRespawn);
    }

    void Update()
    {
        edgeDistance = sideEdgeBounds.y - sideEdgeBounds.x;

        CalculateCameraEffectors();

        playerToBoundLerp = playerDistanceToBoundLerpCurve.Evaluate(playerEdgeDistanceRatio);
        centerToPlayerLerp = centerDistanceToCenterPlayerLerpCurve.Evaluate(2*Mathf.Abs(middlePlayerPos)/edgeDistance);

        targetCameraEdges.x = Mathf.Lerp(sideEdgeBounds.x, leftPlayerBound, playerToBoundLerp);
        targetCameraEdges.y = Mathf.Lerp(sideEdgeBounds.y, rightPlayerBound, playerToBoundLerp);

        cameraEdges.x = Mathf.SmoothDamp(cameraEdges.x, targetCameraEdges.x, ref cameraEdgeSmoothVel.x, cameraEdgeSmoothTime);
        cameraEdges.y = Mathf.SmoothDamp(cameraEdges.y, targetCameraEdges.y, ref cameraEdgeSmoothVel.y, cameraEdgeSmoothTime);

        float size = (cameraEdges.y-cameraEdges.x)/2;
        SetHorizontalSize(size);

        targetPos.x = (cameraEdges.x+cameraEdges.y)/2;
        targetPos.x = Mathf.Lerp(targetPos.x, middlePlayerPos, centerToPlayerLerp);
        //float size = playerDistance/2;
        //size = sizeMarginFromPlayerDistances.Lerp(playerDistance)/2;
        //SetHorizontalSize(size);

        horizontalSize = currentSize*aspectRatio;
        camera.orthographicSize = currentSize;

        pos.x = Mathf.SmoothDamp(pos.x, targetPos.x, ref cameraPosSmoothVel, cameraPosSmoothTime);

        if (doSideBounds) pos.x = Mathf.Clamp(pos.x, sideEdgeBounds.x + horizontalSize, sideEdgeBounds.y - horizontalSize);

        transform.position = new Vector3(pos.x, pos.y, zOffset);
    }

    void CalculateCameraEffectors()
    {
        //float totalX = 0;
        //float totalWeight = 0;

        //playerBounds = new Vector2(transform.position.x,transform.position.x); //highest and lowestest player pos, x is low, y is high
        leftPlayerBound = sideEdgeBounds.x;
        rightPlayerBound = sideEdgeBounds.y;
        int index = 0;
        foreach (CameraEffector e in playerEffectors)
        {
            if (e.target == null) continue;

            if (e.target.transform.position.x < leftPlayerBound || index==0) leftPlayerBound = e.target.transform.position.x;
            if (e.target.transform.position.x > rightPlayerBound || index==0) rightPlayerBound = e.target.transform.position.x;

            index++;
        }
        playerDistance = rightPlayerBound - leftPlayerBound;

        //if (totalWeight == 0) return;

        //targetPos.x = totalX / totalWeight;
    }

    void SetHorizontalSize(float size)
    {
        currentSize = size * 9 / 16;
    }

    public void ResetEffectors()
    {
        playerEffectors.Clear();
    }
    public void AddPlayerEffector(Transform player)
    {
        playerEffectors.Add(new CameraEffector() {target = player, weight = 1});
    }

    public void OnRespawn()
    {
        cameraEdges = sideEdgeBounds;
    }
}
[System.Serializable]
public class CameraEffector
{
    public Transform target;
    public float weight = 1;
}