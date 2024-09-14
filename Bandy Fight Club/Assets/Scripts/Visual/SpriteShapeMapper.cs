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
            if (point.doSecondLine) secondLinePoints.Add(point);
        }

        Vector3[] lineArray = UFunc.ListToArray(linePoints);
        line.positionCount = lineArray.Length;
        line.SetPositions(lineArray);

        DoSecondLines(secondLinePoints);

        void DoSecondLines(List<SpriteShapePoint> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (i >= childLines.Count)
                {
                    childLines.Add(Instantiate(line, this.transform));
                }
                childLines[i].positionCount = list[i].secondLinePoints.Length;
                childLines[i].SetPositions(list[i].secondLinePoints);
                childLines[i].gameObject.SetActive(true);
            }
            for (int i = list.Count; i < childLines.Count; i++) //disabl extra lines
            {
                childLines[i].gameObject.SetActive(false);
            }
        }
    }
}