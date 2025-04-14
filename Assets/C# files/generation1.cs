using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generation1 : MonoBehaviour
{
    public Transform chunkPrefab;
    public int chunkSize = 10;
    public int loadDistance = 3;
    public Transform player;
    public float updateInterval = 0.1f; // Interval between updates

    private Vector3 lastPlayerPosition;
    private Dictionary<Vector3Int, Transform> activeChunks = new Dictionary<Vector3Int, Transform>();
    private List<Transform> chunkPool = new List<Transform>();

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
        StartCoroutine(UpdateChunksCoroutine());
    }

    private IEnumerator UpdateChunksCoroutine()
    {
        while (true)
        {
            Vector3 playerPosition = player.position;
            Vector3Int playerChunkCoord = GetChunkCoord(playerPosition);

            if (Vector3.SqrMagnitude(playerPosition - lastPlayerPosition) > chunkSize * chunkSize)
            {
                lastPlayerPosition = playerPosition;
                UpdateChunks();
            }

            yield return new WaitForSeconds(updateInterval);
        }
    }

    private void UpdateChunks()
    {
        Vector3Int playerChunkCoord = GetChunkCoord(player.position);

        // Load new chunks
        for (int x = -loadDistance; x <= loadDistance; x++)
        {
            for (int z = -loadDistance; z <= loadDistance; z++)
            {
                Vector3Int chunkCoord = playerChunkCoord + new Vector3Int(x, -70, z);

                if (!activeChunks.ContainsKey(chunkCoord))
                {
                    Transform chunk = GetChunkFromPool(chunkCoord);
                    if (chunk == null)
                    {
                        chunk = Instantiate(chunkPrefab, new Vector3(chunkCoord.x * chunkSize, -70, chunkCoord.z * chunkSize), Quaternion.identity, transform);
                    }
                    activeChunks[chunkCoord] = chunk;
                }
            }
        }

        // Unload old chunks
        List<Vector3Int> chunksToUnload = new List<Vector3Int>();
        foreach (var chunkPair in activeChunks)
        {
            Vector3Int chunkPosition = chunkPair.Key;

            bool chunkIsNearPlayer = Mathf.Abs(chunkPosition.x - playerChunkCoord.x) <= loadDistance &&
                                     Mathf.Abs(chunkPosition.z - playerChunkCoord.z) <= loadDistance;

            if (!chunkIsNearPlayer)
            {
                chunksToUnload.Add(chunkPosition);
            }
        }

        foreach (var chunkPosition in chunksToUnload)
        {
            SaveChunkToDisk(chunkPosition, activeChunks[chunkPosition]);
            DeactivateChunk(activeChunks[chunkPosition]);
            activeChunks.Remove(chunkPosition);
        }
    }

    private Transform GetChunkFromPool(Vector3Int chunkCoord)
    {
        foreach (var chunk in chunkPool)
        {
            if (chunk != null && !chunk.gameObject.activeSelf && chunk.position == new Vector3(chunkCoord.x * chunkSize, -70, chunkCoord.z * chunkSize))
            {
                chunk.gameObject.SetActive(true);
                return chunk;
            }
        }
        return null;
    }

    private void DeactivateChunk(Transform chunk)
    {
        chunk.gameObject.SetActive(false);
        chunkPool.Add(chunk);
    }

    private void ApplyChunkData(Transform chunk, ChunkData chunkData)
    {
        // Implement logic to apply chunk data (e.g., heightmaps, terrain) to the chunk
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
            -70,
            Mathf.FloorToInt(position.z / chunkSize)
        );
    }
}
