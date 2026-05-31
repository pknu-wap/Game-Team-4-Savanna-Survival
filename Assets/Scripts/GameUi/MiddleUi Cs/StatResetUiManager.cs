using UnityEngine;

public class StatResetUiManager : MonoBehaviour
{
    [SerializeField] private PlayerAttributeManager playerAttributeManager;

    private AttributeManager attribute;

    private void Start()
    {
        if (playerAttributeManager == null)
            playerAttributeManager = FindObjectOfType<PlayerAttributeManager>();

        attribute = playerAttributeManager.Attribute;
    }

    public void OnClickResetButton()
    {
        ResetStat(StatType.DAMAGE);
        ResetStat(StatType.DEFENSE);
        ResetStat(StatType.MAX_HEALTH);
        ResetStat(StatType.MAX_HUNGER);
        ResetStat(StatType.MOVESPEED);
        ResetStat(StatType.SKILL_DAMAGE);
        ResetStat(StatType.SKILL_COOLDOWN);
        ResetStat(StatType.HEALTH_REGEN);

        Debug.Log("스탯 강화 초기화 완료");
    }

    private void ResetStat(StatType statType)
    {
        float investedPoint = attribute.getAttribute(statType).points;

        if (investedPoint <= 0f)
            return;

        attribute.retrievePoint(statType, investedPoint);
    }
}