using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShineLiner : MonoBehaviour
{
    [SerializeField] LineRenderer renderLine;
    [SerializeField] LineRenderer referenceLine;

    [SerializeField] float offset;
    [SerializeField] bool flipEndTangents;

    [SerializeField] float zOffset = 0;

    [SerializeField] bool limitIndexesUsed;
    //[SerializeField] int[] limitIndexes;
    [SerializeField] int startClamp; //limit index, start this many indexes after the start
    [SerializeField] int endClamp; //end, this many points from the end

    Vector3[] referencePoints;
    Vector2[] referenceLineAngles;
    Vector2[] referenceTangents;
    Vector3[] tangentOffsetPositions; 

    void Update()
    {
        referencePoints = new Vector3[referenceLine.positionCount];
        referenceLine.GetPositions(referencePoints);

        if (referencePoints.Length <= 1) return;

        referenceLineAngles = new Vector2[referencePoints.Length-1]; 
        for (int i = 0; i < referenceLineAngles.Length; i++)
        {
            referenceLineAngles[i] = referencePoints[i+1] - referencePoints[i];
            referenceLineAngles[i] = referenceLineAngles[i].normalized;
        }

        referenceTangents = new Vector2[referencePoints.Length]; 
        for (int i = 0; i < referenceTangents.Length; i++)
        {
            if (i == 0 || i == referenceTangents.Length-1) //edge points
            {
                int flip = 1;
                if (flipEndTangents) flip = -1;
                if (i == 0) referenceTangents[i] = new Vector2(-referenceLineAngles[i].y,referenceLineAngles[i].x) * flip;
                if (i == referenceTangents.Length-1) referenceTangents[i] = new Vector2(-referenceLineAngles[referenceTangents.Length-2].y,referenceLineAngles[referenceTangents.Length-2].x) * flip;
            }
            else 
            {
                referenceTangents[i] = referenceLineAngles[i]-referenceLineAngles[i-1];
                referenceTangents[i] = referenceTangents[i].normalized;
            }
        }

        tangentOffsetPositions = new Vector3[referenceTangents.Length];
        for (int i = 0; i < tangentOffsetPositions.Length; i++)
        {
            tangentOffsetPositions[i] = referencePoints[i] + new Vector3(referenceTangents[i].x*offset,referenceTangents[i].y*offset,zOffset);
        }

        Vector3[] vecList = tangentOffsetPositions;
        if (limitIndexesUsed)
        {
            int length = vecList.Length - startClamp - endClamp;
            if (length < 0) return;
            vecList = new Vector3[length];
            for (int i = 0; i < vecList.Length; i++)
            {
                vecList[i] = tangentOffsetPositions[i+startClamp];
            }
        }

        renderLine.positionCount = vecList.Length;
        renderLine.SetPositions(vecList);
    }
}
