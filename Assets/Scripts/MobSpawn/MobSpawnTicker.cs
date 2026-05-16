using System;
using System.Collections.Generic;
using UnityEngine;

/// Layer 1: MobSpawnTicker - 소환 주기 및 강도 관리
/// TimeManager 상태를 기반으로 주기적인 소환 신호(Tick)를 생성하고,
/// 시간 진행에 따른 소환 빈도 감쇄 및 난이도 상승 로직을 관리
public class MobSpawnTicker : MonoBehaviour
{
    [SerializeField] private MobSpawnData daySpawnData;
    [SerializeField] private MobSpawnData nightSpawnData;
    [SerializeField] private List<float> nightIntervals = new() { 6f, 6f, 5f, 4f, 3f };
    /// 낮 시간 동안 사용되는 소환 간격 (초)
    [SerializeField] private float dayInterval = 3f;

    private Coroutine tickerCoroutine;

    /// 선정된 몹 엔트리 + 마릿수를 실어 발행
    public event Action<MobSpawnEntry, int> OnSpawnTick;

    private void OnEnable()
    {
        StartTicker();
    }

    private void Start()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTimeStateChanged += HandleTimeStateChanged;
        }
    }

    private void OnDisable()
    {
        StopTicker();
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTimeStateChanged -= HandleTimeStateChanged;
        }
    }

    private void HandleTimeStateChanged(bool isDay)
    {
        // 시간이 변경되면 기존 대기를 취소하고 즉시 새로운 주기로 재시작
        StartTicker();
    }

    /// 틱 루프 시작
    public void StartTicker()
    {
        if (tickerCoroutine != null)
            StopCoroutine(tickerCoroutine);

        tickerCoroutine = StartCoroutine(TickerLoop());
    }

    /// 틱 루프 정지
    public void StopTicker()
    {
        if (tickerCoroutine != null)
        {
            StopCoroutine(tickerCoroutine);
            tickerCoroutine = null;
        }
    }

    /// 주기적인 소환 틱 코루틴
    private System.Collections.IEnumerator TickerLoop()
    {
        while (true)
        {
            float interval = CalculateCurrentInterval();
            yield return new WaitForSeconds(interval);

            // 현재 낮/밤 상태에 맞는 소환 데이터 선택
            MobSpawnData spawnData = TimeManager.Instance.IsDay ? daySpawnData : nightSpawnData;

            if (spawnData == null || spawnData.mobEntries.Count == 0)
                continue;

            // 가중치 기반으로 몹 선택
            MobSpawnEntry selectedEntry = SelectMobByWeight(spawnData);

            if (selectedEntry == null)
                continue;

            // 무리 수 결정
            int groupSize = DetermineGroupSize(selectedEntry);

            // 이벤트 발행
            OnSpawnTick?.Invoke(selectedEntry, groupSize);
        }
    }

    /// 현재 시간 진행률에 따라 소환 간격 계산 (감쇄 적용)
    /// 0.0~0.4: 기준 간격 * 1.0x
    /// 0.4~1.0: 기준 간격 * Lerp(1.0f, 3.0f, progress)
    private float CalculateCurrentInterval()
    {
        float baseInt = GetBaseInterval();
        float progress = TimeManager.Instance.CurrentTimeProgress;

        if (progress < 0.4f)
        {
            return baseInt * 1.0f;
        }
        else
        {
            // 0.4~1.0 구간: Lerp(1.0f, 3.0f, (progress - 0.4f) / 0.6f)
            float normalizedProgress = (progress - 0.4f) / 0.6f;
            float multiplier = Mathf.Lerp(1.0f, 3.0f, normalizedProgress);
            return baseInt * multiplier;
        }
    }

    /// 현재 밤 회차(nightCount)에 해당하는 기준 간격 반환
    /// TimeManager.NightCount를 사용하여 정확한 밤 횟수 카운트
    private float GetBaseInterval()
    {
        // 1. 낮일 때는 무조건 dayInterval을 사용
        if (TimeManager.Instance.IsDay)
        {
            return dayInterval;
        }

        // 2. 밤일 때는 회차별 간격 사용
        int nightCount = TimeManager.Instance.NightCount;

        if (nightCount < nightIntervals.Count)
        {
            return nightIntervals[nightCount];
        }

        // 3. 리스트 범위를 벗어난 경우 마지막 값 유지
        if (nightIntervals.Count > 0)
        {
            return nightIntervals[nightIntervals.Count - 1];
        }

        // 리스트가 아예 비어있는 경우에만 dayInterval로 폴백
        return dayInterval;
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
}
