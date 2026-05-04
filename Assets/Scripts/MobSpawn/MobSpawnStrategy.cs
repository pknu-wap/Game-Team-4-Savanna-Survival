using System;
using System.Collections.Generic;
using UnityEngine;

/// Layer 2: MobSpawnStrategy - 소환 위치 및 전략 관리
/// 낮과 밤의 상황별 전략에 따라 최적의 소환 좌표를 탐색하고,
/// 유효한 지면(IsWalkable) 검증 후 최종 좌표를 확정
public class MobSpawnStrategy : MonoBehaviour
{
    [SerializeField] private MobSpawnData daySpawnData;
    [SerializeField] private float nightSpawnRadius = 10f;
    [SerializeField] private int maxWalkableRetries = 10;
    [SerializeField] private float walkableCheckRadius = 0.5f;
    [SerializeField] private LayerMask groundLayer;

    private ChunkPoolManager chunkPoolManager;
    private MobSpawnTicker mobSpawnTicker;
    private StructureManager structureManager;

    /// 유효 좌표 1개 + 몹 엔트리를 Layer 3로 전달
    public event Action<Vector2, MobSpawnEntry> SpawnRequested;

    private void OnEnable()
    {
        chunkPoolManager = FindAnyObjectByType<ChunkPoolManager>();
        mobSpawnTicker = FindAnyObjectByType<MobSpawnTicker>();
        structureManager = FindAnyObjectByType<StructureManager>();

        if (mobSpawnTicker != null)
        {
            mobSpawnTicker.OnSpawnTick += OnSpawnTick;
        }

        if (chunkPoolManager != null)
        {
            chunkPoolManager.OnChunkActivated += OnChunkActivated;
        }
    }

    private void OnDisable()
    {
        if (mobSpawnTicker != null)
        {
            mobSpawnTicker.OnSpawnTick -= OnSpawnTick;
        }

        if (chunkPoolManager != null)
        {
            chunkPoolManager.OnChunkActivated -= OnChunkActivated;
        }
    }

    /// Layer 1 이벤트 핸들러: 낮/밤 전략 분기 후 유효 좌표 N개 탐색
    private void OnSpawnTick(MobSpawnEntry entry, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPos;

            if (TimeManager.Instance.IsDay)
            {
                // 낮: 활성 청크 기반 + 구조물 연동 지원
                if (entry.preferredStructure != StructureType.None)
                {
                    // 선호 구조물이 있으면 구조물 반경 스폰 시도
                    ChunkEntity randomChunk = GetRandomActiveChunk();
                    if (randomChunk != null)
                    {
                        spawnPos = TryGetValidPosition(
                            () => FindStructureLinkedPosition(randomChunk, entry.preferredStructure),
                            maxWalkableRetries
                        );

                        // 선호 구조물이 없으면 스폰 불가 (스펙 준수)
                        if (spawnPos == Vector2.zero)
                            continue;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    // 구조물 미연동: 일반 스폰
                    spawnPos = TryGetValidPosition(FindDaySpawnPosition, maxWalkableRetries);

                    if (spawnPos == Vector2.zero)
                        continue;
                }
            }
            else
            {
                // 밤: 카메라 시야 밖 반경 기반 (구조물 무시)
                spawnPos = TryGetValidPosition(FindNightSpawnPosition, maxWalkableRetries);

                if (spawnPos == Vector2.zero)
                    continue;
            }

            if (spawnPos != Vector2.zero)
            {
                SpawnRequested?.Invoke(spawnPos, entry);
            }
        }
    }

    /// 청크 활성화 핸들러: 즉시 스폰 (낮 전용)
    private void OnChunkActivated(ChunkEntity chunk)
    {
        if (!TimeManager.Instance.IsDay || daySpawnData == null)
            return;

        if (daySpawnData.mobEntries.Count == 0)
            return;

        // 가중치 기반 Entry 선정
        MobSpawnEntry selectedEntry = SelectMobByWeight(daySpawnData);

        if (selectedEntry == null)
            return;

        // min/maxGroupSize로 N 결정
        int groupSize = DetermineGroupSize(selectedEntry);

        // 해당 청크 내에서 N개 좌표 탐색
        for (int i = 0; i < groupSize; i++)
        {
            Vector2 spawnPos;

            // preferredStructure가 None이 아니면 구조물 반경 스폰 시도
            if (selectedEntry.preferredStructure != StructureType.None)
            {
                spawnPos = TryGetValidPosition(
                    () => FindStructureLinkedPosition(chunk, selectedEntry.preferredStructure),
                    maxWalkableRetries
                );

                // 선호 구조물이 없으면 해당 몹은 스폰하지 않음 (스펙 준수)
                if (spawnPos == Vector2.zero)
                    continue;
            }
            else
            {
                // 일반 스폰 (구조물 미연동)
                spawnPos = TryGetValidPosition(
                    () => FindDaySpawnPosition(chunk),
                    maxWalkableRetries
                );

                if (spawnPos == Vector2.zero)
                    continue;
            }

            SpawnRequested?.Invoke(spawnPos, selectedEntry);
        }
    }

    /// 활성 청크 중 무작위 선택 헬퍼 메서드
    private ChunkEntity GetRandomActiveChunk()
    {
        if (chunkPoolManager == null || chunkPoolManager.ActiveChunks.Count == 0)
            return null;

        int randomIndex = UnityEngine.Random.Range(0, chunkPoolManager.ActiveChunks.Count);
        var enumerator = chunkPoolManager.ActiveChunks.Values.GetEnumerator();

        for (int i = 0; i < randomIndex; i++)
            enumerator.MoveNext();

        return enumerator.Current;
    }

    /// 청크 내 무작위 좌표 탐색
    private Vector2 FindDaySpawnPosition()
    {
        ChunkEntity randomChunk = GetRandomActiveChunk();
        if (randomChunk == null)
            return Vector2.zero;

        return FindDaySpawnPosition(randomChunk);
    }

    /// 특정 청크 내 무작위 좌표 탐색
    private Vector2 FindDaySpawnPosition(ChunkEntity chunk)
    {
        if (chunk == null)
            return Vector2.zero;

        // 청크 크기: 20x20
        Vector3 chunkWorldPos = chunk.transform.position;
        float randomX = chunkWorldPos.x + UnityEngine.Random.Range(0f, 20f);
        float randomY = chunkWorldPos.y + UnityEngine.Random.Range(0f, 20f);

        return new Vector2(randomX, randomY);
    }

    /// 카메라 시야 밖 반경 내 무작위 좌표 탐색
    private Vector2 FindNightSpawnPosition()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
            return Vector2.zero;

        Vector3 cameraPos = mainCamera.transform.position;
        float cameraOrthogonalSize = mainCamera.orthographicSize;

        // 카메라 시야 반경: orthographicSize (대각선으로 확대)
        float visionRadius = cameraOrthogonalSize * 1.5f;

        // nightSpawnRadius 거리 떨어진 좌표 탐색
        float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float spawnDistance = visionRadius + nightSpawnRadius;

        float spawnX = cameraPos.x + Mathf.Cos(angle) * spawnDistance;
        float spawnY = cameraPos.y + Mathf.Sin(angle) * spawnDistance;

        return new Vector2(spawnX, spawnY);
    }

    /// 유효 지면 검사 (IsWalkable)
    private bool IsWalkable(Vector2 pos)
    {
        Collider2D hit = Physics2D.OverlapCircle(pos, walkableCheckRadius, groundLayer);
        return hit != null;
    }

    /// 최대 시도 횟수 내 유효 좌표 반환 (공통 유틸)
    private Vector2 TryGetValidPosition(Func<Vector2> positionGenerator, int maxRetries)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            Vector2 pos = positionGenerator.Invoke();

            if (IsWalkable(pos))
            {
                return pos;
            }
        }

        // 최대 시도 실패 시 반환 안 함 (zero)
        return Vector2.zero;
    }

    /// 가중치 기반으로 몹 엔트리 선택
    private MobSpawnEntry SelectMobByWeight(MobSpawnData spawnData)
    {
        if (spawnData.mobEntries.Count == 0)
            return null;

        // 가중치 합계 계산
        int totalWeight = 0;
        foreach (var entry in spawnData.mobEntries)
        {
            totalWeight += entry.weight;
        }

        if (totalWeight <= 0)
            return spawnData.mobEntries[0];

        // Random.value (0~1) * 총 가중치
        float randomValue = UnityEngine.Random.value * totalWeight;
        float weightSum = 0f;

        foreach (var entry in spawnData.mobEntries)
        {
            weightSum += entry.weight;
            if (randomValue <= weightSum)
            {
                return entry;
            }
        }

        return spawnData.mobEntries[spawnData.mobEntries.Count - 1];
    }

    /// 무리 수 결정 (min ~ max 범위)
    private int DetermineGroupSize(MobSpawnEntry entry)
    {
        if (entry.minGroupSize >= entry.maxGroupSize)
        {
            return entry.minGroupSize;
        }

        return UnityEngine.Random.Range(entry.minGroupSize, entry.maxGroupSize + 1);
    }

    /// 선호 구조물 타입 기반 스폰 위치 탐색 (구조물 연동 스폰용)
    /// preferredStructure가 None이면 일반 스폰 위치 반환
    private Vector2 FindStructureLinkedPosition(ChunkEntity chunk, StructureType preferredStructure)
    {
        if (chunk == null || structureManager == null)
            return Vector2.zero;

        // preferredStructure가 None이면 일반 위치 탐색
        if (preferredStructure == StructureType.None)
            return FindDaySpawnPosition(chunk);

        List<GameObject> structures = structureManager.GetStructuresInChunk(chunk);
        if (structures.Count == 0)
            return Vector2.zero;

        // preferredStructure 타입과 일치하는 구조물만 필터링
        List<GameObject> preferredStructures = new List<GameObject>();
        foreach (var structure in structures)
        {
            StructureType structureType = DetermineStructureType(structure.name);
            if (structureType == preferredStructure)
            {
                preferredStructures.Add(structure);
            }
        }

        // 선호 구조물이 없으면 스폰 불가 (스펙 준수)
        if (preferredStructures.Count == 0)
            return Vector2.zero;

        // 선호 구조물 중 무작위 선택
        GameObject randomStructure = preferredStructures[UnityEngine.Random.Range(0, preferredStructures.Count)];
        if (randomStructure == null)
            return Vector2.zero;

        // 구조물 반경 내(1.5Unit) 무작위 위치 선정
        Vector2 structurePos = randomStructure.transform.position;
        float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float distance = UnityEngine.Random.Range(0f, 1.5f);

        return new Vector2(
            structurePos.x + Mathf.Cos(angle) * distance,
            structurePos.y + Mathf.Sin(angle) * distance
        );
    }

    /// 구조물 타입 판정 (이름 기반)
    private StructureType DetermineStructureType(string objectName)
    {
        if (objectName.Contains("Tree"))
            return StructureType.Tree;
        if (objectName.Contains("Rock"))
            return StructureType.Rock;
        return StructureType.None;
    }
}
