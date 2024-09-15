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

    int[] tris = new int[0];

    public bool doWorldSpace;

    public Vector2 verticeRotationVec;

    public Vector2 uvRotation;
    public int[] uvRotationVerticeReferences = new int[2];

    void Update()
    {
        mesh = meshFilter.sharedMesh;

        //verticies
        if (doVerticeReferences)
        {
            vertices = new Vector3[verticeReferences.Length];
            for (int i = 0; i < verticeReferences.Length; i++)
            {
                Vector3 offset = new Vector3(0,0,-0.1f);
                if (doWorldSpace) offset -= transform.position;
                vertices[i] = verticeReferences[i].position + offset;
                //Vector2 rotVec = UFunc.RotateVecByVec(vertices[i], verticeRotationVec);
                //vertices[i] = new Vector3(rotVec.x,rotVec.y,vertices[i].z);
            }
        }

        //uv
        if (UV.Length != vertices.Length) UV = new Vector2[vertices.Length];
        Vector3 vectice1 = vertices[0];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 vertice = vertices[i] - vectice1;//-transform.position;//-vertices[uvOffsetPivot]
            UV[i] = new Vector2(vertice.y, vertice.x); 
        }
        //rotate uv
        if (vertices.Length >= 2 && uvRotationVerticeReferences.Length >= 2)
        {
            uvRotation = vertices[uvRotationVerticeReferences[0]] - vertices[uvRotationVerticeReferences[1]];
        }
        for (int i = 0; i < UV.Length; i++)
        {
            UV[i] = UFunc.RotateVecByVec(UV[i], uvRotation);
        }

        //tris
        if (tris.Length != triangles.Length*3) tris = new int[triangles.Length * 3];
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
