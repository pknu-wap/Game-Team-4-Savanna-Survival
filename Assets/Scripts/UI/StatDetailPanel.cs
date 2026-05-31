using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class StatDetailPanel : MonoBehaviour
{
    private PlayerStatCore statCore;
    private bool isPanelOpen = false;

    [Header("연결 대상")]
    [SerializeField] private PlayerStatManager playerStatManager;
    [SerializeField] private GameObject statDetailPanel;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private ItemInfoUI itemInfoUI;

    [Header("스탯 텍스트")]
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text defenseText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text hungerText;
    [SerializeField] private TMP_Text expText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text moveSpeedText;
    [SerializeField] private TMP_Text skillDamageText;
    [SerializeField] private TMP_Text skillCooldownText;
    [SerializeField] private TMP_Text healthRegenText;

    private void Start()
    {
        statCore = playerStatManager.StatCore;
        statCore.onStatRegistered += onStatRegistered; //스탯갱신용 이벤트연결
        statDetailPanel.SetActive(false); //스탯창 끄기
        inventoryPanel.SetActive(false);
    }
    private void OnDestroy() //이벤트 해제용
    {
        if (statCore != null)
        {
            statCore.onStatRegistered -= onStatRegistered;
        }
    }

    /*
    public void OnStatAndInventory(InputValue value) //InputSystem 스탯창열기
    {
        Debug.Log("OnStatAndInventory 호출됨");
        if (value.isPressed == false)
        {
            return;
        }

        toggleStatDetailPanel();
    } 
    */

    public void toggleStatDetailPanel() //창 on off
    {
        // Debug.Log("toggleStatDetailPanel 호출됨");
        isPanelOpen = !isPanelOpen;
        statDetailPanel.SetActive(isPanelOpen);
        inventoryPanel.SetActive(isPanelOpen);

        if (isPanelOpen) //창열때 스탯갱신
        {
            refreshAllStatTexts();
        }
        else
        {
            if (itemInfoUI != null)
            {
                itemInfoUI.hideEquipmentInfo();
            }
        }
    }

    private void onStatRegistered(StatType statType, float value) //이벤트 연결
    {
        if (isPanelOpen == false)
        {
            return;
        }

        refreshAllStatTexts();
    }

    private void refreshAllStatTexts()
    {
        float damage = statCore.getStat(StatType.DAMAGE).rawValue;
        float defense = statCore.getStat(StatType.DEFENSE).rawValue;

        float currentHealth = statCore.getStat(StatType.HEALTH).rawValue;
        float maxHealth = statCore.getStat(StatType.MAX_HEALTH).rawValue;

        float currentHunger = statCore.getStat(StatType.HUNGER).rawValue;
        float maxHunger = statCore.getStat(StatType.MAX_HUNGER).rawValue;

        float currentExp = statCore.getStat(StatType.EXP).rawValue;
        float maxExp = statCore.getStat(StatType.MAX_EXP).rawValue;

        float level = statCore.getStat(StatType.LEVEL).rawValue;
        float moveSpeed = statCore.getStat(StatType.MOVESPEED).rawValue;
        float skillDamage = statCore.getStat(StatType.SKILL_DAMAGE).rawValue;
        float skillCooldown = statCore.getStat(StatType.SKILL_COOLDOWN).rawValue;
        float healthRegen = statCore.getStat(StatType.HEALTH_REGEN).rawValue;

        damageText.text = "공격력 : " + formatStat(damage);
        defenseText.text = "방어력 : " + formatStat(defense);
        healthText.text = "체력 : " + formatStat(currentHealth) + " / " + formatStat(maxHealth);
        hungerText.text = "배고픔 : " + formatStat(currentHunger) + " / " + formatStat(maxHunger);
        expText.text = "Exp : " + formatStat(currentExp) + " / " + formatStat(maxExp);
        levelText.text = "LV : " + formatStat(level);
        moveSpeedText.text = "이동속도 : " + formatStat(moveSpeed);
        skillDamageText.text = "스킬 공격력 : " + formatStat(skillDamage);
        skillCooldownText.text = "스킬 가속 : " + formatStat(skillCooldown);
        healthRegenText.text = "체력 재생 : " + formatStat(healthRegen);
    }

    private string formatStat(float value)
    {
        return value.ToString("0.##");
    }
}
