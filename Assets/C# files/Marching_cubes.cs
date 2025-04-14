using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Marching_cubes : MonoBehaviour
{
    [SerializeField] private int width = 30;
    [SerializeField] private int height = 10;
    [SerializeField] private float noiseScale = 0.1f;
    [SerializeField] private float heightThreshold = 0.5f;
    [SerializeField] private bool visualizeNoise;
    [SerializeField] private bool use3DNoise;
    [SerializeField] private GameObject kelpPrefab; // Kelp prefab assigned in the Unity editor
    [SerializeField] private GameObject rockprefab;
    [SerializeField] private GameObject grassprefab;
    [SerializeField] private GameObject coralprefab;
    [SerializeField] private float kelpSpawnProbability = 0.01f; // Probability of spawning kelp
    [SerializeField] private float rockSpawnProbability = 0.03f;
    [SerializeField] private float grassSpawnProbability = 0.5f;
    [SerializeField] private float coralSpawnProbability = 0.5f;
    private List<Vector3> spawnedPositions = new List<Vector3>(); // Use a list to store the spawned positions
    [SerializeField] private float minSpawnDistance = 2f; // Minimum distance between spawned objects
    [SerializeField] private Transform chunkTransform;
    [SerializeField] private List<SpawnedObject> spawnedObjects = new List<SpawnedObject>();
    //private HashSet<Vector3> spawnedPositions = new HashSet<Vector3>();
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();
    private float[,,] heights;
    public float rands;
    public float genos;
    public float sigma;
    private MeshFilter meshFilter;
    public GameObject serbia;
    [System.Serializable]
    public class SpawnedObject
    {
        public Vector3 Position;
        public GameObject GameObject;
    }

    void OnEnable ()
    {
        // Start the coroutine to generate the mesh gradually
        StartCoroutine(GenerateMeshGradually());
    }

    private void Awake()
    {
       
        meshFilter = GetComponent<MeshFilter>();
        rands = serbia.transform.position.x * noiseScale;
        genos = serbia.transform.position.z * noiseScale;
        sigma = serbia.transform.position.y * noiseScale;
        SetHeights();
      //  StartCoroutine(GenerateMeshGradually());
        // Remove the direct call to MarchCubes here
        // MarchCubes(); 
        // SetMesh(); // This will now be called inside the coroutine after all cubes are marched
        // SpawnKelp(); // This will also be called at the end of the coroutine
    }
    void Update()
    {
        List<SpawnedObject> toRemove = new List<SpawnedObject>();

        foreach (SpawnedObject spawned in spawnedObjects)
        {
            if (spawned.GameObject == null) // Check if the GameObject reference is null (destroyed)
            {
                toRemove.Add(spawned);
            }
        }

        foreach (SpawnedObject destroyed in toRemove)
        {
            spawnedObjects.Remove(destroyed);
            spawnedPositions.Remove(destroyed.Position); // Optionally remove from positions if needed
        }
    }
    private void SetMesh()
    {
        if (vertices.Count == 0 || triangles.Count == 0) return;

        Mesh mesh = new Mesh
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray(),
            uv = GenerateUVs()
        };

        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    private Vector2[] GenerateUVs()
    {
        Vector2[] uvs = new Vector2[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 vertex = vertices[i];
            uvs[i] = new Vector2(
                (vertex.x / width) * 0.5f + 0.5f,
                (vertex.z / width) * 0.5f + 0.5f
            );
        }
        return uvs;
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
                    if (y == height)
                    {
                        heights[x, y, z] = heightThreshold + 1;
                    }
                    else if (use3DNoise)
                    {
                        float currentHeight = PerlinNoise3D(x * noiseScale + rands, y * noiseScale + sigma, z * noiseScale + genos);
                        heights[x, y, z] = currentHeight;
                    }
                    else
                    {
                        float currentHeight = height * Mathf.PerlinNoise(x * noiseScale + rands, z * noiseScale + genos);
                        float distToSurface = Mathf.Abs(currentHeight - y * noiseScale);
                        heights[x, y, z] = distToSurface;
                    }
                }
            }
        }
    }

    private float PerlinNoise3D(float x, float y, float z)
    {
        float xy = Mathf.PerlinNoise(x, y);
        float xz = Mathf.PerlinNoise(x, z);
        float yz = Mathf.PerlinNoise(y, z);

        float yx = Mathf.PerlinNoise(y, x);
        float zx = Mathf.PerlinNoise(z, x);
        float zy = Mathf.PerlinNoise(z, y);

        return (xy + xz + yz + yx + zx + zy) / 6;
    }

    private int GetConfigIndex(float[] cubeCorners)
    {
        int configIndex = 0;
        for (int i = 0; i < 8; i++)
        {
            if (cubeCorners[i] > heightThreshold)
            {
                configIndex |= 1 << i;
            }
        }
        return configIndex;
    }

    private IEnumerator GenerateMeshGradually()
    {
        vertices.Clear();
        triangles.Clear();

        int cubesProcessed = 0; // Keep track of processed cubes
        const int cubesPerFrame = 100; // Number of cubes to process before yielding

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

                    cubesProcessed++;

                    // Yield control back to the Unity engine after processing a set number of cubes
                    if (cubesProcessed >= cubesPerFrame/1.421)
                    {
                        cubesProcessed = 0; // Reset counter
                        yield return null; // Yield after processing `cubesPerFrame` cubes
                    }
                }
            }
        }

        // After generating all cubes, update the mesh and spawn objects
        SetMesh();
        SpawnKelp(chunkTransform);
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

                Vector3 vertex = Vector3.Lerp(edgeStart, edgeEnd,
                    (heightThreshold - cubeCorners[GetEdgeEndVertex(MarchingTable.Edges[triTableValue, 0])]) /
                    (cubeCorners[GetEdgeEndVertex(MarchingTable.Edges[triTableValue, 1])] -
                     cubeCorners[GetEdgeEndVertex(MarchingTable.Edges[triTableValue, 0])]));

                vertices.Add(vertex);
                triangles.Add(vertices.Count - 1);

                edgeIndex++;
            }
        }
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

    private float CalculateSlope(Vector3 normal)
    {
        float cosineTheta = Vector3.Dot(normal.normalized, Vector3.up);
        float slopeAngle = Mathf.Acos(cosineTheta) * Mathf.Rad2Deg;
        return slopeAngle;
    }


    private void SpawnKelp(Transform chunkTransform)
    {
        Mesh mesh = meshFilter.mesh;
        Vector3[] normals = mesh.normals;
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPosition = transform.TransformPoint(vertices[i]);

            Vector3 roundedPosition = new Vector3(
                Mathf.Round(worldPosition.x * 10) / 10f,
                Mathf.Round(worldPosition.y * 10) / 10f,
                Mathf.Round(worldPosition.z * 10) / 10f
            );

            bool tooClose = false;
            foreach (SpawnedObject spawned in spawnedObjects)
            {
                if (Vector3.Distance(spawned.Position, roundedPosition) < minSpawnDistance)
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose) continue;

            float slope = CalculateSlope(normals[i]);
            GameObject spawnedObject = null;

            if (slope <= 60f)
            {
                float hash = HashCode.Combine(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.y), Mathf.RoundToInt(worldPosition.z));
                float hashNormalized = (hash % 10000) / 10000f;

                if (hashNormalized < kelpSpawnProbability)
                {
                    Vector3 kelpPlace = new Vector3(worldPosition.x, worldPosition.y + 14f, worldPosition.z);
                    Vector3 rotacjaKelp = new Vector3(90f + slope, 0, 0);
                    spawnedObject = Instantiate(kelpPrefab, kelpPlace, Quaternion.AngleAxis(270f, rotacjaKelp), chunkTransform);
                }
                else if (hashNormalized < kelpSpawnProbability - rockSpawnProbability)
                {
                    Vector3 rotacjaRock = new Vector3(hashNormalized, hashNormalized, hashNormalized);
                    spawnedObject = Instantiate(rockprefab, worldPosition, Quaternion.AngleAxis(250f, rotacjaRock), chunkTransform);
                }
                else if (hashNormalized < kelpSpawnProbability - rockSpawnProbability - grassSpawnProbability)
                {
                    Vector3 coralPlace = new Vector3(worldPosition.x, worldPosition.y + 0.1f, worldPosition.z);
                    Vector3 rotacjaCoral = new Vector3(90f + slope, 0, 0);
                    spawnedObject = Instantiate(coralprefab, coralPlace, Quaternion.AngleAxis(270f, rotacjaCoral), chunkTransform);
                }

                if (spawnedObject != null)
                {
                    spawnedPositions.Add(roundedPosition);
                    spawnedObjects.Add(new SpawnedObject { Position = roundedPosition, GameObject = spawnedObject });
                }
            }
        }
    }



}
