using UnityEngine;
using System.Collections.Generic;
using System;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    private HashSet<BaseSkillData> unlockedSkills = new();
    private PlayerStatManager playerStatManager;
    
    public event Action<BaseSkillData> OnSkillUnlocked;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        playerStatManager = FindObjectOfType<PlayerStatManager>();
        if (playerStatManager == null)
        {
            Debug.LogError("PlayerStatManager not found in scene!");
            return;
        }
    }

    public bool TryUnlockSkill(BaseSkillData skill)
    {
        if (skill == null) return false;
        if (unlockedSkills.Contains(skill)) return false;

        // 선행 스킬 확인
        foreach (var prerequisite in skill.prerequisites)
        {
            if (!unlockedSkills.Contains(prerequisite))
            {
                return false;
            }
        }

        // 포인트 확인
        int currentPoints = GetCurrentPoints();
        if (currentPoints < skill.cost) return false;

        // 포인트 소모
        playerStatManager.StatCore.addStat(StatType.SKILL_POINTS, -skill.cost);

        // 해금 목록에 추가
        unlockedSkills.Add(skill);

        // 패시브 효과 적용
        if (skill is PassiveSkillData passiveSkill)
        {
            GameObject player = playerStatManager.gameObject;
            foreach (var effect in passiveSkill.effects)
            {
                effect.Apply(player);
            }
        }

        OnSkillUnlocked?.Invoke(skill);
        return true;
    }

    public bool IsUnlocked(BaseSkillData skill)
    {
        return unlockedSkills.Contains(skill);
    }

    public int GetCurrentPoints()
    {
        if (playerStatManager == null) return 0;

        try
        {
            var skillPointsStat = playerStatManager.StatCore.getStat(StatType.SKILL_POINTS);
            if (skillPointsStat == null) return 0;
            return (int)skillPointsStat.rawValue;
        }
        catch (System.Collections.Generic.KeyNotFoundException)
        {
            return 0;
        }
    }

    public void ResetRun()
    {
        // 모든 패시브 효과 제거
        if (playerStatManager != null)
        {
            GameObject player = playerStatManager.gameObject;
            foreach (var skill in unlockedSkills)
            {
                if (skill is PassiveSkillData passiveSkill)
                {
                    foreach (var effect in passiveSkill.effects)
                    {
                        effect.Remove(player);
                    }
                }
                
                if (skill is ActiveSkillData activeSkill && activeSkill.action != null)
                {
                    activeSkill.action.Clear(player);
                }
                
                if (skill is AutoSkillData autoSkill && autoSkill.action != null)
                {
                    autoSkill.action.Clear(player);
                }
            }
        }

        unlockedSkills.Clear();
        
        if (playerStatManager != null)
        {
            playerStatManager.StatCore.registerStat(StatType.SKILL_POINTS, playerStatManager.startSkillPoints);
        }
    }
}
