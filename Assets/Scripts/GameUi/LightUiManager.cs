using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightUiManager : MonoBehaviour
{
    [Serializable]
    public class DayNightDuration
    {
        public CycleDuration dayTime = new CycleDuration(1, 0);
        public CycleDuration nightTime = new CycleDuration(1, 0);
    }

    [Header("References")]
    [SerializeField] private Image nightPanel;

    [Header("Duration Settings")]
    [SerializeField] private List<DayNightDuration> durations = new()
    {
        new DayNightDuration()
    };

    [Header("Color Settings")]
    [SerializeField] private Color dayColor = new Color(0, 0, 0, 0);
    [SerializeField] private Color nightColor = new Color(0, 0, 0, 0.7f);

    private void Update()
    {
        if (nightPanel == null) return;
        if (TimeManager.Instance == null) return;
        if (durations == null || durations.Count == 0) return;

        bool isDay = TimeManager.Instance.IsDay;

        nightPanel.color = isDay
            ? dayColor
            : nightColor;
    }
}