using UnityEngine;
using UnityEngine.UI;

public class SkillTooltip : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Text nameText;
    [SerializeField] private Text typeText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text cooldownText;
    [SerializeField] private Text costText;

    private Canvas parentCanvas;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        Hide();
    }

    public void Show(BaseSkillData skillData, RectTransform nodeRect)
    {
        gameObject.SetActive(true);
        PositionNextToNode(nodeRect);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = false;
        }

        UpdateTooltipText(skillData);
    }

    public void Hide()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        gameObject.SetActive(false);
    }

    private void PositionNextToNode(RectTransform nodeRect)
    {
        if (parentCanvas == null) return;

        // 노드 월드 좌표 → 캔버스 로컬 좌표 변환
        Vector3 localPos = parentCanvas.transform.InverseTransformPoint(nodeRect.position);
        float xOffset = nodeRect.sizeDelta.x * 0.5f + 10f;
        transform.localPosition = new Vector3(localPos.x + xOffset, localPos.y, 0f);
    }

    private void UpdateTooltipText(BaseSkillData skillData)
    {
        if (nameText != null)
            nameText.text = skillData.skillName;

        if (typeText != null)
        {
            string type = skillData switch
            {
                ActiveSkillData => "Active",
                AutoSkillData => "Auto",
                PassiveSkillData => "Passive",
                _ => "Unknown"
            };
            typeText.text = type;
        }

        if (descriptionText != null)
            descriptionText.text = skillData.description;

        if (costText != null)
            costText.text = $"Cost: {skillData.cost}";

        if (cooldownText != null)
        {
            string cooldown = skillData switch
            {
                ActiveSkillData active => $"Cooldown: {active.cooldown}s",
                AutoSkillData auto => $"Interval: {auto.interval}s",
                _ => ""
            };
            cooldownText.text = cooldown;
        }
    }
}
