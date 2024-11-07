using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShineLiner : MonoBehaviour
{
    [SerializeField] LineRenderer renderLine;
    [SerializeField] LineRenderer referenceLine;

    [SerializeField] float offset;

    [SerializeField] bool limitIndexesUsed;
    [SerializeField] int[] limitIndexes;

    [SerializeField] Vector3[] referencePoints;
    [SerializeField] Vector2[] referenceLineAngles;
    [SerializeField] Vector2[] referenceTangents;
    [SerializeField] Vector3[] tangentOffsetPositions; 

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
                if (i == 0) referenceTangents[i] = new Vector2(-referenceLineAngles[i].y,referenceLineAngles[i].x);
                if (i == referenceTangents.Length-1) referenceTangents[i] = new Vector2(-referenceLineAngles[referenceTangents.Length-2].y,referenceLineAngles[referenceTangents.Length-2].x);
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
            tangentOffsetPositions[i] = referencePoints[i] + new Vector3(referenceTangents[i].x,referenceTangents[i].y,0)*offset;
        }

        Vector3[] vecList = tangentOffsetPositions;
        if (limitIndexesUsed)
        {
            vecList = new Vector3[limitIndexes.Length];
            for (int i = 0; i < vecList.Length; i++)
            {
                vecList[i] = tangentOffsetPositions[limitIndexes[i]];
            }
        }

        renderLine.positionCount = vecList.Length;
        renderLine.SetPositions(vecList);
    }
}
