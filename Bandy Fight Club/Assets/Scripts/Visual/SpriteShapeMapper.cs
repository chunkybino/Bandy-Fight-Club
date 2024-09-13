using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[ExecuteInEditMode]
public class SpriteShapeMapper : MonoBehaviour
{
    //public Vector3[] points;
    //public Transform[] points;
    public Transform[] objectPoints;
    public SpriteShapeMapPoint[] points;

    public SpriteShapeController shape;
    public LineRenderer line;

    public bool updateObjectsToList;

    void Update()
    {
        if (updateObjectsToList)
        {
            updateObjectsToList = false;

            SpriteShapeMapPoint[] newList = new SpriteShapeMapPoint[objectPoints.Length];
            for (int i = 0; i < objectPoints.Length; i++)
            {
                newList[i] = new SpriteShapeMapPoint();
                newList[i].point = objectPoints[i];
            }
            points = newList;
        }

        Spline spline = shape.spline;

        spline.Clear();

        List<Vector3> newPoints = new List<Vector3>();
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 pos = points[i].point.position;

            bool doPoint = true;
            bool doPos2 = false;

            if (points[i].doByIntersection)
            {
                Vector2 pos1 = pos;
                Vector2 pos2 = points[i].upperIntersectionPoint.position;

                Vector2 distance1 = points[i-1].point.position - pos;
                Vector2 distance2 = points[i+1].point.position - points[i].upperIntersectionPoint.position;
                float slope1 = distance1.y/distance1.x;
                float slope2 = distance2.y/distance2.x;

                //float x = (pos2.y + slope2*-pos2.x - pos1.y - slope1*-pos1.x) / (slope1 - slope2);

                Vector2 newPosVec2 = UFunc.SlopeIntersect(slope1, pos1, slope2, pos2);
                Vector3 newPos = new Vector3(newPosVec2.x,newPosVec2.y,0);
                //Vector3 newPos = new Vector3(x, slope1*(x-pos1.x) + pos1.y);

                bool shouldDo = true;

                if ((newPos-pos).magnitude >= points[i].maxIntersectionDistance) shouldDo = false;  

                float angleDifference = UFunc.VecToAngle(distance1) - UFunc.VecToAngle(distance2);
                angleDifference = UFunc.FullToNegativeRangeAngle(angleDifference);
                //print(angleDifference);
                if (angleDifference > points[i].intersectionAngleDifferenceRange.y || angleDifference < points[i].intersectionAngleDifferenceRange.x) shouldDo = false;
                
                if (shouldDo)
                {
                    pos = newPos;
                }
                else
                {
                    doPos2 = true;
                }
            }
            else if (points[i].pinNeighborConnection) 
            {
                Vector3 pos1 = points[i-1].point.position;
                Vector3 pos2 = points[i+1].point.position;
                Vector2 dis = pos2-pos1;
                float slope = dis.y/dis.x;
                float perpSlope = dis.x/dis.y;
                Vector2 intersect = UFunc.SlopeIntersect(slope, points[i+1].point.position, perpSlope, pos);
                float verticalIntersect = pos1.y + slope*(pos.x-pos1.x); //find where the point intersects its neighbors line vertically
                
                if (!points[i].inversePin)
                {
                    if (verticalIntersect < pos.y) pos = intersect;//doPoint = false; //if the intersect goes past the pin
                }
                else
                {
                    if (verticalIntersect > pos.y) pos = intersect;
                }
            }
            /*else if (points[i].interpolateToNeighborMidpoint) 
            {
                Vector3 pos1 = points[i-1].point.position;
                Vector3 pos2 = points[i+1].point.position;
                Vector3 midPoint = UFunc.LerpVector3(pos1,pos2,0.5f);

                pos = UFunc.LerpVector3(pos,midPoint,points[i].neighborInterpFactor);
            }*/

            if (doPoint)
            {
                newPoints.Add(pos);
                if (doPos2) //for intersection stuff, insert an extra point if the intersection condidtion arnt met, iterated the insert index twice
                {
                    if ((pos-points[i].upperIntersectionPoint.position).magnitude > 0.05f) //dont add if the points are too close
                    {
                        newPoints.Add(points[i].upperIntersectionPoint.position);
                    }
                }
            }
        }
        Vector3[] vecArray = UFunc.ListToArray(newPoints);
        line.positionCount = vecArray.Length;
        line.SetPositions(vecArray);
        for (int i = 0; i < newPoints.Count; i++)
        {
            spline.InsertPointAt(i, newPoints[i]);
        }
        shape.RefreshSpriteShape();
    }
}
[System.Serializable]
public class SpriteShapeMapPoint
{
    public Transform point;

    public bool doByIntersection;
    public Transform upperIntersectionPoint;
    public float maxIntersectionDistance; //dont do intersection point if the point is too far
    public Vector2 intersectionAngleDifferenceRange;

    public bool pinNeighborConnection;
    public bool inversePin;

    //public bool interpolateToNeighborMidpoint;
    //[Range(0,1)] public float neighborInterpFactor;
}