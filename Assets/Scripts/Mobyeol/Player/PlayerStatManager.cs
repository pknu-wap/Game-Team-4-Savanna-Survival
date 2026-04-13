using System.Collections.Generic;
using UnityEngine;

public class PlayerStatManager : MonoBehaviour
{
    private PlayerStatCore statCore; //StatManager와 연결.

    [Header("초기 스탯")]
    public float startDamage = 10f;
    public float startDefense = 0f;
    public float startHealth = 100f;
    public float startMaxHealth = 100f;
    public float startLevel = 1f;
    public float startHunger = 50f;
    public float startExp = 0f;
    public float startMaxExp = 100f;
    public float startMoveSpeed = 5f;
    public float startSkillDamage = 20f;
    public float startSkillCooldown = 1f;

    [Header("현재 스탯")]
    public float playerDamage;
    public float playerDefense;
    public float playerHealth;
    public float playerMaxHealth;
    public float playerLevel;
    public float playerHunger;
    public float playerExp;
    public float playerMaxExp;
    public float playerMoveSpeed;
    public float playerSkillDamage;
    public float playerSkillCooldown;

    private void Awake()
    {
        statCore = new PlayerStatCore();
        statCore.onStatRegistered += OnStatRegistered; //이벤트 함수 연결. 인스펙터 시각적 갱신 위해서

        RegisterDefaultStats();
        RefreshInspectorValue();
    }

    private void RegisterDefaultStats() //시작스탯 등록
    {
        statCore.registerStat(StatType.DAMAGE, startDamage);
        statCore.registerStat(StatType.DEFENSE, startDefense);
        statCore.registerStat(StatType.HEALTH, startHealth);
        statCore.registerStat(StatType.MAX_HEALTH, startMaxHealth);
        statCore.registerStat(StatType.LEVEL, startLevel);
        statCore.registerStat(StatType.HUNGER, startHunger);
        statCore.registerStat(StatType.EXP, startExp);
        statCore.registerStat(StatType.MAX_EXP, startMaxExp);
        statCore.registerStat(StatType.MOVESPEED, startMoveSpeed);
        statCore.registerStat(StatType.SKILL_DAMAGE, startSkillDamage);
        statCore.registerStat(StatType.SKILL_COOLDOWN, startSkillCooldown);
    }

    private void OnStatRegistered(StatType statType, float value) //이벤트 연결용 함수
    {
        RefreshInspectorValue();
    }

    private void RefreshInspectorValue() //인스펙터 반영
    {
        // playerDamage = statCore.getRawStat(StatType.DAMAGE);
        // playerDefense = statCore.getRawStat(StatType.DEFENSE);
        // playerHealth = statCore.getRawStat(StatType.HEALTH);
        // playerMaxHealth = statCore.getRawStat(StatType.MAX_HEALTH);
        // playerLevel = statCore.getRawStat(StatType.LEVEL);
        // playerHunger = statCore.getRawStat(StatType.HUNGER);
        // playerExp = statCore.getRawStat(StatType.EXP);
        // playerMaxExp = statCore.getRawStat(StatType.MAX_EXP);
        // playerMoveSpeed = statCore.getRawStat(StatType.MOVESPEED);
        // playerSkillDamage = statCore.getRawStat(StatType.SKILL_DAMAGE);
        // playerSkillCooldown = statCore.getRawStat(StatType.SKILL_COOLDOWN);
    }

    //장비스탯 리스트의 스탯 반영
    public void ApplyEquipmentList(List<EquipmentStat> equipmentStats) 
    {
        foreach (EquipmentStat equipmentStat in equipmentStats) 
        {
            // statCore.addStat(equipmentStat.statType, equipmentStat.value);
        }
        RefreshInspectorValue();
    }
}