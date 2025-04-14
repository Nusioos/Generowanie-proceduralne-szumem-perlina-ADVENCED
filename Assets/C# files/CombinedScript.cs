using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CombinedScript : MonoBehaviour
{
    [SerializeField]
    public Transform cube_pos;
    [SerializeField]
    private Transform player;
    [SerializeField]
    public Vector3 pozycja;
    [SerializeField]
    private bool isspawned = false;

    private float distance;
    private MeshFilter spawnedSimba;

    Mesh mesh;
    [SerializeField, Range(1, 1000)]
    private int resolution = 10;
    [SerializeField, Range(0, 1)]
    private float czynnik = 0.05f;
    private Vector3[] vertices;

    public List<MeshFilter> cubes = new List<MeshFilter>();
    public List<MeshFilter> sourceFilter = new List<MeshFilter>();
    public MeshFilter fast;
    [SerializeField] private MeshFilter targetFilter;

    [SerializeField]
    public bool hasInstantiated = false;
    public int w = 0;
    public float offset_X;
    public float offset_Y;
    public float offset_Z;
    private float offset_X_Random;
    private float offset_Y_Random;
    private float offset_Z_Random;
    public Material combinedMeshMaterial;
    Vector3 simba;

    private void Awake()
    {
        if (!hasInstantiated)
        {
            offset_X_Random = Random.Range(0f, 1000f);
            offset_Y_Random = Random.Range(0f, 1000f);
            offset_Z_Random = Random.Range(0f, 1000f);

            Generate();
            hasInstantiated = true;
        }
    }

    private void Start()
    {
        // Optionally initialize pozycja here if needed
    }

    private void Update()
    {
        /*pozycja = player.position;
        distance = Vector3.Distance(player.position, cube_pos.position);

        if (distance < 100 && !isspawned)
        {
            //spawnedSimba = Instantiate(targetFilter, cube_pos.position, Quaternion.identity);
            isspawned = true;
        }
        else if (distance > 100 && isspawned)
        {
            if (spawnedSimba != null)
            {
                spawnedSimba.gameObject.SetActive(false);
            }
            isspawned = false;
        }
        else if (distance < 100 && isspawned)
        {
            if (spawnedSimba != null)
            {
                spawnedSimba.gameObject.SetActive(true);
            }
        }*/

        // Optionally update cube_pos here if needed
        // For example:
        // cube_pos.position = new Vector3(pozycja.x + someOffsetX, pozycja.y + someOffsetY, pozycja.z + someOffsetZ);
    }

    private void Generate()
    {
        float noiseScale = Random.Range(0.12f, 0.12f);
        List<CombineInstance> combineList = new List<CombineInstance>();

        offset_X = pozycja.x;  // Ensure you're using the correct position
        offset_Z = pozycja.z;  // Ensure you're using the correct position

        for (int x = 0; x < resolution * 2; x++)
        {
            for (int z = 0; z < resolution * 2; z++)
            {
                for (int y = 0; y < resolution / 3; y++)
                {
                    if (Perlin3D(x * noiseScale, y * noiseScale, z * noiseScale, offset_X, offset_Z) > 0.5f)
                    {
                        Vector3 pos = new Vector3(x, y, z);
                        MeshFilter ws = Instantiate(fast, pos, Quaternion.identity, transform);

                        if (ws.mesh != null)
                        {
                            sourceFilter.Add(ws);

                            CombineInstance ci = new CombineInstance
                            {
                                mesh = ws.mesh,
                                transform = ws.transform.localToWorldMatrix
                            };
                            combineList.Add(ci);
                            Destroy(ws.gameObject);

                            if (combineList.Sum(c => c.mesh.vertexCount) > 50000)
                            {
                                CreateNewMeshObject(combineList.ToArray());
                                combineList.Clear();
                            }
                        }
                    }
                }
            }
        }

        if (combineList.Count > 0)
        {
            CreateNewMeshObject(combineList.ToArray());
        }
    }

    private void CreateNewMeshObject(CombineInstance[] combineInstances)
    {
        Mesh newMesh = new Mesh();
        newMesh.CombineMeshes(combineInstances, true, true);

        if (newMesh.vertexCount > 0)
        {
            MeshFilter newCube = Instantiate(fast, transform);
            newCube.mesh = newMesh;

            MeshRenderer meshRenderer = newCube.gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null && combinedMeshMaterial != null)
            {
                meshRenderer.material = combinedMeshMaterial;
            }

            cubes.Add(newCube);
        }
    }

    public static float Perlin3D(float x, float y, float z, float offset, float offset_z)
    {
        float ab = Mathf.PerlinNoise(x + offset, y);
        float bc = Mathf.PerlinNoise(y, z + offset_z);
        float ac = Mathf.PerlinNoise(x + offset, z + offset_z);

        float ba = Mathf.PerlinNoise(y, x + offset);
        float cb = Mathf.PerlinNoise(z + offset_z, y);
        float ca = Mathf.PerlinNoise(z + offset_z, x + offset);

        float abc = ab + bc + ac + ba + cb + ca;
        return abc / 6f;
    }
}
