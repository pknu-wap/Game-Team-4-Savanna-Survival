using TMPro;
using UnityEngine;

public class TimeUIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text timeText;

    [Header("Cycle Duration")]
    [SerializeField] private int dayMinutes = 1;
    [SerializeField] private int daySeconds = 0;

    [SerializeField] private int nightMinutes = 1;
    [SerializeField] private int nightSeconds = 0;

    private void Update()
    {
        if (TimeManager.Instance == null) return;

        bool isDay = TimeManager.Instance.IsDay;
        float progress = TimeManager.Instance.CurrentTimeProgress;

        float totalSeconds = isDay
            ? dayMinutes * 60f + daySeconds
            : nightMinutes * 60f + nightSeconds;

        float remainingSeconds = totalSeconds * (1f - progress);

        int minutes = Mathf.FloorToInt(remainingSeconds / 60f);
        int seconds = Mathf.FloorToInt(remainingSeconds % 60f);

        string state = isDay ? "DAY" : "NIGHT";

        timeText.text = $"{state} {minutes:00}:{seconds:00}";
    }
}