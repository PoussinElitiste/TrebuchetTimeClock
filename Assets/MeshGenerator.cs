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

    public GameObject cylinder;

    public Vector3 position;
    public Vector3 size = new Vector3(1f,1f,1f);

    // cylinder
    [Range(3, 10)]
    public int iter = 3;
    int num = 0;
    public int leng = 5;


    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;

        //MakeCube(size, position);
        MakeCylinder(1, iter, leng);
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

    void MakeCylinder(int radius, int nbSections, int length)
    {
        vertices = new List<Vector3>(nbSections);
        triangles = new List<int>();

        // vertices
        int segment = 2;
        float angleSection = Mathf.PI * 2 / nbSections;
        for (int i = 0;i < segment; ++i)
        {
            var offset = i * (nbSections);
            vertices.Add(new Vector3(0f,0f,offset));
            for (int j = 0; j < nbSections; ++j)
            {
                float angle = j * angleSection;
                float x = Mathf.Sin(angle) * radius;
                float y = Mathf.Cos(angle) * radius;
                vertices.Add(new Vector3(x, y, i * length));
            }
        }

        // triangle
        for (int i = 0; i < segment ; ++i)
        {
            var loop = vertices.Count / segment;
            var offset = i * (nbSections + 1);
            for (int j = 0; j <= loop-2; ++j) 
            {
                triangles.Add(offset + 0 );
                triangles.Add(offset + j + 1);
                if (j < loop-2)
                    triangles.Add(offset + j + 2);
                else
                    triangles.Add(offset + 1);
            }
        }
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }
}
