using TMPro;
using UnityEngine;

public class PlayerStatUiManager : MonoBehaviour
{
    [SerializeField] private PlayerStatManager playerStatManager;

    [Header("Stat Texts")]
[SerializeField] private TMP_Text damageText;
[SerializeField] private TMP_Text healthText;
[SerializeField] private TMP_Text hungerText;
[SerializeField] private TMP_Text moveSpeedText;
[SerializeField] private TMP_Text defenseText;
[SerializeField] private TMP_Text healthRegenText;
[SerializeField] private TMP_Text skillDamageText;
[SerializeField] private TMP_Text skillCooldownText;
    // 텍스트를 플레이어 스탯에서 가져옴

    private PlayerStatCore statCore;
    //플레이어 스탯코어 지정

    private void OnEnable()
    {
        if (playerStatManager == null)
            playerStatManager = FindObjectOfType<PlayerStatManager>();
            //인스펙터 미연결 자동으로 스탯매니저에서 값을 찾아줌
        statCore = playerStatManager.StatCore;
        statCore.onStatRegistered += OnStatChanged;

        RefreshUI();
    }

    private void OnDisable()
    {
        if (statCore != null)
            statCore.onStatRegistered -= OnStatChanged;
    }

    private void OnStatChanged(StatType statType, float value)
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
    float health = statCore.getStat(StatType.HEALTH).rawValue;
    float maxHealth = statCore.getStat(StatType.MAX_HEALTH).rawValue;
    float hunger = statCore.getStat(StatType.HUNGER).rawValue;
    float maxHunger = statCore.getStat(StatType.MAX_HUNGER).rawValue;
    // 스탯 코어에서 값들을 가져옴

    damageText.text = $"Damage : {statCore.getStat(StatType.DAMAGE).rawValue:0.##}";
    healthText.text = $"Health : {health:0} / {maxHealth:0}";
    hungerText.text = $"Hunger : {hunger:0} / {maxHunger:0}";
    moveSpeedText.text = $"Speed : {statCore.getStat(StatType.MOVESPEED).rawValue:0.##}";
    defenseText.text = $"Defanse : {statCore.getStat(StatType.DEFENSE).rawValue:0.##}";
    healthRegenText.text = $"Health Regen : {statCore.getStat(StatType.HEALTH_REGEN).rawValue:0.##}";
    skillDamageText.text = $"Skill Damage : {statCore.getStat(StatType.SKILL_DAMAGE).rawValue:0.##}";
    skillCooldownText.text = $"Skill CoolTime : {statCore.getStat(StatType.SKILL_COOLDOWN).rawValue:0.#}s";
        // 스탯코어의 값을 텍스트에 반영함
    }
}