using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Layer 3: MobEntityManager - 몹 생성 및 한도 관리
/// 최종 좌표에 몹을 실제로 생성하거나 오브젝트 풀에서 꺼내오며,
/// 필드 내 전체 마릿수(Global Cap)를 관리

public class MobEntityManager : MonoBehaviour
{
    [SerializeField] private int globalCap = 50;
    [SerializeField] private float despawnCheckInterval = 1f;

    private int currentMobCount = 0;
    private List<GameObject> activeMobs = new List<GameObject>();
    private ChunkPoolManager chunkPoolManager;

    /// 현재 필드에 있는 몹 개수 (읽기 전용)
    public int CurrentMobCount => currentMobCount;

    /// 프리팹별 몹 풀 관리
    private Dictionary<GameObject, Queue<GameObject>> mobPool = new();

    private MobSpawnStrategy mobSpawnStrategy;

    private void OnEnable()
    {
        mobSpawnStrategy = FindAnyObjectByType<MobSpawnStrategy>();
        chunkPoolManager = FindAnyObjectByType<ChunkPoolManager>();

        if (mobSpawnStrategy != null)
        {
            mobSpawnStrategy.SpawnRequested += OnSpawnRequested;
        }

        StartCoroutine(DespawnCheckLoop());
    }

    private void OnDisable()
    {
        if (mobSpawnStrategy != null)
        {
            mobSpawnStrategy.SpawnRequested -= OnSpawnRequested;
        }
    }

    /// Layer 2 이벤트 핸들러: 한도 체크 후 실제 생성
    private void OnSpawnRequested(Vector2 spawnPos, MobSpawnEntry entry)
    {
        // 전역 마릿수 제한 체크
        if (currentMobCount >= globalCap)
        {
            return;
        }

        // 실제 생성
        GameObject prefab = entry.mobPrefab;
        if (prefab == null)
        {
            Debug.LogWarning("MobSpawnEntry의 mobPrefab이 null");
            return;
        }

        GameObject mobInstance = SpawnMob(prefab, spawnPos);

        if (mobInstance != null)
        {
            currentMobCount++;

            // 이후 몹이 사망할 때 풀에 반환하도록 컴포넌트 설정 (나중에 전투 중에 죽었을 때)
            // -> 몹 스크립트에서 MobEntityManager.Instance.ReturnMobToPool(gameObject)을 호출
        }
    }

    /// 오브젝트 풀에서 꺼내거나 없으면 생성
    private GameObject SpawnMob(GameObject prefab, Vector2 pos)
    {
        GameObject mobInstance = null;

        // 풀에서 재사용 가능한 몹이 있는지 확인
        if (mobPool.ContainsKey(prefab) && mobPool[prefab].Count > 0)
        {
            mobInstance = mobPool[prefab].Dequeue();
            mobInstance.SetActive(true);
        }
        else
        {
            // 풀이 없거나 비어있으면 생성
            if (!mobPool.ContainsKey(prefab))
            {
                mobPool[prefab] = new Queue<GameObject>();
            }

            mobInstance = Instantiate(prefab, transform);
        }

        // 위치 설정
        if (mobInstance != null)
        {
            mobInstance.transform.position = pos;
        }

        activeMobs.Add(mobInstance);
        return mobInstance;
    }

    /// 몹 사망·제거 시 풀에 반환
    public void ReturnMobToPool(GameObject mob)
    {
        if (mob == null)
        {
            return;
        }

        activeMobs.Remove(mob);

        // 원본 프리팹 찾기 (PrefabUtility 사용 또는 프리팹 참조 저장)
        // 간단히: 이 함수를 호출할 때 mob의 원본 프리팹을 알아야 함
        // 또는 mob에 프리팹 참조를 저장하는 컴포넌트 필요

        // 현재 구현: Dictionary의 키 중에서 찾기
        GameObject prefab = null;

        foreach (var key in mobPool.Keys)
        {
            if (key != null && key.name == mob.name.Replace("(Clone)", "").Trim())
            {
                prefab = key;
                break;
            }
        }

        if (prefab == null)
        {
            // 프리팹을 찾지 못했으면 Destroy
            Destroy(mob);
            return;
        }

        // 풀에 반환
        mob.SetActive(false);
        mobPool[prefab].Enqueue(mob);
        currentMobCount--;
    }

    private IEnumerator DespawnCheckLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(despawnCheckInterval);

            if (chunkPoolManager == null) continue;

            for (int i = activeMobs.Count - 1; i >= 0; i--)
            {
                GameObject mob = activeMobs[i];
                if (mob == null) continue;

                bool isInActiveChunk = false;

                foreach (var chunk in chunkPoolManager.ActiveChunks.Values)
                {
                    Bounds chunkBounds = new Bounds(
                        chunk.transform.position + new Vector3(10f, 10f, 0f),
                        new Vector3(20f, 20f, 0f)
                    );

                    if (chunkBounds.Contains(mob.transform.position))
                    {
                        isInActiveChunk = true;
                        break;
                    }
                }

                if (!isInActiveChunk)
                {
                    ReturnMobToPool(activeMobs[i]);
                }
            }
        }
    }
}
