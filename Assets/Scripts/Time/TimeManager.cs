using System;
using System.Collections.Generic;
using UnityEngine;

// Layer 2: TimeManager - 상태 및 이벤트 허브
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

    // 현재 낮/밤 상태 (true: 낮, false: 밤)
    public bool IsDay => timeTicker.IsDay;

    // 현재 주기 내에서의 진행률 (0.0 ~ 1.0)
    public float CurrentTimeProgress => timeTicker.CurrentProgress;

    // 현재 주기 인덱스
    public int CurrentCycleIndex => timeTicker.CurrentCycleIndex;

    private int nightCount = 0;
    // 지금까지 진행된 밤의 횟수
    public int NightCount => nightCount;

    // 낮/밤 전환 시 호출되는 이벤트
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
            if (!IsDay) // 밤으로 전환될 때만 카운트
                nightCount++;
        }
    }
}
