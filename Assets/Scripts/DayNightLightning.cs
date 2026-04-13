using UnityEngine;
using UnityEngine.UI;

public class DayNightPanelController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image nightPanel;

    [Header("Time Settings")]
    [SerializeField] private float dayTime = 60f;
    [SerializeField] private float nightTime = 40f;
    [SerializeField] private float transitionTime = 10f;

    [Header("Color Settings")]
    [SerializeField] private Color dayColor = new Color(0, 0, 0, 0);
    [SerializeField] private Color nightColor = new Color(0, 0, 0, 0.7f);

    private float timer;

    private enum State
    {
        Day,
        ToNight,
        Night,
        ToDay
    }

    private State currentState = State.Day;

    void Update()
    {
        timer += Time.deltaTime;

        switch (currentState)
        {
            case State.Day:
                nightPanel.color = dayColor;
                if (timer >= dayTime)
                {
                    timer = 0f;
                    currentState = State.ToNight;
                }
                break;

            case State.ToNight:
                nightPanel.color = Color.Lerp(dayColor, nightColor, timer / transitionTime);
                if (timer >= transitionTime)
                {
                    timer = 0f;
                    currentState = State.Night;
                }
                break;

            case State.Night:
                nightPanel.color = nightColor;
                if (timer >= nightTime)
                {
                    timer = 0f;
                    currentState = State.ToDay;
                }
                break;

            case State.ToDay:
                nightPanel.color = Color.Lerp(nightColor, dayColor, timer / transitionTime);
                if (timer >= transitionTime)
                {
                    timer = 0f;
                    currentState = State.Day;
                }
                break;
        }
    }
}