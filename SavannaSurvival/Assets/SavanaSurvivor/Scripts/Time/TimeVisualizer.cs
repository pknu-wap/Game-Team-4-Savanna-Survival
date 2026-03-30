using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Layer 3: TimeVisualizer - 환경 연출 및 동기화
/// 낮/밤 상태에 따라 카메라 배경색 및 UI 어둠 효과를 시각적으로 변화시킵니다.
/// </summary>
public class TimeVisualizer : MonoBehaviour
{
    [Header("Lighting Settings")]
    [SerializeField] private Color dayLightColor = Color.white;
    [SerializeField] private float dayLightIntensity = 1.0f;
    [SerializeField] private Color nightLightColor = new(0.3f, 0.3f, 0.5f);
    [SerializeField] private float nightLightIntensity = 0.3f;

    [Header("Night Darkening UI")]
    [SerializeField] private float maxNightAlpha = 0.5f;

    [Header("UI Settings")]
    [SerializeField] private GameObject dayIcon;
    [SerializeField] private GameObject nightIcon;

    private Image darkenImage;
    private bool isCurrentlyDay = true;

    private void Start()
    {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;

        // Canvas와 darkening Image 찾기
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas != null)
        {
            darkenImage = canvas.GetComponentInChildren<Image>();
            if (darkenImage == null)
            {
                Debug.LogWarning("No Image found in Canvas for nighttime darkening");
            }
            else
            {
                // 초기 상태 설정
                Color imageColor = darkenImage.color;
                imageColor.a = 0f;
                darkenImage.color = imageColor;
            }
        }
        else
        {
            Debug.LogWarning("No Canvas found in scene for TimeVisualizer");
        }

        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTimeStateChanged += OnTimeStateChanged;
            isCurrentlyDay = TimeManager.Instance.IsDay;
            UpdateUI();
        }
        else
        {
            Debug.LogError("TimeManager instance not available for TimeVisualizer");
        }
    }

    private void Update()
    {
        if (TimeManager.Instance == null)
            return;

        UpdateLighting();
    }

    private void UpdateLighting()
    {
        Color targetColor = isCurrentlyDay ? dayLightColor : nightLightColor;
        float targetIntensity = isCurrentlyDay ? dayLightIntensity : nightLightIntensity;
        Color targetBGColor = targetColor * targetIntensity;

        // 1. Camera backgroundColor Lerp (배경색 변화)
        if (Camera.main != null)
        {
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, targetBGColor, Time.deltaTime * 2f);
        }

        // 2. UI 어둠 효과: 밤에는 alpha 올리고, 낮에는 내림
        if (darkenImage != null)
        {
            float targetAlpha = isCurrentlyDay ? 0f : maxNightAlpha;
            Color imageColor = darkenImage.color;
            imageColor.a = Mathf.Lerp(imageColor.a, targetAlpha, Time.deltaTime * 2f);
            darkenImage.color = imageColor;
        }
    }

    private void OnTimeStateChanged(bool isDay)
    {
        isCurrentlyDay = isDay;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (dayIcon != null)
            dayIcon.SetActive(isCurrentlyDay);
        if (nightIcon != null)
            nightIcon.SetActive(!isCurrentlyDay);
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTimeStateChanged -= OnTimeStateChanged;
        }
    }
}
