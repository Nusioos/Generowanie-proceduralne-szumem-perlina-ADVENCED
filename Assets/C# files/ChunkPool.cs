using System.Collections.Generic;
using UnityEngine;

public class ChunkPool
{
    private Transform chunkPrefab;
    private Queue<Transform> chunkPool = new Queue<Transform>();
    private Dictionary<Vector3Int, Transform> activeChunks = new Dictionary<Vector3Int, Transform>();
    private int maxChunks;

    public ChunkPool(Transform prefab, int maxChunks)
    {
        chunkPrefab = prefab;
        this.maxChunks = maxChunks;
    }

    public Transform GetChunk(Vector3 position)
    {
        Transform chunk;
        if (chunkPool.Count > 0)
        {
            chunk = chunkPool.Dequeue();
            chunk.gameObject.SetActive(true);
        }
        else if (activeChunks.Count < maxChunks)
        {
            chunk = Object.Instantiate(chunkPrefab);
        }
        else
        {
            
            var oldestChunkCoord = activeChunks.Keys.GetEnumerator();
            oldestChunkCoord.MoveNext();
            chunk = activeChunks[oldestChunkCoord.Current];
            ReturnChunk(chunk);
        }

        chunk.position = position;
        return chunk;
    }

    public void ReturnChunk(Transform chunk)
    {
        chunk.gameObject.SetActive(false);
        chunkPool.Enqueue(chunk);
    }

    public Transform GetActiveChunk(Vector3Int chunkCoord)
    {
        if (activeChunks.ContainsKey(chunkCoord))
        {
            return activeChunks[chunkCoord];
        }
        return null;
    }
}
