using System.Collections.Generic;
using UnityEngine;

public class PlayerStatManager : MonoBehaviour
{
    private PlayerStatCore statCore;
    public PlayerStatCore StatCore  //StatManager
{
    get { return statCore; }
}

    [Header("초기 스탯")]
    public float startDamage = 10f;
    public float startDefense = 0f;
    public float startHealth = 50f;
    public float startMaxHealth = 100f;
    public float startLevel = 1f;
    public float startHunger = 50f;
    public float startMaxHunger = 50f;
    public float startExp = 0f;
    public float startMaxExp = 100f;
    public float startMoveSpeed = 10f;
    public float startSkillDamage = 20f;
    public float startSkillCooldown = 1f;
    public float startHealthRegen = 0.5f;

    [Header("현재 스탯")]
    public float playerDamage;
    public float playerDefense;
    public float playerHealth;
    public float playerMaxHealth;
    public float playerLevel;
    public float playerHunger;
    public float playerMaxHunger;
    public float playerExp;
    public float playerMaxExp;
    public float playerMoveSpeed;
    public float playerSkillDamage;
    public float playerSkillCooldown;
    public float playerHealthRegen;

    private void Awake()
    {
        statCore = new PlayerStatCore();

        registerDefaultStats();
        refreshInspectorValue();

        statCore.onStatRegistered += onStatRegistered; //이벤트 함수 연결. 인스펙터 시각적 갱신 위해서
    }

    private void registerDefaultStats() //시작스탯 등록
    {
        statCore.registerStat(StatType.DAMAGE, startDamage);
        statCore.registerStat(StatType.DEFENSE, startDefense);
        statCore.registerStat(StatType.HEALTH, startHealth);
        statCore.registerStat(StatType.MAX_HEALTH, startMaxHealth);
        statCore.registerStat(StatType.LEVEL, startLevel);
        statCore.registerStat(StatType.HUNGER, startHunger);
        statCore.registerStat(StatType.MAX_HUNGER, startMaxHunger);
        statCore.registerStat(StatType.EXP, startExp);
        statCore.registerStat(StatType.MAX_EXP, startMaxExp);
        statCore.registerStat(StatType.MOVESPEED, startMoveSpeed);
        statCore.registerStat(StatType.SKILL_DAMAGE, startSkillDamage);
        statCore.registerStat(StatType.SKILL_COOLDOWN, startSkillCooldown);
        statCore.registerStat(StatType.HEALTH_REGEN, startHealthRegen);
    }

    private void onStatRegistered(StatType statType, float value) //이벤트 연결용 함수
    {
        refreshInspectorValue();
    }

    private void refreshInspectorValue()
    {
        playerDamage = statCore.getStat(StatType.DAMAGE).rawValue;
        playerDefense = statCore.getStat(StatType.DEFENSE).rawValue;
        playerHealth = statCore.getStat(StatType.HEALTH).rawValue;
        playerMaxHealth = statCore.getStat(StatType.MAX_HEALTH).rawValue;
        playerLevel = statCore.getStat(StatType.LEVEL).rawValue;
        playerHunger = statCore.getStat(StatType.HUNGER).rawValue;
        playerMaxHunger = statCore.getStat(StatType.MAX_HUNGER).rawValue;
        playerExp = statCore.getStat(StatType.EXP).rawValue;
        playerMaxExp = statCore.getStat(StatType.MAX_EXP).rawValue;
        playerMoveSpeed = statCore.getStat(StatType.MOVESPEED).rawValue;
        playerSkillDamage = statCore.getStat(StatType.SKILL_DAMAGE).rawValue;
        playerSkillCooldown = statCore.getStat(StatType.SKILL_COOLDOWN).rawValue;
        playerHealthRegen = statCore.getStat(StatType.HEALTH_REGEN).rawValue;
    }

    //장비스탯 리스트의 스탯 반영
    public void applyEquipmentStat(List<EquipmentStat> equipmentStats) 
    {
        foreach (EquipmentStat equipmentStat in equipmentStats) 
        {
            statCore.addStat(equipmentStat.statType, equipmentStat.value);
        }
        refreshInspectorValue();
    }

    public void removeEquipmentStat(List<EquipmentStat> equipmentStats)
    {
        foreach (EquipmentStat equipmentStat in equipmentStats) 
        {
            statCore.addStat(equipmentStat.statType, -equipmentStat.value);
        }
        refreshInspectorValue();
    }
}