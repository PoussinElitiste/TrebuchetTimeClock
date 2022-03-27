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
        // v1 
        public static Vector3[] vertices_v2 =
        {
            // face Z positive
            new Vector3( 0, 0, 0), // 0
            new Vector3( 0, 0, 1), // z
            new Vector3( 0, 1, 0), // y
            new Vector3( 1, 0, 0), // x
        };

        public static int[] faceTriangles_v2 =
        {
            // parcours en sens horlogique -> face caméra
            0, 1, 2, // 0: Face 0-Z-Y
            0, 2, 3, // 1: Face 0-Y-X

            // parcours en sens anti-horlogique -> dos caméra
            3, 2, 1, // 3: Face X-Y-Z
            3, 1, 0 // 2: Face 0-X-Z
        };

        // v2: normalisé avec le parcour d'un cylindre non centré
        public static Vector3[] vertices =
        {   
            // face Z positive
            new Vector3( 0, 0, 0), // 0: origine
            new Vector3( 0, 1, 0), // 1: y
            new Vector3( 1, 0, 0), // 2: x
            new Vector3( 0, 0, 1)  // 3: z
        };

        public static int[] faceTriangles =
        {
            // parcours en sens horlogique -> face caméra
            0, 1, 2, // 0: Face 0-Y-X = base pyramide
            3, 2, 1, // 1: Face Z-X-Y

            // parcours en sens anti-horlogique -> dos caméra
            3, 1, 0, // 2: Face Z-Y-0
            3, 0, 2  // 3: Face Z-0-X
        };

        public static Vector3[] FaceVertices(Vector3 size, Vector3 pos)
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

namespace Game.Run
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class MeshGenerator : MonoBehaviour
    {
        private Mesh mesh;
        private List<Vector3> vertices;
        private List<int> triangles;

        [SerializeField]
        private Vector3 position;
        [SerializeField]
        private Vector3 size = new(1f, 1f, 1f);

        // cylinder
        [Range(3, 10)]
        [SerializeField]
        private int section = 3;
        [SerializeField]
        private int radius = 1;
        [SerializeField]
        private int length = 5;

        public Mesh GeneratedMesh { get { return mesh; } }

        void Start()
        {
            mesh = GetComponent<MeshFilter>().sharedMesh;

            //MakeTetrahedron(size, position);
            //MakeCube(size, position);
            MakeCylinder_v1(radius, section, length);
            //MakeCylinder_v2(1, section, length);
            //CreateShape();
            UpdateMesh();
        }

        private void MakeCube(Vector3 size, Vector3 pos)
        {
            vertices = new List<Vector3>();
            triangles = new List<int>();
            for (int i = 0; i < 6; i++)
            {
                MakeQuadFace(i, size * CubeMeshData.scaleAdjust, pos);
            }
        }

        private void MakeQuadFace(int dir, Vector3 size, Vector3 pos)
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

        private void MakeTetrahedron(Vector3 size, Vector3 pos)
        {
            vertices = new List<Vector3>();
            triangles = new List<int>();
            vertices.AddRange(TetraMeshData.FaceVertices(size, pos));
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

        // Version avec position centrée
        // nbSections : angular divisions
        // lenSegment : distance between each cylinder segment (subdivision)
        // cylSegments: number of cylinder segment (faces includes)
        private void MakeCylinder_v1(int radius, int nbSections, int lenSegment)
        {
            vertices = new List<Vector3>(nbSections);
            triangles = new List<int>();

            // vertices
            int cylSegments = 2;
            float angleSection = Mathf.PI * 2 / nbSections;
            for (int i = 0; i < cylSegments; ++i)
            {
                var offset = i * lenSegment;
                vertices.Add(new Vector3(0f, 0f, offset));
                for (int j = 0; j < nbSections; ++j)
                {
                    float angle = j * angleSection;
                    float x = Mathf.Sin(angle) * radius;
                    float y = Mathf.Cos(angle) * radius;
                    vertices.Add(new Vector3(x, y, offset));
                }
            }

            var segmentloop = vertices.Count / cylSegments;
            // triangles draw face directions
            for (int i = 0; i < cylSegments; ++i)
            {
                var offset = i * (nbSections + 1);
                if (i == 0)
                {
                    for (int j = 0; j <= segmentloop - 2; ++j)
                    {
                        triangles.Add(offset + 0);
                        triangles.Add(offset + j + 1);
                        triangles.Add((j < segmentloop - 2) ? offset + j + 2 : offset + 1);
                    }
                }
                else if (i == cylSegments - 1)
                {
                    for (int j = segmentloop; j >= 2; --j)
                    {
                        triangles.Add(offset + 0);
                        triangles.Add(offset + j - 1);
                        triangles.Add((j > 2) ? offset + j - 2 : offset + segmentloop - 1);
                    }
                }
            }

            // Draw Quad between faces (only pair value)
            if ((vertices.Count % 2 == 0) && (segmentloop == nbSections + 1))
            {
                for (int i = 0; i < nbSections; ++i)
                {
                    int noCenterPos = i + 1;
                    // generate temporary quad coordinates between vertices faces
                    List<int> quadIndex = new()
                    {
                        noCenterPos,
                        segmentloop + noCenterPos,
                        (segmentloop + noCenterPos + 1 == vertices.Count) ? segmentloop + 1 : segmentloop + noCenterPos + 1,
                        (noCenterPos + 1 == segmentloop) ? 1 : noCenterPos + 1
                    };

                    DrawQuadTriangles(quadIndex);
                }
            }
            else
            {
                // TODO: pyramidal case (like tetrahedron)
            }
        }

        // nbSections : angular divisions
        // lenSegment : distance between each cylinder segment (subdivision)
        // cylSegments: number of cylinder segment (faces includes)
        private void MakeCylinder_v2(int radius, int nbSections, int lenSegment)
        {
            vertices = new List<Vector3>(nbSections);
            triangles = new List<int>();

            // vertices positions
            int cylSegments = 2;
            float angleSection = Mathf.PI * 2 / nbSections;
            for (int i = 0; i < cylSegments; ++i)
            {
                for (int j = 0; j < nbSections; ++j)
                {
                    float angle = j * angleSection;
                    float x = Mathf.Sin(angle) * radius;
                    float y = Mathf.Cos(angle) * radius;
                    vertices.Add(new Vector3(x, y, i * lenSegment));
                }
            }

            var segmentloop = vertices.Count / cylSegments;
            // triangles draw face directions
            for (int i = 0; i < cylSegments; ++i)
            {
                var offset = i * segmentloop;
                // draw front camera face -> clockwork positive
                if (i == 0)
                {
                    for (int j = 0; j < segmentloop - 2; ++j)
                    {
                        triangles.Add(offset + 0);
                        triangles.Add(offset + j + 1);
                        triangles.Add(offset + j + 2);
                    }
                }
                // draw back camera face -> clockwork negative
                else if (i == cylSegments - 1)
                {
                    for (int j = segmentloop; j > 2; --j)
                    {
                        triangles.Add(offset + j - 1);
                        triangles.Add(offset + j - 2);
                        triangles.Add(offset + 0);
                    }
                }
            }

            // Draw Quad between faces (only pair value)
            if ((vertices.Count % 2 == 0) && (segmentloop == nbSections))
            {
                for (int i = 0; i < segmentloop; ++i)
                {
                    // generate temporary quad coordinates between vertices faces
                    List<int> quadIndex = new()
                    {
                        i,
                        segmentloop + i,
                        (segmentloop + i + 1 == vertices.Count) ? segmentloop : segmentloop + i + 1,
                        (i + 1 == segmentloop) ? 0 : i + 1
                    };

                    DrawQuadTriangles(quadIndex);
                }
            }
            else
            {
                // TODO: pyramidal case (like tetrahedron)
            }
        }

        private void DrawQuadTriangles(List<int> quadIndex)
        {
            // Apply generic quad draw face directions
            // Quad 1 -> {0,1,2}
            triangles.Add(quadIndex[0]);
            triangles.Add(quadIndex[1]);
            triangles.Add(quadIndex[2]);
            // Quad 2 -> {0,2,3}
            triangles.Add(quadIndex[0]);
            triangles.Add(quadIndex[2]);
            triangles.Add(quadIndex[3]);
        }

        private void UpdateMesh()
        {
            if (mesh == null)
                mesh = GetComponent<MeshFilter>().mesh;
            mesh.Clear();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
        }

        public bool GenerateTetrahedron()
        {
            if (mesh == null)
                mesh = GetComponent<MeshFilter>().sharedMesh;
            MakeTetrahedron(size, position);
            UpdateMesh();

            return true;
        }

        internal bool ClearModel()
        {
            if (mesh == null)
                mesh = GetComponent<MeshFilter>().sharedMesh;
            mesh.Clear();

            return true;
        }
    }
}
