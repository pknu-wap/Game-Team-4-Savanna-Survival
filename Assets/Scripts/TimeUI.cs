using UnityEngine;
using TMPro;

public class TimeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private float dayDuration = 150f;
    [SerializeField] private float nightDuration = 180f;

    private float accumulatedTime = 0f;

    private void Start()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTimeStateChanged += OnCycleChanged;
        }
    }

    private void Update()
    {
        if (TimeManager.Instance == null) return;

        float progress = TimeManager.Instance.CurrentTimeProgress;

        float currentCycleDuration = TimeManager.Instance.IsDay ? dayDuration : nightDuration;

        float currentCycleTime = progress * currentCycleDuration;

        float totalTime = accumulatedTime + currentCycleTime;

        int minutes = Mathf.FloorToInt(totalTime / 60f);
        int seconds = Mathf.FloorToInt(totalTime % 60f);

        timeText.text = $"Time: {minutes:00}:{seconds:00}";
    }

    private void OnCycleChanged(bool isDay)
    {
        float previousDuration = isDay ? nightDuration : dayDuration;
        accumulatedTime += previousDuration;
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTimeStateChanged -= OnCycleChanged;
        }
    }
}