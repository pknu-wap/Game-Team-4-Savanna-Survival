using UnityEngine;
using TMPro;

public class TimeUIManager : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField] private float dayTime = 180f;
    [SerializeField] private float nightTime = 200f;

    [Header("UI")]
    public TextMeshProUGUI timeText;

    private float currentProgress = 0f; // 0 ~ 1
    private bool isDay = true;

    private float elapsedSeconds = 0f;

    void Update()
    {
        UpdateTime();
        UpdateUI();
    }

    void UpdateTime()
    {
        float duration = isDay ? dayTime : nightTime;

        currentProgress += Time.deltaTime / duration;

        elapsedSeconds += Time.deltaTime;

        if (currentProgress >= 1f)
        {
            currentProgress = 0f;
            isDay = !isDay;
        }
    }

    void UpdateUI()
    {
        int minutes = Mathf.FloorToInt(elapsedSeconds / 60f);
        int seconds = Mathf.FloorToInt(elapsedSeconds % 60f);

        string state = isDay ? "DAY" : "NIGHT";

        timeText.text = state + " " +
            minutes.ToString("00") + ":" +
            seconds.ToString("00");
    }
}