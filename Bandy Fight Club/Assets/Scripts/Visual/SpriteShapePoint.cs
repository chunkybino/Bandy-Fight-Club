using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpriteShapePoint : MonoBehaviour
{
    public List<Vector3> points = new List<Vector3>(); 

    public bool doByIntersection;
    public Transform intersectionInnerPoint1;
    public Transform intersectionInnerPoint2;
    public Transform intersectionOuterPoint1;
    public Transform intersectionOuterPoint2;

    public Transform intersectPoint;

    public Transform movePastPoint;

    public IntersectPoints intersect1Points;
    public IntersectPoints intersect2Points;
    public IntersectPoints intersectBothPoints;

    public IntersectPoints intersectNoSameIntersectDirectionPoints; //f our intersect is not in bounds, and its above the bounds of line1
    public IntersectPoints intersectNoDiffIntersectDirectionPoints;

    float intersectLerp1;
    float intersectLerp2;

    public bool doIfSlopeDifference;
    public Transform slopePoint1;
    public Transform slopePoint2;
    public bool slopeGreaterThan;
    float slope1;
    float slope2;

    public bool doMeshPoints;
    public Transform[] meshPoints = new Transform[0];

    public bool flipPointOrder;

    void Update()
    {
        points.Clear();

        bool doSelf = true;

        if (doByIntersection)
        {
            bool notNull = true;
            if (!intersectionInnerPoint1) notNull = false;
            if (!intersectionInnerPoint2) notNull = false;
            if (!intersectionOuterPoint1) notNull = false;
            if (!intersectionOuterPoint2) notNull = false;

            if (notNull)
            {
                doSelf = false;
                DoIntersectionPoint();
            }
        }

        if (doIfSlopeDifference && slopePoint1 && slopePoint2)
        {
            doSelf = false;

            Vector3 diff1 = transform.position-slopePoint1.position;
            Vector3 diff2 = transform.position-slopePoint2.position;

            float slopeDiff = UFunc.SlopeDifference(diff1.y/diff1.x,diff2.y/diff2.x);

            if (slopeGreaterThan == slopeDiff > 0) 
            {
                points.Add(transform.position);
            }
            else
            {
                points.Add(UFunc.LerpVector3(slopePoint1.position, slopePoint2.position, 0.5f));
            }
        }

        if (doSelf)
        {
            points.Add(transform.position);
        }

        if (flipPointOrder)
        {
            points = UFunc.FlipList(points);
        }

        if (doMeshPoints)
        {
            for (int i = 0; i < meshPoints.Length; i++)
            {
                if (i < points.Count)
                {
                    meshPoints[i].position = points[i];
                }
                else if (points.Count > 0)
                {
                    meshPoints[i].position = points[points.Count-1];
                }
            }
        }
    }

    void DoIntersectionPoint()
    {
        Vector2 dis1 = intersectionInnerPoint1.position-intersectionOuterPoint1.position;
        Vector2 dis2 = intersectionOuterPoint2.position-intersectionInnerPoint2.position;
        float slope1 = dis1.y/dis1.x;
        float slope2 = dis2.y/dis2.x;
        Vector2 intersect = UFunc.SlopeIntersect(slope1, intersectionInnerPoint1.position, slope2, intersectionInnerPoint2.position);

        if (intersectPoint) intersectPoint.position = intersect;

        intersectLerp1 = UFunc.InverseLerpUnclamped(intersectionInnerPoint1.position.x, intersectionOuterPoint1.position.x, intersect.x);
        intersectLerp2 = UFunc.InverseLerpUnclamped(intersectionInnerPoint2.position.x, intersectionOuterPoint2.position.x, intersect.x);

        bool intersectLine1 = UFunc.Between01(intersectLerp1);
        bool intersectLine2 = UFunc.Between01(intersectLerp2);

        //movepast point
        if (intersectLine1 && intersectLine2 && movePastPoint)
        {
            Vector3 inPos = intersectionInnerPoint1.position;
            Vector3 outPos = intersectionOuterPoint1.position;
            float movePastFactor = Mathf.InverseLerp(inPos.x, outPos.x, intersect.x); //move the point closer to the inner point, the closer the outer and intersect point are to eachother

            Vector3 movePastPos = UFunc.LerpVector3(inPos, intersect, 1-movePastFactor);

            movePastPoint.position = movePastPos; 
        }

        //intersect points
        if (intersectLine1 && intersectLine2)
        {
            points = AppendTransformToVector3(points, intersectBothPoints.points);
        }
        else if (intersectLine1)
        {
            points = AppendTransformToVector3(points, intersect1Points.points);
            if (intersectLerp2 > 1)
            {
                points = AppendTransformToVector3(points, intersect1Points.aboveIntersectPoints);
            }
            else if (intersectLerp2 < 0)
            {
                points = AppendTransformToVector3(points, intersect1Points.belowIntersectPoints);
            }
        }
        else if (intersectLine2)
        {
            points = AppendTransformToVector3(points, intersect2Points.points);
            if (intersectLerp1 > 1)
            {
                points = AppendTransformToVector3(points, intersect2Points.aboveIntersectPoints);
            }
            else if (intersectLerp1 < 0)
            {
                points = AppendTransformToVector3(points, intersect2Points.belowIntersectPoints);
            }
        }
        else
        {
            if (UFunc.Sign(intersectLerp1) == UFunc.Sign(intersectLerp2))
            {
                points = AppendTransformToVector3(points, intersectNoSameIntersectDirectionPoints.points);
            }
            else
            {
                points = AppendTransformToVector3(points, intersectNoDiffIntersectDirectionPoints.points);
            }
        }
    }

    List<Vector3> AppendTransformToVector3(List<Vector3> list, Transform[] trans)
    {
        foreach (Transform t in trans)
        {
            list.Add(t.position);
        }
        return list;
    }

    [System.Serializable]
    public class IntersectPoints
    {
        public Transform[] points = new Transform[0];
        public Transform[] aboveIntersectPoints = new Transform[0];
        public Transform[] belowIntersectPoints = new Transform[0];
    }
}
