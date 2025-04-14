using System.Collections.Generic;
using UnityEngine;

public class verticy_gen : MonoBehaviour
{
    private List<Vector3> vertices;
    private List<int> triangles;
    public float changer1;
    public float changer2;
    public int changer3;
    // Start is called before the first frame update
    void Update()
    {
        GeneratePlane(new Vector2(changer1, changer2), changer3);
        CreateMesh();
    }

    void GeneratePlane(Vector2 size, int resolution)
    {
        // Create vertices
        vertices = new List<Vector3>();
        float xPerStep = size.x / resolution;
        float yPerStep = size.y / resolution;
        for (int y = 0; y < resolution + 1; y++)
        {
            for (int x = 0; x < resolution + 1; x++)
            {
                vertices.Add(new Vector3(x * xPerStep, 0, y * yPerStep));
            }
        }

        // Create triangles
        triangles = new List<int>();
        for (int row = 0; row < resolution; row++)
        {
            for (int column = 0; column < resolution; column++)
            {
                int i = row * (resolution + 1) + column;

                // First triangle
                triangles.Add(i);
                triangles.Add(i + resolution + 1);
                triangles.Add(i + resolution + 2);

                // Second triangle
                triangles.Add(i);
                triangles.Add(i + resolution + 2);
                triangles.Add(i + 1);
            }
        }
    }

    void CreateMesh()
    {
        // Create a new Mesh
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        // Attach the Mesh to the GameObject
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        meshFilter.mesh = mesh;

        // Recalculate normals for proper lighting
        mesh.RecalculateNormals();
    }

    // Update is called once per frame
 
}