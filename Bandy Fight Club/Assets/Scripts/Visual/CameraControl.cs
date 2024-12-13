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

    [SerializeField] float verticalPlayerLerp = 0.5f;
    [SerializeField] float normalYPos = 0;
    [SerializeField] float yPosDegree = 0.5f; //raise our y pos to this degree, should be less than 1

    [SerializeField] List<CameraEffector> playerEffectors = new List<CameraEffector>();

    float edgeDistance;
    float playerDistance;
    float playerEdgeDistanceRatio {get {return playerDistance/edgeDistance;}}

    float leftPlayerBound;
    float rightPlayerBound;
    float averagePlayerPos;
    float topPlayerPos;

    [SerializeField] Vector2 targetCameraEdges;
    [SerializeField] Vector2 cameraEdges;
    Vector2 verticalCameraEdges;
 
    Vector2 cameraPosSmoothVel;
    Vector2 cameraEdgeSmoothVel;
    [SerializeField] float cameraPosSmoothTime = 1;
    [SerializeField] float cameraEdgeSmoothTime = 1;

    public bool doZoomMult = false;
    public float zoomMult = 1;
    public bool doFocusTarget = false;
    public int targetFocusIndex;

    [SerializeField] GameObject spotlightPrefab;
    [SerializeField] List<GameObject> spotlights = new List<GameObject>();
    [SerializeField] Transform spotlightParent;
    [SerializeField] float spotlightLerpFactor = 0.1f;
    [SerializeField] float spotlightLinearSpeed = 4f;

    void Start()
    {
        cameraEdges = sideEdgeBounds;

        GameManager.singleton.onRespawn.AddListener(OnRespawn);
    }

    void Update()
    {
        edgeDistance = sideEdgeBounds.y - sideEdgeBounds.x;

        CalculateCameraEffectors();

        DoSpotlights();

        playerToBoundLerp = playerDistanceToBoundLerpCurve.Evaluate(playerEdgeDistanceRatio);
        if (doZoomMult) playerToBoundLerp = 1-((1-playerToBoundLerp)/zoomMult);
        centerToPlayerLerp = centerDistanceToCenterPlayerLerpCurve.Evaluate(2*Mathf.Abs(averagePlayerPos)/edgeDistance);

        targetCameraEdges.x = Mathf.Lerp(sideEdgeBounds.x, leftPlayerBound, playerToBoundLerp);
        targetCameraEdges.y = Mathf.Lerp(sideEdgeBounds.y, rightPlayerBound, playerToBoundLerp);
        
        verticalCameraEdges.x = transform.position.y - currentSize;
        verticalCameraEdges.y = transform.position.y + currentSize;

        cameraEdges.x = Mathf.SmoothDamp(cameraEdges.x, targetCameraEdges.x, ref cameraEdgeSmoothVel.x, cameraEdgeSmoothTime);
        cameraEdges.y = Mathf.SmoothDamp(cameraEdges.y, targetCameraEdges.y, ref cameraEdgeSmoothVel.y, cameraEdgeSmoothTime);

        float size = (cameraEdges.y-cameraEdges.x)/2;
        SetHorizontalSize(size);

        targetPos.x = (cameraEdges.x+cameraEdges.y)/2;
        targetPos.x = Mathf.Lerp(targetPos.x, averagePlayerPos, centerToPlayerLerp);

        targetPos.y = Mathf.Lerp(normalYPos, topPlayerPos, verticalPlayerLerp);
        targetPos.y = Mathf.Pow(targetPos.y, yPosDegree);
        if (doFocusTarget) targetPos = playerEffectors[targetFocusIndex].target.position;

        horizontalSize = currentSize*aspectRatio;
        camera.orthographicSize = currentSize;

        pos.x = Mathf.SmoothDamp(pos.x, targetPos.x, ref cameraPosSmoothVel.x, cameraPosSmoothTime);
        pos.y = Mathf.SmoothDamp(pos.y, targetPos.y, ref cameraPosSmoothVel.y, cameraPosSmoothTime);

        if (doSideBounds) pos.x = Mathf.Clamp(pos.x, sideEdgeBounds.x + horizontalSize, sideEdgeBounds.y - horizontalSize);

        if (float.IsNaN(pos.x)) pos.x = 0;

        transform.position = new Vector3(pos.x, pos.y, zOffset);
    }

    void CalculateCameraEffectors()
    {
        leftPlayerBound = sideEdgeBounds.x;
        rightPlayerBound = sideEdgeBounds.y;
        topPlayerPos = normalYPos;

        averagePlayerPos = 0;
        int totalPlayers = 0;

        int index = 0;
        foreach (CameraEffector e in playerEffectors)
        {
            if (e.target == null) continue;
            if (!e.active) continue;

            totalPlayers++;
            averagePlayerPos += e.target.transform.position.x;

            if (e.target.transform.position.x < leftPlayerBound || index==0) leftPlayerBound = e.target.transform.position.x;
            if (e.target.transform.position.x > rightPlayerBound || index==0) rightPlayerBound = e.target.transform.position.x;

            if (e.target.transform.position.y > topPlayerPos) topPlayerPos = e.target.transform.position.y;

            index++;
        }
        if (doFocusTarget)
        {
           leftPlayerBound = playerEffectors[targetFocusIndex].target.transform.position.x; 
           rightPlayerBound = playerEffectors[targetFocusIndex].target.transform.position.x;
        }

        playerDistance = rightPlayerBound - leftPlayerBound;
        averagePlayerPos = averagePlayerPos/playerEffectors.Count;
    }

    void DoSpotlights()
    {
        for (int i = 0; i < playerEffectors.Count || i < spotlights.Count; i++)
        {
            if (i >= playerEffectors.Count)
            {
                spotlights[i].SetActive(false);
                continue;
            }
            if (i >= spotlights.Count)
            {
                spotlights.Add(Instantiate(spotlightPrefab, new Vector3(0,0,0), Quaternion.identity, spotlightParent));
            }

            Vector2 p = spotlights[i].transform.position;
            //float x = spotlights[i].transform.position.x;
            p.x = Mathf.Lerp(p.x, playerEffectors[i].target.position.x, 1-Mathf.Pow(spotlightLerpFactor, Time.deltaTime));
            //float y = spotlights[i].transform.position.y;
            p.y = Mathf.Lerp(p.y, playerEffectors[i].target.position.y, 1-Mathf.Pow(spotlightLerpFactor, Time.deltaTime));

            Vector2 dis = p - new Vector2(spotlights[i].transform.position.x, spotlights[i].transform.position.y);
            Vector2 normalDis = dis.normalized;

            if (normalDis.x > 0)
            {
                p.x = Mathf.Min(p.x + normalDis.x*spotlightLinearSpeed, playerEffectors[i].target.position.x);
            }
            else if (normalDis.x < 0)
            {
                p.x = Mathf.Max(p.x + normalDis.x*spotlightLinearSpeed, playerEffectors[i].target.position.x);
            }

            if (p.y < playerEffectors[i].target.position.y)
            {
                p.y = Mathf.Min(p.y + normalDis.y*spotlightLinearSpeed, playerEffectors[i].target.position.y);
            }
            else if (p.y > playerEffectors[i].target.position.y)
            {
                p.y = Mathf.Max(p.y + normalDis.y*spotlightLinearSpeed, playerEffectors[i].target.position.y);
            }

            spotlights[i].transform.position = p;
        }
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
        playerEffectors.Add(new CameraEffector() {target = player, weight = 1, active = true});
    }
    public void RemovePlayerEffector(int index)
    {
        playerEffectors[index].active = false;
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
    public bool active = true;
}