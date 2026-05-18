using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillTreeNodeUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private BaseSkillData skillData;
    [SerializeField] private Image iconImage;
    [SerializeField] private Text skillNameText;
    [SerializeField] private Color lockedColor = Color.gray;
    [SerializeField] private Color unlockedColor = Color.white;
    [SerializeField] private Color completedColor = Color.cyan;

    private float holdTime = 0f;
    private bool isHolding = false;
    private Vector2 originalLocalPosition;
    private const float HOLD_DURATION = 2f;

    private SkillTooltip tooltip;
    private Button button;

    public BaseSkillData SkillData => skillData;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.interactable = true;
        }

        tooltip = GetComponentInParent<SkillTreeWindow>()?.GetComponentInChildren<SkillTooltip>(true);
        if (tooltip == null)
        {
            tooltip = FindObjectOfType<SkillTooltip>(true);
        }
    }

    private void Start()
    {
        if (skillData != null)
        {
            GetComponent<RectTransform>().anchoredPosition = skillData.treePosition;
        }
        originalLocalPosition = transform.localPosition;
        UpdateVisualState();
        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.OnSkillUnlocked += OnSkillUnlockedAnywhere;
        }
    }

private void Update()
    {
        if (isHolding)
        {
            holdTime += Time.unscaledDeltaTime;

            ShakeNode();
            ScaleNode();

            if (holdTime >= HOLD_DURATION)
            {
                isHolding = false;
                holdTime = 0f;
                ResetScale();
                OnHoldComplete();
            }
        }
    }

public void OnPointerDown(PointerEventData eventData)
    {
        if (skillData == null || SkillManager.Instance == null) return;
        if (SkillManager.Instance.IsUnlocked(skillData)) return;

        isHolding = true;
        holdTime = 0f;
        originalLocalPosition = transform.localPosition;
    }

public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
        holdTime = 0f;
        ResetScale();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip != null)
        {
            tooltip.Show(skillData, GetComponent<RectTransform>());
        }
    }

public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip != null)
        {
            tooltip.Hide();
        }

        isHolding = false;
        holdTime = 0f;
        ResetScale();
    }

    private void OnHoldComplete()
    {
        bool success = SkillManager.Instance.TryUnlockSkill(skillData);
        
        if (success)
        {
            UpdateVisualState();
        }
        else
        {
            FlashRed();
        }
    }

public void RefreshVisual()
    {
        UpdateVisualState();
    }

    private void UpdateVisualState()
    {
        if (skillData == null || SkillManager.Instance == null) return;

        bool isUnlocked = SkillManager.Instance.IsUnlocked(skillData);
        bool canUnlock = CanUnlock();

        Color targetColor = isUnlocked ? completedColor : (canUnlock ? unlockedColor : lockedColor);

        if (iconImage != null)
        {
            iconImage.color = targetColor;
            if (skillData.icon != null)
                iconImage.sprite = skillData.icon;
        }

        if (skillNameText != null)
        {
            skillNameText.text = skillData.skillName;
        }

        if (button != null)
        {
            button.interactable = !isUnlocked;
        }
    }

private bool CanUnlock()
    {
        if (skillData == null || SkillManager.Instance == null) return false;

        foreach (var prerequisite in skillData.prerequisites)
        {
            if (!SkillManager.Instance.IsUnlocked(prerequisite))
            {
                return false;
            }
        }

        return SkillManager.Instance.GetCurrentPoints() >= skillData.cost;
    }

private void ShakeNode()
    {
        Vector2 offset = Random.insideUnitCircle * 2f;
        transform.localPosition = originalLocalPosition + offset;
    }

    private void ScaleNode()
    {
        float scale = 1f + (holdTime / HOLD_DURATION) * 0.2f;
        transform.localScale = Vector3.one * scale;
    }

private void ResetScale()
    {
        transform.localPosition = originalLocalPosition;
        transform.localScale = Vector3.one;
    }

    private void FlashRed()
    {
        StartCoroutine(FlashCoroutine());
    }

private System.Collections.IEnumerator FlashCoroutine()
    {
        if (iconImage == null) yield break;

        Color originalColor = iconImage.color;
        iconImage.color = Color.red;
        yield return new WaitForSecondsRealtime(0.2f);
        iconImage.color = originalColor;
    }

    private void OnSkillUnlockedAnywhere(BaseSkillData skill)
    {
        UpdateVisualState();
    }

    private void OnDestroy()
    {
        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.OnSkillUnlocked -= OnSkillUnlockedAnywhere;
        }
    }
}
