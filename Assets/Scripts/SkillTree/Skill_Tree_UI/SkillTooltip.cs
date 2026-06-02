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
    [SerializeField] private GameObject cooldownRow;

    private Canvas parentCanvas;
    private RectTransform tooltipRect;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        tooltipRect = GetComponent<RectTransform>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        Hide();
    }

    public void Show(BaseSkillData skillData, RectTransform nodeRect)
    {
        // skillData가 없는(빈) 노드면 툴팁을 띄우지 않는다 (NRE 방지)
        if (skillData == null)
        {
            Hide();
            return;
        }

        gameObject.SetActive(true);
        UpdateTooltipContent(skillData);

        Canvas.ForceUpdateCanvases();

        PositionNextToNode(nodeRect);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void Hide()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        gameObject.SetActive(false);
    }

    private void PositionNextToNode(RectTransform nodeRect)
    {
        if (parentCanvas == null || tooltipRect == null) return;

        Vector3 nodeLocalPos = parentCanvas.transform.InverseTransformPoint(nodeRect.position);
        float nodeHalfWidth = nodeRect.sizeDelta.x * 0.5f;
        float tooltipHalfWidth = tooltipRect.sizeDelta.x * 0.5f;
        float gap = 10f;

        RectTransform canvasRect = parentCanvas.GetComponent<RectTransform>();
        float canvasHalfWidth = canvasRect.sizeDelta.x * 0.5f;

        float rightX = nodeLocalPos.x + nodeHalfWidth + gap + tooltipHalfWidth;
        float leftX  = nodeLocalPos.x - nodeHalfWidth - gap - tooltipHalfWidth;

        float finalX = (rightX + tooltipHalfWidth <= canvasHalfWidth) ? rightX : leftX;

        tooltipRect.localPosition = new Vector3(finalX, nodeLocalPos.y, 0f);
    }

    private void UpdateTooltipContent(BaseSkillData skillData)
    {
        if (nameText != null)
            nameText.text = skillData.skillName;

        if (typeText != null)
        {
            typeText.text = skillData switch
            {
                ActiveSkillData => "Active",
                AutoSkillData   => "Auto",
                PassiveSkillData => "Passive",
                _ => "Unknown"
            };
        }

        if (descriptionText != null)
            descriptionText.text = skillData.description;

        if (costText != null)
            costText.text = $"Cost: {skillData.cost}";

        bool hasCooldown = skillData is ActiveSkillData || skillData is AutoSkillData;

        if (cooldownRow != null)
            cooldownRow.SetActive(hasCooldown);

        if (cooldownText != null && hasCooldown)
        {
            cooldownText.text = skillData switch
            {
                ActiveSkillData active => $"Cooldown: {active.cooldown}s",
                AutoSkillData auto     => $"Interval: {auto.interval}s",
                _ => ""
            };
        }
    }
}
