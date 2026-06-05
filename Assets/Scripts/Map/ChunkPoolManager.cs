using System;
using System.Collections.Generic;
using UnityEngine;

public class ChunkPoolManager : MonoBehaviour
{
    [SerializeField] private GameObject chunkPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float chunkSize = 20f;
    [SerializeField] private int poolSize = 15;

    public event Action<ChunkEntity> OnChunkActivated;

    private Queue<ChunkEntity> _idlePool = new Queue<ChunkEntity>();
    private Dictionary<Vector2Int, ChunkEntity> _activeChunks = new Dictionary<Vector2Int, ChunkEntity>();
    private StructureManager _structureManager;

    public Dictionary<Vector2Int, ChunkEntity> ActiveChunks => _activeChunks;

    private Vector2Int _lastChunkIndex = new Vector2Int(int.MinValue, int.MinValue);

    private void Awake()
    {
        FindPlayer();
    }

    private void Start()
    {
        _structureManager = FindAnyObjectByType<StructureManager>();

        InitPool();

        if (playerTransform == null)
        {
            FindPlayer();
        }

        if (playerTransform != null)
        {
            Vector2Int startIndex = GetChunkIndex(playerTransform.position);
            UpdateChunks(GetActiveIndices(startIndex));
            _lastChunkIndex = startIndex;
        }
    }

    private void Update()
    {
        if (playerTransform == null)
        {
            FindPlayer();
            return;
        }

        Vector2Int currentIndex = GetChunkIndex(playerTransform.position);

        if (currentIndex != _lastChunkIndex)
        {
            _lastChunkIndex = currentIndex;
            UpdateChunks(GetActiveIndices(currentIndex));
        }
    }

    private void FindPlayer()
    {
        if (playerTransform != null)
            return;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Player 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }
    }

    private void InitPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(chunkPrefab, transform);
            obj.SetActive(false);

            ChunkEntity entity = obj.GetComponent<ChunkEntity>();
            _idlePool.Enqueue(entity);
        }
    }

    private Vector2Int GetChunkIndex(Vector3 playerPos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(playerPos.x / chunkSize),
            Mathf.FloorToInt(playerPos.y / chunkSize)
        );
    }

    private List<Vector2Int> GetActiveIndices(Vector2Int center)
    {
        List<Vector2Int> indices = new List<Vector2Int>(9);

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                indices.Add(new Vector2Int(center.x + dx, center.y + dy));
            }
        }

        return indices;
    }

    public void UpdateChunks(List<Vector2Int> targetIndices)
    {
        HashSet<Vector2Int> targetSet = new HashSet<Vector2Int>(targetIndices);

        List<Vector2Int> toRemove = new List<Vector2Int>();

        foreach (var kvp in _activeChunks)
        {
            if (!targetSet.Contains(kvp.Key))
                toRemove.Add(kvp.Key);
        }

        foreach (var index in toRemove)
        {
            ChunkEntity entity = _activeChunks[index];
            _activeChunks.Remove(index);

            if (_structureManager != null)
            {
                _structureManager.ClearStructuresForChunk(entity);
            }

            entity.ResetChunk();
            entity.gameObject.SetActive(false);
            _idlePool.Enqueue(entity);
        }

        foreach (var index in targetIndices)
        {
            if (_activeChunks.ContainsKey(index))
                continue;

            if (_idlePool.Count == 0)
                break;

            ChunkEntity entity = _idlePool.Dequeue();
            entity.gameObject.SetActive(true);
            entity.Initialize(index);

            _activeChunks[index] = entity;

            if (_structureManager != null)
            {
                _structureManager.SpawnStructuresForChunk(entity);
            }

            OnChunkActivated?.Invoke(entity);
        }
    }
}