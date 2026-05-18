using UnityEngine;
using TMPro;

public class ItemInfoUI : MonoBehaviour
{
    [Header("Script")]
    [SerializeField] private EquipmentInventory equipmentInventory;
    [SerializeField] private GameObject ItemInfoPanel;

    [Header("UI")]
    [SerializeField] private TMP_Text itemName;
    // [SerializeField] private TMP_Text itemType; 미구현(1)
    [SerializeField] private TMP_Text[] statTypes;
    [SerializeField] private TMP_Text[] statValues;

    private EquipmentData currentEquipment;

    private void Awake()
    {
        ItemInfoPanel.SetActive(false);
    }
    
    public void openEquipmentInfo(EquipmentData equipment)
    {
        ItemInfoPanel.SetActive(true);
        setInfoText(equipment);

    }

    public void hideEquipmentInfo()
    {
        ItemInfoPanel.SetActive(false);
    }

    private string getStatName(StatType statType)
    {
        return statType switch
        {
            StatType.DAMAGE => "공격력",
            StatType.DEFENSE => "방어력",
            StatType.HEALTH => "체력",
            StatType.MAX_HEALTH => "최대 체력",
            StatType.MOVESPEED => "이동속도",
            StatType.SKILL_DAMAGE => "스킬 공격력",
            StatType.SKILL_COOLDOWN => "스킬 가속",
            StatType.HEALTH_REGEN => "체력 재생",
            StatType.HUNGER => "허기",
            _ => statType.ToString(),
        };
    }

    private void setInfoText(EquipmentData equipment)
    {
        itemName.text = equipment.equipmentName;
        // itemType = equipment.equipmentType; 미구현(1)
        for (int i = 0; i < statTypes.Length; ++i)
        {
            if (i < equipment.equipmentStats.Count)
            {
                EquipmentStat tempEquipmentStat = equipment.equipmentStats[i];

                statTypes[i].gameObject.SetActive(true);
                statTypes[i].text = getStatName(tempEquipmentStat.statType);
                statValues[i].gameObject.SetActive(true);
                statValues[i].text = tempEquipmentStat.value.ToString("0.##");
            }
            else
            {
                statTypes[i].gameObject.SetActive(false);
                statValues[i].gameObject.SetActive(false);
            }
        }
    }
}
