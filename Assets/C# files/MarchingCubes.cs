using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MarchingCubes : MonoBehaviour
{
    [SerializeField] private int width = 30;
    [SerializeField] private int height = 10;

    [SerializeField] float noiseScale = 1;
    [SerializeField] private float heightTresshold = 0.5f;
    [SerializeField] bool use3DNoise;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>(); // Add list for UVs
    private float[,,] heights;

    private MeshFilter meshFilter;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    void Update()
    {
        StartCoroutine(TestAll());
    }

    private IEnumerator TestAll()
    {
        while (true)
        {
            SetHeights();
            MarchCubes();
            SetMesh();
            yield return new WaitForSeconds(1000f);
        }
    }

    private void SetMesh()
    {
        Mesh mesh = new Mesh();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();  // Set UVs on the mesh
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    private void SetHeights()
    {
        heights = new float[width + 1, height + 1, width + 1];

        for (int x = 0; x < width + 1; x++)
        {
            for (int y = 0; y < height + 1; y++)
            {
                for (int z = 0; z < width + 1; z++)
                {
                    float currentHeight = height * Mathf.PerlinNoise(x * noiseScale, z * noiseScale);
                    float distToSufrace;

                    if (y <= currentHeight - 0.5f)
                        distToSufrace = 0f;
                    else if (y > currentHeight + 0.5f)
                        distToSufrace = 1f;
                    else if (y > currentHeight)
                        distToSufrace = y - currentHeight;
                    else
                        distToSufrace = currentHeight - y;

                    heights[x, y, z] = distToSufrace;
                }
            }
        }
    }

    private int GetConfigIndex(float[] cubeCorners)
    {
        int configIndex = 0;

        for (int i = 0; i < 8; i++)
        {
            if (cubeCorners[i] > heightTresshold)
            {
                configIndex |= 1 << i;
            }
        }

        return configIndex;
    }

    private int GetEdgeEndVertex(Vector3 pos)
    {
        for (int i = 0; i < MarchingTable.Corners.Length; i++)
        {
            if (pos == MarchingTable.Corners[i])
            {
                return i;
            }
        }
        return default;
    }

    private void MarchCubes()
    {
        vertices.Clear();
        triangles.Clear();
        uvs.Clear(); // Clear UVs

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    float[] cubeCorners = new float[8];

                    for (int i = 0; i < 8; i++)
                    {
                        Vector3Int corner = new Vector3Int(x, y, z) + MarchingTable.Corners[i];
                        cubeCorners[i] = heights[corner.x, corner.y, corner.z];
                    }

                    MarchCube(new Vector3(x, y, z), cubeCorners);
                }
            }
        }
    }

    private void MarchCube(Vector3 position, float[] cubeCorners)
    {
        int configIndex = GetConfigIndex(cubeCorners);

        if (configIndex == 0 || configIndex == 255)
        {
            return;
        }

        int edgeIndex = 0;
        for (int t = 0; t < 5; t++)
        {
            for (int v = 0; v < 3; v++)
            {
                int triTableValue = MarchingTable.Triangles[configIndex, edgeIndex];

                if (triTableValue == -1)
                {
                    return;
                }

                Vector3 edgeStart = position + MarchingTable.Edges[triTableValue, 0];
                Vector3 edgeEnd = position + MarchingTable.Edges[triTableValue, 1];

                Vector3 vertex = Vector3.Lerp(edgeStart, edgeEnd, (heightTresshold - cubeCorners[GetEdgeEndVertex(MarchingTable.Edges[triTableValue, 0])]) /
                    (cubeCorners[GetEdgeEndVertex(MarchingTable.Edges[triTableValue, 1])] - cubeCorners[GetEdgeEndVertex(MarchingTable.Edges[triTableValue, 0])]));

                vertices.Add(vertex);
                triangles.Add(vertices.Count - 1);

                // Calculate UV based on vertex position
                Vector2 uv = new Vector2(vertex.x / width, vertex.z / width); // Normalizing the x and z position
                uvs.Add(uv);

                edgeIndex++;
            }
        }
    }
}
