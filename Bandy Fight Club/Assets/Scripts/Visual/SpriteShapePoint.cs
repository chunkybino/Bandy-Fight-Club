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
    public bool onlyDoInLine1Bounds;
    public bool onlyDoInLine2Bounds;
    public bool flipIntersectionPointOrder;
    public bool movePastIntersection;
    public Transform movePastIntersectionPosPoint;
    public Transform[] overIntersectBoundPoints = new Transform[0];
    public SpriteShapePoint[] overIntersectBoundDisablePoints = new SpriteShapePoint[0];
    public float slopeSimularRange = 0.05f;

    public bool doSecondLine;
    public Vector3[] secondLinePoints = new Vector3[2];

    public bool dontDoForFrame;

    void Update()
    {
        points = new List<Vector3>() {transform.position};
        doSecondLine = false;

        if (doByIntersection)
        {
            bool notNull = true;
            if (!intersectionInnerPoint1) notNull = false;
            if (!intersectionInnerPoint2) notNull = false;
            if (!intersectionOuterPoint1) notNull = false;
            if (!intersectionOuterPoint2) notNull = false;

            if (notNull)
            {
                points.Clear();
                DoIntersectionPoint();
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

        bool inLine1Bounds = UFunc.IsBetweenVector2(intersect, intersectionOuterPoint1.position, intersectionInnerPoint1.position);
        float intersectLerp1 = UFunc.InverseLerpUnclamped(intersectionInnerPoint1.position.x, intersectionOuterPoint1.position.x, intersect.x);
        bool inLine2Bounds = UFunc.IsBetweenVector2(intersect, intersectionOuterPoint2.position, intersectionInnerPoint2.position);
        float intersectLerp2 = UFunc.InverseLerpUnclamped(intersectionInnerPoint2.position.x, intersectionOuterPoint2.position.x, intersect.x);

        bool shouldDo = true;
        if (!UFunc.Between01(intersectLerp1) && onlyDoInLine1Bounds) shouldDo = false;
        if (!UFunc.Between01(intersectLerp2) && onlyDoInLine2Bounds) shouldDo = false;

        float slopeDiff = UFunc.SlopeDifference(slope1,slope2);
        bool simularSlope = slopeDiff < slopeSimularRange;
        if (simularSlope) shouldDo = false;

        else if (shouldDo)
        {
            Vector3 pos = intersect;
            transform.position = pos;
            points = new List<Vector3>() {transform.position};

            if (movePastIntersection)
            {
                doSecondLine = true;

                Vector3 inPos = intersectionInnerPoint1.position;
                Vector3 outPos = intersectionOuterPoint1.position;
                float movePastFactor = Mathf.InverseLerp(inPos.x, outPos.x, intersect.x);

                Vector3 movePastPos = UFunc.LerpVector3(inPos, pos, 1-movePastFactor);

                secondLinePoints[0] = pos;
                secondLinePoints[1] = movePastPos;
                if (movePastIntersectionPosPoint) movePastIntersectionPosPoint.position = movePastPos;
            }
        }
        else
        {
            bool overIntersectBounds = intersectLerp2 >= 1;

            if (!overIntersectBounds || simularSlope)
            {
                Vector2 dis3 = intersectionInnerPoint2.position-intersectionInnerPoint1.position;
                float slope3 = dis3.y/dis3.x;

                bool right1 = false;
                int sign1Flip = 0;
                if (dis1.x < 0) sign1Flip++;
                if (dis3.x < 0) sign1Flip++;

                if (sign1Flip != 1){
                    right1 = slope1 > slope3;
                } else {
                    right1 = slope1 < slope3;
                }

                bool right2 = false;
                int sign2Flip = 0;
                if (dis2.x < 0) sign2Flip++;
                if (dis3.x < 0) sign2Flip++;

                if (sign2Flip != 1){
                    right2 = slope2 > slope3;
                } else {
                    right2 = slope2 < slope3;
                }

                if (right1 == right2)
                {
                    points.Add(intersectionInnerPoint2.position);
                }
                else
                {
                    points.Add(intersectionInnerPoint1.position);
                    points.Add(intersectionInnerPoint2.position);
                }
            }
            else
            {
                points.Clear();
                foreach (Transform p in overIntersectBoundPoints)
                {
                    points.Add(p.position);
                }
                foreach (SpriteShapePoint p in overIntersectBoundDisablePoints)
                {
                    p.dontDoForFrame = true;
                }
            }
        }

        if (flipIntersectionPointOrder)
        {
            points = UFunc.FlipList(points);
        }
    }
}
