using System.Collections;
using UnityEngine;

public class stwarzacz1 : MonoBehaviour
{
    [SerializeField]
    private Transform cubePos;
    [SerializeField]
    private GameObject stwarwzaczPrefab;
    [SerializeField]
    private Transform player;
    [SerializeField]
    private bool isSpawned = false;

    private float distanceSquaredThreshold = 200f * 200f;
    private GameObject spawnedSimba;

    void Start()
    {
        // Validate and initialize positions if necessary
        if (cubePos == null)
        {
            Debug.LogError("cubePos is not assigned!");
        }

        if (player == null)
        {
            Debug.LogError("player is not assigned!");
        }

        // Start the distance check coroutine
        StartCoroutine(CheckDistance());
    }

    private IEnumerator CheckDistance()
    {
        while (true)
        {
            Vector3 playerPos = player.position;
            Vector3 cubePos = this.cubePos.position;
            if (cubePos == null || player == null)
                yield return null;

          

            // Calculate the square of the distance between player and cubePos
            float distanceSquared = (playerPos.x - cubePos.x) * (playerPos.x - cubePos.x) +
                                    (playerPos.z - cubePos.z) * (playerPos.z - cubePos.z);

            if (distanceSquared < distanceSquaredThreshold)
            {
                if (!isSpawned)
                {
                    spawnedSimba = Instantiate(stwarwzaczPrefab, this.cubePos.position, Quaternion.identity);
                    isSpawned = true;
                }
                else if (spawnedSimba != null && !spawnedSimba.activeSelf)
                {
                    spawnedSimba.SetActive(true);
                }
            }
            else if (isSpawned && spawnedSimba != null && spawnedSimba.activeSelf)
            {
                spawnedSimba.SetActive(false);
                // Uncomment the next line if you want to allow re-spawning
                // isSpawned = false;
            }

            // Wait for a short duration before checking the distance again
            yield return new WaitForSeconds(0.2f); // Adjust this value based on your needs
        }
    }
}
