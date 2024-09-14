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
    public float movePastIntersectionFactor; //place a point this much distance between the intersection and one of the innnerpoints
    public bool movePastIntersectionDoOtherPoint; //change the innnerpoit we choose for the above variable

    public bool doSecondLine;
    public Vector3[] secondLinePoints = new Vector3[2];

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
        bool inLine2Bounds = UFunc.IsBetweenVector2(intersect, intersectionOuterPoint2.position, intersectionInnerPoint2.position);

        bool shouldDo = true;
        if (onlyDoInLine1Bounds && !inLine1Bounds) shouldDo = false;
        if (onlyDoInLine2Bounds && !inLine2Bounds) shouldDo = false;

        if (shouldDo)
        {
            Vector3 pos = intersect;
            transform.position = pos;
            points = new List<Vector3>() {transform.position};

            if (movePastIntersection)
            {
                doSecondLine = true;

                Vector3 lerpPos = intersectionInnerPoint1.position;
                if (movePastIntersectionDoOtherPoint) lerpPos = intersectionInnerPoint2.position;
                Vector2 movePastPos = UFunc.LerpVector3(pos, lerpPos, movePastIntersectionFactor);
                //points.Insert(0, movePastPos);
                secondLinePoints[0] = pos;
                secondLinePoints[1] = movePastPos;
            }
        }
        else
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
                points = new List<Vector3>() {intersectionInnerPoint2.position};
            }
            else
            {
                points = new List<Vector3>() {intersectionInnerPoint1.position,intersectionInnerPoint2.position};
            }
        }

        if (flipIntersectionPointOrder)
        {
            points = UFunc.FlipList(points);
        }
    }
}
