using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[ExecuteInEditMode]
public class SpriteShapeMapper : MonoBehaviour
{
    public SpriteShapePoint[] points;

    public LineRenderer line;

    public List<LineRenderer> childLines;

    public float lineZOffset = 0.2f;

    public bool doLocalSpace;

    public Transform pointAnchor;

    public bool flip;

    void LateUpdate()
    {
        List<Vector3> linePoints = new List<Vector3>();
        //List<SpriteShapePoint> secondLinePoints = new List<SpriteShapePoint>();
        foreach (SpriteShapePoint point in points)
        {
            if (!point) continue;
            if (!point.enabled) continue;

            foreach(Vector3 p in point.points)
            {
                Vector3 p2 = p - pointAnchor.position;
                if (flip) p2 = new Vector3(-p2.x,p2.y,p2.z);
                linePoints.Add(p2);
            }
        }

        Vector3[] lineArray = UFunc.ListToArray(linePoints);
        SetLinePoints(line, lineArray);


        void SetLinePoints(LineRenderer theLine, Vector3[] thePoints)
        {
            Vector3 offset = new Vector3(0,0,-lineZOffset);
            if (doLocalSpace) offset += transform.position;
            for (int i = 0; i < thePoints.Length; i++) {thePoints[i] += offset;}
            theLine.positionCount = thePoints.Length;
            theLine.SetPositions(thePoints);
        }
    }
}