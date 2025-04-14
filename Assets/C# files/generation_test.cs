using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class generation_test : MonoBehaviour
{
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

    //public stwarzacz gen;

    [SerializeField]
    public bool hasInstantiated = false;
    public int w = 0;
    public float offset_X;
    public float offset_Y;
    public float offset_Z;
    private float offset_X_Random;
    private float offset_Y_Random;
    private float offset_Z_Random;
    public float trans_z;
    public Material combinedMeshMaterial;
    Vector3 simba;
    public float trans_x;
    public float noiseScale;
    private void Awake()
    {
        
        trans_x = targetFilter.transform.position.x*noiseScale;
        trans_z= targetFilter.transform.position.z*noiseScale;
       // Debug.Log(rands + "pozycja");
        Generate();
     
    }

    private void Generate()
    {
        float Randoms=Random.Range(3f,3f);
     
        List<CombineInstance> combineList = new List<CombineInstance>();

        for (int x = 0; x < resolution * 2; x++)
        {
            for (int z = 0; z < resolution * 2; z++)
            {
                for (int y = 0; y < resolution / Randoms; y++)
                {
                    if (Perlin3D(x * noiseScale, y * noiseScale, z * noiseScale, trans_x, trans_z) > 0.5f)
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

    private void Update()
    {
    }

    public static float Perlin3D(float x, float y, float z, float offset, float offset_z)
    {
        float ab = Mathf.PerlinNoise(x + offset, y);
        float bc = Mathf.PerlinNoise(y, z + offset_z);
        float ac = Mathf.PerlinNoise(x + offset, z + offset_z);

        float ba = Mathf.PerlinNoise(y, x + offset);
        float cb = Mathf.PerlinNoise(z + offset_z, y);
        float ca = Mathf.PerlinNoise(z + offset_z, x);

        float abc = ab + bc + ac + ba + cb + ca;
        return abc / 6f;
    }
}
