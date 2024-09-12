using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[ExecuteInEditMode]
public class SpriteShapeMapper : MonoBehaviour
{
    //public Vector3[] points;
    public Transform[] points;
    public SpriteShapeController shape;

    void Update()
    {
        Spline spline = shape.spline;

        spline.Clear();
        for (int i = 0; i < points.Length; i++)
        {
            spline.InsertPointAt(i, points[i].position);
        }
        shape.RefreshSpriteShape();
    }
}
