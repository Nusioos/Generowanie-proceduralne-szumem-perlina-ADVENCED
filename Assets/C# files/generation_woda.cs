using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generation_woda : MonoBehaviour
{
    public Transform cubePrefab; // Renamed for clarity
    public int resolution;

    [SerializeField, Range(0, 224)]
    public int picker;
    public Vector3[] blockPositions; // Combined array for positions

    private void Start()
    {
        // Initialize the blockPositions array
        blockPositions = new Vector3[resolution * resolution];

        // Precompute some values
        float spacing = 0.5f * 48;
        float offsetX = 28f;
        float offsetZ = 21f;
     
        int i = 0;
        for (int x = 0; x < resolution; x++)
        {
            for (int z = 0; z < resolution; z++)
            {
                // Instantiate the cube with the current script's transform as the parent
                Transform point = Instantiate(cubePrefab, transform);

                // Compute the position
                Vector3 position = new Vector3(spacing * x, 13.1f, spacing * z);

                // Set the position of the cube
                point.position = position;

                // Store the adjusted position in the array
                blockPositions[i] = position + new Vector3(offsetX, 0, offsetZ);

                i++;
            }
        }
    }


}

