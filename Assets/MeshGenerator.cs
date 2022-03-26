using System.Collections.Generic;
using UnityEngine;
using Game.Data;

namespace Game.Data
{
    public static class CubeMeshData
    {
        public const float scaleAdjust = 0.5f; // vertices coordinate made Cube twice size
        public static Vector3[] vertices =
        {
            // face Z positive
            new Vector3( 1, 1, 1),
            new Vector3(-1, 1, 1),
            new Vector3(-1,-1, 1),
            new Vector3( 1,-1, 1),

            // face Z negative
            new Vector3(-1, 1,-1),
            new Vector3( 1, 1,-1),
            new Vector3( 1,-1,-1),
            new Vector3(-1,-1,-1)
        };

        public static int[][] faceTriangles =
        {
            // parcours en sens horlogique
            new int[] { 0, 1, 2, 3}, // 0: Face N
            new int[] { 5, 0, 3, 6}, // 1: Face E
            new int[] { 4, 5, 6, 7}, // 2: Face S
            new int[] { 1, 4, 7, 2}, // 3: Face W
            new int[] { 5, 4, 1, 0}, // 4: Face Up
            new int[] { 3, 2, 7, 6}  // 5: Face Dwn
        };
    
        public static Vector3[] faceVertices(int dir, Vector3 size, Vector3 pos)
        {
            Vector3[] fv = new Vector3[4];
            for (int i = 0; i < fv.Length; ++i)
            {
                Vector3 v = vertices[faceTriangles[dir][i]];
                fv[i].x = v.x * size.x;
                fv[i].y = v.y * size.y;
                fv[i].z = v.z * size.z;
                fv[i] += pos;
            }

            return fv;
        }
    }

    public static class TetraMeshData
    {
        public static Vector3[] vertices =
        {
            // face Z positive
            new Vector3( 0, 0, 0), // 0
            new Vector3( 0, 0, 1), // z
            new Vector3( 0, 1, 0), // y
            new Vector3( 1, 0, 0), // x
        };

        public static int[] faceTriangles =
        {
            // parcours en sens horlogique
            0, 1, 2, // 0: Face 0-Z-Y
            0, 2, 3, // 1: Face 0-Y-X
            0, 3, 1, // 2: Face 0-X-Z
            3, 2, 1  // 3: Face X-Y-Z
        };

        public static Vector3[] faceVertices(Vector3 size, Vector3 pos)
        {
            Vector3[] fv = new Vector3[vertices.Length];
            for (int i = 0; i < fv.Length; ++i)
            {
                fv[i].x = vertices[i].x * size.x;
                fv[i].y = vertices[i].y * size.y;
                fv[i].z = vertices[i].z * size.z;
                fv[i] += pos;
            }

            return fv;
        }
    }
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;

    public Vector3 position;
    public Vector3 size = new Vector3(1f,1f,1f);

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;

        MakeCube(size, position);
        //MakeTetrahedron(size, position);
        //CreateShape();
        UpdateMesh();
    }

    void MakeCube(Vector3 size, Vector3 pos)
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        for (int i = 0; i < 6; i++)
        {
            MakeQuadFace(i, size * CubeMeshData.scaleAdjust, pos);
        }
    }

    void MakeQuadFace(int dir, Vector3 size, Vector3 pos)
    {
        vertices.AddRange(CubeMeshData.faceVertices(dir, size, pos));
        int vCount = vertices.Count;
        // first triangle quad
        triangles.Add(vCount - 4 + 0);
        triangles.Add(vCount - 4 + 1);
        triangles.Add(vCount - 4 + 2);
        // second triangle quad
        triangles.Add(vCount - 4 + 0);
        triangles.Add(vCount - 4 + 2);
        triangles.Add(vCount - 4 + 3);
    }

    void MakeTetrahedron(Vector3 size, Vector3 pos)
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        vertices.AddRange(TetraMeshData.faceVertices(size, pos));
        //vertices.AddRange(TetraMeshData.vertices);
        triangles.AddRange(TetraMeshData.faceTriangles);
    }

    private void CreateShape()
    {
        vertices = new List<Vector3>()
        {
            new Vector3(0,0,0),
            new Vector3(0,0,1),
            new Vector3(0,1,0),
            new Vector3(1,0,0)
        };

        triangles = new List<int>()
        {
            0, 1, 2, // 0-Z-Y
            0, 2, 3, // 0-Y-X
            0, 3, 1, // 0-X-Z
            3, 2, 1  // X-Y-Z
        };
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }
}
