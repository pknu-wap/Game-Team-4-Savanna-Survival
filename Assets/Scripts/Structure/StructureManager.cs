using System;
using System.Collections.Generic;
using UnityEngine;

// Layer 1: Structure Manager - 구조물 풀 관리자
// 나무, 바위 등 랜덤 구조물의 오브젝트 풀링을 관리하고, 청크의 요청에 따라 밀도와 규칙에 맞는 구조물을 배치한다.
public class StructureManager : MonoBehaviour
{
    [SerializeField] private StructureData structureData;
    [SerializeField] private Transform poolParent;

    private Dictionary<StructureType, Queue<GameObject>> _structurePool = new Dictionary<StructureType, Queue<GameObject>>();
    private Dictionary<ChunkEntity, List<GameObject>> _chunkStructures = new Dictionary<ChunkEntity, List<GameObject>>();

    private static StructureManager _instance;
    public static StructureManager Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        if (structureData == null)
        {
            Debug.LogError("StructureData not assigned to StructureManager!");
            return;
        }

        // 풀 부모 오브젝트 생성 (없으면)
        if (poolParent == null)
        {
            GameObject poolObj = new GameObject("StructurePool");
            poolObj.transform.SetParent(transform);
            poolParent = poolObj.transform;
        }

        // 각 구조물 타입별로 풀 초기화
        foreach (var entry in structureData.structureEntries)
        {
            if (entry.structureType == StructureType.None)
                continue;

            if (entry.prefab == null)
            {
                Debug.LogWarning($"Prefab not assigned for {entry.structureType}!");
                continue;
            }

            Queue<GameObject> queue = new Queue<GameObject>();

            // poolSize만큼 미리 생성
            for (int i = 0; i < entry.poolSize; i++)
            {
                GameObject obj = Instantiate(entry.prefab, poolParent);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }

            _structurePool[entry.structureType] = queue;
        }
    }

    // 청크에 구조물 배치
    public void SpawnStructuresForChunk(ChunkEntity chunk)
    {
        if (chunk == null || structureData == null)
            return;

        if (!_chunkStructures.ContainsKey(chunk))
        {
            _chunkStructures[chunk] = new List<GameObject>();
        }

        Vector3 chunkWorldPos = chunk.transform.position;
        const float chunkSize = 20f;

        // 각 구조물 타입별로 배치
        foreach (var entry in structureData.structureEntries)
        {
            if (entry.structureType == StructureType.None || entry.prefab == null)
                continue;

            int spawnCount = UnityEngine.Random.Range(entry.minCount, entry.maxCount + 1);

            for (int i = 0; i < spawnCount; i++)
            {
                Vector2 spawnPos = FindValidPosition(chunkWorldPos, chunkSize, entry.minDistance);

                GameObject structureObj = GetFromPool(entry.structureType);
                if (structureObj == null)
                    continue;

                structureObj.transform.position = spawnPos;
                structureObj.SetActive(true);
                _chunkStructures[chunk].Add(structureObj);
            }
        }
    }

    // 청크에 배치된 구조물 회수
    public void ClearStructuresForChunk(ChunkEntity chunk)
    {
        if (chunk == null || !_chunkStructures.ContainsKey(chunk))
            return;

        List<GameObject> structures = _chunkStructures[chunk];
        foreach (var structure in structures)
        {
            ReturnToPool(structure);
        }

        structures.Clear();
    }

    // 특정 청크의 구조물 목록 반환 (몹 스폰용)
    public List<GameObject> GetStructuresInChunk(ChunkEntity chunk)
    {
        if (chunk == null || !_chunkStructures.ContainsKey(chunk))
            return new List<GameObject>();

        return _chunkStructures[chunk];
    }

    // 풀에서 구조물 꺼내기
    private GameObject GetFromPool(StructureType type)
    {
        if (!_structurePool.ContainsKey(type))
            return null;

        Queue<GameObject> queue = _structurePool[type];
        if (queue.Count > 0)
        {
            return queue.Dequeue();
        }

        // 풀이 부족하면 동적 생성
        StructureEntry entry = structureData.GetEntry(type);
        if (entry != null && entry.prefab != null)
        {
            return Instantiate(entry.prefab, poolParent);
        }

        return null;
    }

    // 풀에 구조물 반납
    private void ReturnToPool(GameObject structure)
    {
        if (structure == null)
            return;

        // 구조물의 타입 판단 (태그 또는 컴포넌트로 판단 가능)
        // 임시로 Prefab의 이름으로 판단
        StructureType type = DetermineStructureType(structure.name);
        if (type == StructureType.None)
            return;

        structure.SetActive(false);

        if (!_structurePool.ContainsKey(type))
            _structurePool[type] = new Queue<GameObject>();

        _structurePool[type].Enqueue(structure);
    }

    // 유효한 배치 위치 탐색
    private Vector2 FindValidPosition(Vector3 chunkPos, float chunkSize, float minDistance, int maxRetries = 20)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            float randomX = chunkPos.x + UnityEngine.Random.Range(0f, chunkSize);
            float randomY = chunkPos.y + UnityEngine.Random.Range(0f, chunkSize);
            Vector2 candidatePos = new Vector2(randomX, randomY);

            // 다른 구조물과의 거리 확인
            if (IsPositionValid(candidatePos, minDistance))
            {
                return candidatePos;
            }
        }

        // 재시도 실패 시 임의 반환 (대체 위치)
        return new Vector2(chunkPos.x + chunkSize / 2f, chunkPos.y + chunkSize / 2f);
    }

    // 위치 유효성 확인 (주변 구조물과의 거리)
    private bool IsPositionValid(Vector2 pos, float minDistance)
    {
        // 모든 활성 구조물과의 거리 확인
        foreach (var structures in _chunkStructures.Values)
        {
            foreach (var structure in structures)
            {
                if (structure.activeInHierarchy)
                {
                    float distance = Vector2.Distance(pos, structure.transform.position);
                    if (distance < minDistance)
                        return false;
                }
            }
        }
        return true;
    }

    // 구조물 타입 판정 (이름 기반)
    private StructureType DetermineStructureType(string objectName)
    {
        if (objectName.Contains("Tree"))
            return StructureType.Tree;
        if (objectName.Contains("Rock"))
            return StructureType.Rock;
        return StructureType.None;
    }
}
