using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Layer 2: TimeManager - 상태 및 이벤트 허브
/// TimeTicker를 내부에 보유하고 싱글톤 인터페이스를 통해 시간 상태를 제공하며,
/// 상태 변화 시 이벤트를 발생시킵니다.
/// </summary>
public class TimeManager : MonoBehaviour
{
    [SerializeField] private List<CycleDuration> cycleDurations = new()
    {
        new(2, 30),  // 낮: 2분 30초
        new(3, 0)    // 밤: 3분
    };

    private TimeTicker timeTicker = new();
    private static TimeManager instance;

    public static TimeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<TimeManager>();
                if (instance == null)
                {
                    Debug.LogError("TimeManager not found in scene!");
                }
            }
            return instance;
        }
    }

    /// <summary>현재 낮/밤 상태 (true: 낮, false: 밤)</summary>
    public bool IsDay => timeTicker.IsDay;

    /// <summary>현재 주기 내에서의 진행률 (0.0 ~ 1.0)</summary>
    public float CurrentTimeProgress => timeTicker.CurrentProgress;

    /// <summary>낮/밤 전환 시 호출되는 이벤트</summary>
    public event Action<bool> OnTimeStateChanged;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        timeTicker.Initialize(cycleDurations);
    }

    private void Update()
    {
        timeTicker.Tick(Time.deltaTime);

        if (timeTicker.CycleChanged)
        {
            OnTimeStateChanged?.Invoke(IsDay);
        }
    }
}