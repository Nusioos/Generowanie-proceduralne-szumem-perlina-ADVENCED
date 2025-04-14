using System.Collections;
using UnityEngine;

public class stwarzacz : MonoBehaviour
{
    [SerializeField] private Transform cubePos;
    [SerializeField] private GameObject stwarwzaczPrefab;
    [SerializeField] public Transform player;
    [SerializeField] private bool isSpawned = false;

    private float distanceSquaredThreshold = 100f * 100f;
    private GameObject spawnedSimba;
    private Vector3 lastPlayerPosition;

    private void Awake()
    {
        if (cubePos == null)
        {
            Debug.LogError("cubePos is not assigned!");
        }

        if (player == null)
        {
            Debug.LogError("player is not assigned!");
        }

        lastPlayerPosition = player.position;

        // Optionally, spawn the object at start if it's within the threshold
        CheckAndSpawn();
    }

    private void Update()
    {
        // Only check distance if player has moved beyond a threshold
        if ((player.position - lastPlayerPosition).sqrMagnitude > 1f) // Threshold to avoid unnecessary checks
        {
            lastPlayerPosition = player.position;
            CheckAndSpawn();
        }
    }

    private void CheckAndSpawn()
    {
        Vector3 playerPos = player.position;
        Vector3 cubePos = this.cubePos.position;

        // Calculate the square of the distance between player and cubePos
        float distanceSquared = (playerPos - cubePos).sqrMagnitude;

        // If the player is within the threshold distance
        if (distanceSquared < distanceSquaredThreshold)
        {
            if (!isSpawned)
            {
                // Instantiate only once
                spawnedSimba = Instantiate(stwarwzaczPrefab, this.cubePos.position, Quaternion.identity);
                isSpawned = true;
            }
            else if (spawnedSimba != null && !spawnedSimba.activeSelf)
            {
                // Only activate if it's not already active
                spawnedSimba.SetActive(true);
            }
        }
        else if (isSpawned && spawnedSimba != null && spawnedSimba.activeSelf)
        {
            // Only deactivate if it's currently active
            Destroy(spawnedSimba);
         //   Destroy(stwarwzaczPrefab);
        }
    }
}
