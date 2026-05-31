using UnityEngine;
using System.Collections.Generic;
using System;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    private HashSet<BaseSkillData> unlockedSkills = new();
    private int wildnessUnlockedCount = 0;
    private int fantasyUnlockedCount = 0;
    public event Action<BaseSkillData> OnSkillRemoved;

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

        foreach (var prerequisite in skill.prerequisites)
        {
            if (!unlockedSkills.Contains(prerequisite))
                return false;
        }

        int currentPoints = GetCurrentPoints();
        if (currentPoints < skill.cost) return false;

        playerStatManager.StatCore.addStat(StatType.SKILL_POINTS, -skill.cost);

        GameObject player = playerStatManager.gameObject;

        if (skill.unlockBehavior == SkillUnlockBehavior.ReplaceGroup && !string.IsNullOrEmpty(skill.skillGroupId))
        {
            BaseSkillData toRemove = null;
            foreach (var s in unlockedSkills)
            {
                if (s.skillGroupId == skill.skillGroupId)
                {
                    toRemove = s;
                    break;
                }
            }
            if (toRemove != null)
            {
                if (toRemove is PassiveSkillData oldPassive)
                    foreach (var effect in oldPassive.effects)
                        effect.Remove(player);
                if (toRemove is ActiveSkillData oldActive)
                    oldActive.action?.Clear(player);
                if (toRemove is AutoSkillData oldAuto)
                    oldAuto.action?.Clear(player);

                OnSkillRemoved?.Invoke(toRemove);
                unlockedSkills.Remove(toRemove);
            }
        }

        unlockedSkills.Add(skill);

        if (skill is PassiveSkillData passiveSkill)
            foreach (var effect in passiveSkill.effects)
                effect.Apply(player);

        if (skill.treeType == SkillTreeType.Wildness) wildnessUnlockedCount++;
        else if (skill.treeType == SkillTreeType.Fantasy) fantasyUnlockedCount++;

        OnSkillUnlocked?.Invoke(skill);
        return true;
    }

public bool ReplaceSkill(BaseSkillData oldSkill, BaseSkillData newSkill)
    {
        if (!unlockedSkills.Contains(oldSkill)) return false;
        if (newSkill == null) return false;

        GameObject player = playerStatManager.gameObject;

        if (oldSkill is PassiveSkillData oldPassive)
            foreach (var effect in oldPassive.effects)
                effect.Remove(player);
        if (oldSkill is ActiveSkillData oldActive)
            oldActive.action?.Clear(player);
        if (oldSkill is AutoSkillData oldAuto)
            oldAuto.action?.Clear(player);

        OnSkillRemoved?.Invoke(oldSkill);
        unlockedSkills.Remove(oldSkill);

        unlockedSkills.Add(newSkill);

        if (newSkill is PassiveSkillData newPassive)
            foreach (var effect in newPassive.effects)
                effect.Apply(player);

        OnSkillUnlocked?.Invoke(newSkill);
        return true;
    }

public SkillTreeType GetDominantSkillTree()
    {
        if (wildnessUnlockedCount > fantasyUnlockedCount) return SkillTreeType.Wildness;
        if (fantasyUnlockedCount > wildnessUnlockedCount) return SkillTreeType.Fantasy;
        return SkillTreeType.None;
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
        if (playerStatManager != null)
        {
            GameObject player = playerStatManager.gameObject;
            foreach (var skill in unlockedSkills)
            {
                if (skill is PassiveSkillData passiveSkill)
                    foreach (var effect in passiveSkill.effects)
                        effect.Remove(player);
                if (skill is ActiveSkillData activeSkill)
                    activeSkill.action?.Clear(player);
                if (skill is AutoSkillData autoSkill)
                    autoSkill.action?.Clear(player);
            }
        }

        unlockedSkills.Clear();
        wildnessUnlockedCount = 0;
        fantasyUnlockedCount = 0;

        if (playerStatManager != null)
            playerStatManager.StatCore.registerStat(StatType.SKILL_POINTS, playerStatManager.startSkillPoints);
    }
}
