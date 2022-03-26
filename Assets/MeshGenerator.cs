using System.Collections.Generic;
using UnityEngine;
using Game.Data;

namespace Game.Data
{
    public static class CubeMeshData
    {
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
    
        public static Vector3[] faceVertices(int dir, float scale, Vector3 pos)
        {
            Vector3[] fv = new Vector3[4];
            for (int i = 0; i < fv.Length; ++i)
                fv[i] = (vertices[faceTriangles[dir][i]] * scale) + pos;

            return fv;
        }
    }

    public static class TetraMeshData
    {
        public static Vector3[] vertices =
        {
            // face Z positive
            new Vector3( 0, 0, 0), // 0
            new Vector3( 0, 1, 0), // y
            new Vector3( 0, 0, 1), // z
            new Vector3( 1, 0, 0), // x
        };

        public static int[][] faceTriangles =
        {
            // parcours en sens horlogique
            new int[] { 0, 1, 2}, // 0: Face 0-z-y
            new int[] { 0, 1, 3}, // 1: Face 0-z-x
            new int[] { 2, 3, 1}, // 2: Face 0-y-x
            new int[] { 1, 2, 3}, // 3: Face x-y-z
        };

        public static Vector3[] faceVertices(int dir, float scale, Vector3 pos)
        {
            Vector3[] fv = new Vector3[3];
            for (int i = 0; i < fv.Length; ++i)
                fv[i] = (vertices[faceTriangles[dir][i]] * scale) + pos;

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

    public float scale = 1f;
    private float adjScale = 1f;
    public int posX, posY, posZ;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        adjScale = scale * 0.5f;

        //MakeCube(adjScale, new Vector3(posX*adjScale, posY * adjScale, posZ * adjScale));
        MakeTetrahedron(adjScale, new Vector3(posX*adjScale, posY * adjScale, posZ * adjScale));
        UpdateMesh();
    }

    void MakeCube(float cubeScale, Vector3 cubPos)
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        for (int i = 0; i < 6; i++)
        {
            MakeQuadFace(i, cubeScale, cubPos);
        }
    }

    void MakeQuadFace(int dir, float faceScale, Vector3 facePos)
    {
        vertices.AddRange(CubeMeshData.faceVertices(dir, faceScale, facePos));
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

    void MakeTetrahedron(float tetraScale, Vector3 cubPos)
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        for (int i = 0; i < 4; ++i)
        {
            MakeTriangleFace(i, tetraScale, cubPos);
        }
    }

    void MakeTriangleFace(int dir, float faceScale, Vector3 facePos)
    {
        vertices.AddRange(TetraMeshData.faceVertices(dir, faceScale, facePos));
        triangles.AddRange(TetraMeshData.faceTriangles[dir]);
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }
}
