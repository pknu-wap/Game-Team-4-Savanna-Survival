using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeUIManager : MonoBehaviour
{
    [Serializable]
    public class DayNightDuration
    {
        public CycleDuration dayTime = new CycleDuration(1, 0);
        public CycleDuration nightTime = new CycleDuration(1, 0);
    }

    [Header("UI Texts")]
    [SerializeField] private TMP_Text currentDayText;
    [SerializeField] private TMP_Text runTimeText;
    [SerializeField] private TMP_Text morningOrNightText;

    [Header("Duration Settings")]
    [SerializeField] private List<DayNightDuration> durations = new()
    {
        new DayNightDuration()
    };

    private void Update()
    {
        if (TimeManager.Instance == null) return;
        if (durations == null || durations.Count == 0) return;

        TimeManager tm = TimeManager.Instance;

        int cycleIndex = tm.CurrentCycleIndex;
        int currentDay = cycleIndex / 2 + 1;
        int dayIndex = Mathf.Clamp(currentDay - 1, 0, durations.Count - 1);

        bool isDay = tm.IsDay;
        float progress = tm.CurrentTimeProgress;

        float currentCycleSeconds = isDay
            ? durations[dayIndex].dayTime.TotalSeconds
            : durations[dayIndex].nightTime.TotalSeconds;

        float currentLeftSeconds = currentCycleSeconds * (1f - progress);

        float elapsedSeconds = GetElapsedSeconds(cycleIndex, progress, isDay);

        if (currentDayText != null)
            currentDayText.text = $"Day: {currentDay}";

        if (runTimeText != null)
            runTimeText.text = $"Run Time: {FormatTime(elapsedSeconds)}";

        if (morningOrNightText != null)
        {
            string state = isDay ? "Morning" : "Night";
            morningOrNightText.text = $"{state}: {FormatTime(currentLeftSeconds)}";
        }
    }

    private float GetElapsedSeconds(int cycleIndex, float progress, bool isDay)
    {
        float elapsed = 0f;

        for (int i = 0; i < cycleIndex; i++)
        {
            int dayIndex = i / 2;

            if (dayIndex >= durations.Count)
                break;

            elapsed += i % 2 == 0
                ? durations[dayIndex].dayTime.TotalSeconds
                : durations[dayIndex].nightTime.TotalSeconds;
        }

        int currentDayIndex = Mathf.Clamp(cycleIndex / 2, 0, durations.Count - 1);

        float currentCycleSeconds = isDay
            ? durations[currentDayIndex].dayTime.TotalSeconds
            : durations[currentDayIndex].nightTime.TotalSeconds;

        elapsed += currentCycleSeconds * progress;

        return elapsed;
    }

    private string FormatTime(float time)
    {
        int totalSeconds = Mathf.CeilToInt(time);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        return $"{minutes:00}:{seconds:00}";
    }
}