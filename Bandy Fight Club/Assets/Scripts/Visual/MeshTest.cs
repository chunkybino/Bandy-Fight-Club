using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MeshTest : MonoBehaviour
{
    public MeshFilter meshFilter;
    public Mesh mesh;

    public bool doVerticeReferences = true;
    public Transform[] verticeReferences;
    public Vector2[] UV;
    public Vector3[] vertices;
    public Triangle[] triangles;

    void Update()
    {
        mesh = meshFilter.mesh;

        if (doVerticeReferences)
        {
            vertices = new Vector3[verticeReferences.Length];
            for (int i = 0; i < verticeReferences.Length; i++)
            {
                vertices[i] = verticeReferences[i].position - transform.position + new Vector3(0,0,-0.1f);
            }
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            UV[i].y = vertices[i].x;
            UV[i].x = 1-vertices[i].y;
        }

        int[] tris = new int[triangles.Length * 3];
        int i2 = 0;
        foreach (Triangle t in triangles)
        {
            tris[i2] = t.p1;
            tris[i2+1] = t.p2;
            tris[i2+2] = t.p3;
            i2 += 3;
        }

        mesh.vertices = vertices;
        mesh.uv = UV;
        mesh.triangles = tris;
    }

    [System.Serializable]
    public struct Triangle
    {
        public int p1;
        public int p2;
        public int p3;
    }
}
