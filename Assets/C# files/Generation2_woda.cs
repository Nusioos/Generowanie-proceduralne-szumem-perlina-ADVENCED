using System.Collections.Generic;
using UnityEngine;

public class Generation2_woda : MonoBehaviour
{
    public Transform chunkPrefab;
    public int chunkSize = 10;
    public int loadDistance = 3;
    public Transform player;

    private Vector3 lastPlayerPosition;
    private Dictionary<Vector3Int, Transform> activeChunks = new Dictionary<Vector3Int, Transform>();

    [System.Serializable]
    public class ChunkData
    {
        public Vector3Int position;
        public float[] heightMap;

        public ChunkData(Vector3Int position, float[] heightMap)
        {
            this.position = position;
            this.heightMap = heightMap;
        }
    }

    private void Start()
    {
        lastPlayerPosition = player.position;
        UpdateChunks();
    }

    private void Update()
    {
        Vector3 playerPosition = player.position;
        Vector3Int playerChunkCoord = GetChunkCoord(playerPosition);

        // Check if the player has moved significantly
        if (Vector3.SqrMagnitude(playerPosition - lastPlayerPosition) > chunkSize * chunkSize)
        {
            lastPlayerPosition = playerPosition;
            UpdateChunks();
        }
    }

    private void UpdateChunks()
    {
        Vector3Int playerChunkCoord = GetChunkCoord(player.position);

        // Determine which chunks are within the load distance
        HashSet<Vector3Int> newChunkCoords = new HashSet<Vector3Int>();

        for (int x = -loadDistance; x <= loadDistance; x++)
        {
            for (int z = -loadDistance; z <= loadDistance; z++)
            {
                Vector3Int chunkCoord = playerChunkCoord + new Vector3Int(x, 13, z);
                newChunkCoords.Add(chunkCoord);

                if (!activeChunks.ContainsKey(chunkCoord))
                {
                    // Instantiate and load new chunk if not already active
                    Transform chunk = Instantiate(chunkPrefab, new Vector3(chunkCoord.x * chunkSize, 13, chunkCoord.z * chunkSize), Quaternion.identity, transform);
                    activeChunks[chunkCoord] = chunk;
                }
            }
        }

        // Unload chunks that are no longer within the load distance
        List<Vector3Int> chunksToUnload = new List<Vector3Int>();

        foreach (var chunkPair in activeChunks)
        {
            Vector3Int chunkCoord = chunkPair.Key;
            if (!newChunkCoords.Contains(chunkCoord))
            {
                chunksToUnload.Add(chunkCoord);
            }
        }

        foreach (var chunkCoord in chunksToUnload)
        {
            SaveChunkToDisk(chunkCoord, activeChunks[chunkCoord]);
            Destroy(activeChunks[chunkCoord].gameObject);
            activeChunks.Remove(chunkCoord);
        }
    }

    private Transform LoadChunkFromDisk(Vector3Int chunkCoord)
    {
        // This function is not needed in this approach since chunks are instantiated fresh
        return null;
    }

    private void ApplyChunkData(Transform chunk, ChunkData chunkData)
    {
        // Apply chunk data to the chunk if needed
    }

    private void SaveChunkToDisk(Vector3Int chunkCoord, Transform chunk)
    {
        string fileName = Application.persistentDataPath + $"/chunk_{chunkCoord.x}_{chunkCoord.z}.dat";
        ChunkData chunkData = new ChunkData(chunkCoord, GetChunkHeightMap(chunk));

        string jsonData = JsonUtility.ToJson(chunkData);
        System.IO.File.WriteAllText(fileName, jsonData);
    }

    private float[] GetChunkHeightMap(Transform chunk)
    {
        return new float[chunkSize * chunkSize];
    }

    private Vector3Int GetChunkCoord(Vector3 position)
    {
        return new Vector3Int(
            Mathf.FloorToInt(position.x / chunkSize),
            13,
            Mathf.FloorToInt(position.z / chunkSize)
        );
    }
}
