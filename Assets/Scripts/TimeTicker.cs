using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Layer 1: TimeTicker - 순수 C# 시간 연산 로직
/// 분 단위의 주기 설정을 초로 변환하고, 타이머를 업데이트하며 낮/밤 주기를 계산합니다.
/// </summary>
[Serializable]
public struct CycleDuration
{
    public int minutes;
    public int seconds;

    public CycleDuration(int m, int s)
    {
        minutes = m;
        seconds = s;
    }

    public float TotalSeconds => minutes * 60f + seconds;
}

public class TimeTicker
{
    private List<float> cycleDurations = new();
    private float currentTime = 0f;
    private int cycleIndex = 0;
    private bool cycleChanged = false;

    /// <summary>초기화: 분 단위를 초로 변환하여 내부 배열 저장</summary>
    public void Initialize(List<CycleDuration> cycleDurationsInput)
    {
        cycleDurations.Clear();
        foreach (var cycle in cycleDurationsInput)
        {
            cycleDurations.Add(cycle.TotalSeconds);
        }
        currentTime = 0f;
        cycleIndex = 0;
        cycleChanged = false;
    }

    /// <summary>매 프레임 호출: 시간 업데이트 및 주기 전환 감지</summary>
    public void Tick(float deltaTime)
    {
        cycleChanged = false;

        if (cycleDurations.Count == 0)
            return;

        currentTime += deltaTime;
        float currentCycleDuration = cycleDurations[cycleIndex % cycleDurations.Count];

        if (currentTime >= currentCycleDuration)
        {
            currentTime -= currentCycleDuration;
            cycleIndex++;
            cycleChanged = true;
        }
    }

    /// <summary>현재가 낮인지 밤인지 여부 (index % 2 == 0 이면 낮)</summary>
    public bool IsDay => (cycleIndex % 2) == 0;

    /// <summary>현재 주기 내에서의 진행률 (0.0 ~ 1.0)</summary>
    public float CurrentProgress
    {
        get
        {
            if (cycleDurations.Count == 0)
                return 0f;

            float currentCycleDuration = cycleDurations[cycleIndex % cycleDurations.Count];
            return currentCycleDuration > 0 ? currentTime / currentCycleDuration : 0f;
        }
    }

    /// <summary>현재 몇 번째 주기인지에 대한 인덱스</summary>
    public int CurrentCycleIndex => cycleIndex;

    /// <summary>이번 Tick에서 주기 전환 발생 여부</summary>
    public bool CycleChanged => cycleChanged;
}