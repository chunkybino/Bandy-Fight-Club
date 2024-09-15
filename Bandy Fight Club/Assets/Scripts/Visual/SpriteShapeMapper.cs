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

    /*public SpriteShapeController shapeControl;
    public Transform[] shapePoints = new Transform[0];
    public float minShapePointDistance = 0.04f;*/

    void Update()
    {
        List<Vector3> linePoints = new List<Vector3>();
        List<SpriteShapePoint> secondLinePoints = new List<SpriteShapePoint>();
        foreach (SpriteShapePoint point in points)
        {
            if (!point) continue;

            foreach(Vector3 p in point.points)
            {
                linePoints.Add(p);
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