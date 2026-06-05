using System.Collections.Generic;
using UnityEngine;

public class FloorTileSpawner : MonoBehaviour
{
    [Header("플레이어")]
    [SerializeField] private Transform player;

    [Header("바닥 타일 프리팹")]
    [SerializeField] private GameObject floorTilePrefab;

    [Header("타일 크기")]
    [SerializeField] private float tileSize = 1f;

    [Header("청크당 타일 개수")]
    [SerializeField] private int chunkTileCount = 20;

    [Header("주변 청크 생성 범위")]
    [SerializeField] private int activeRadius = 1;

    private readonly HashSet<Vector2Int> spawnedChunks = new();
    private Vector2Int lastChunkIndex;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
                player = playerObj.transform;
            else
            {
                Debug.LogError("Player를 찾을 수 없습니다.");
                enabled = false;
                return;
            }
        }

        lastChunkIndex = GetChunkIndex(player.position);

        SpawnChunksAround(lastChunkIndex);
    }

    private void Update()
    {
        Vector2Int currentChunk = GetChunkIndex(player.position);

        if (currentChunk != lastChunkIndex)
        {
            lastChunkIndex = currentChunk;
            SpawnChunksAround(currentChunk);
        }
    }

    private Vector2Int GetChunkIndex(Vector3 position)
    {
        float chunkWorldSize = chunkTileCount * tileSize;

        return new Vector2Int(
            Mathf.FloorToInt(position.x / chunkWorldSize),
            Mathf.FloorToInt(position.y / chunkWorldSize)
        );
    }

    private void SpawnChunksAround(Vector2Int centerChunk)
    {
        for (int x = -activeRadius; x <= activeRadius; x++)
        {
            for (int y = -activeRadius; y <= activeRadius; y++)
            {
                Vector2Int chunkIndex = new Vector2Int(
                    centerChunk.x + x,
                    centerChunk.y + y
                );

                if (spawnedChunks.Contains(chunkIndex))
                    continue;

                SpawnChunk(chunkIndex);

                spawnedChunks.Add(chunkIndex);
            }
        }
    }

    private void SpawnChunk(Vector2Int chunkIndex)
    {
        GameObject chunkObj = new GameObject($"Chunk_{chunkIndex.x}_{chunkIndex.y}");
        chunkObj.transform.SetParent(transform);

        float chunkStartX = chunkIndex.x * chunkTileCount * tileSize;
        float chunkStartY = chunkIndex.y * chunkTileCount * tileSize;

        for (int x = 0; x < chunkTileCount; x++)
        {
            for (int y = 0; y < chunkTileCount; y++)
            {
                Vector3 pos = new Vector3(
                    chunkStartX + (x * tileSize),
                    chunkStartY + (y * tileSize),
                    0f
                );

                Instantiate(
                    floorTilePrefab,
                    pos,
                    Quaternion.identity,
                    chunkObj.transform
                );
            }
        }
    }
}