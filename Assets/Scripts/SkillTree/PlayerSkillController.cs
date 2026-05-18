using UnityEngine;
using System.Collections.Generic;

public class PlayerSkillController : MonoBehaviour
{
    private List<ActiveSkillData> unlockedActiveSkills = new();
    private List<AutoSkillData> unlockedAutoSkills = new();
    
    private Dictionary<ActiveSkillData, float> activeSkillCooldowns = new();
    private Dictionary<AutoSkillData, float> autoSkillTimers = new();

    private int enemyLayerMask;

    private void Awake()
    {
        enemyLayerMask = LayerMask.GetMask("Enemy");
    }

    private void Start()
    {
        SkillManager.Instance.OnSkillUnlocked += OnSkillUnlocked;
    }

    private void Update()
    {
        HandleActiveSkills();
        HandleAutoSkills();
    }

    private void OnSkillUnlocked(BaseSkillData skill)
    {
        if (skill is ActiveSkillData activeSkill)
        {
            unlockedActiveSkills.Add(activeSkill);
            activeSkillCooldowns[activeSkill] = 0f;
        }
        else if (skill is AutoSkillData autoSkill)
        {
            unlockedAutoSkills.Add(autoSkill);
            autoSkillTimers[autoSkill] = autoSkill.interval;
        }
    }

    private void HandleActiveSkills()
    {
        foreach (var skill in unlockedActiveSkills)
        {
            // Update cooldown
            if (activeSkillCooldowns[skill] > 0f)
            {
                activeSkillCooldowns[skill] -= Time.deltaTime;
            }

            // Check input (placeholder - needs proper key mapping)
            // For now, we'll use number keys 1-9
            int skillIndex = unlockedActiveSkills.IndexOf(skill);
            KeyCode key = KeyCode.Alpha1 + skillIndex;

            if (Input.GetKeyDown(key) && activeSkillCooldowns[skill] <= 0f)
            {
                if (skill.action != null)
                {
                    skill.action.Process(gameObject, skill);
                    activeSkillCooldowns[skill] = skill.cooldown;
                }
            }
        }
    }

    private void HandleAutoSkills()
    {
        foreach (var skill in unlockedAutoSkills)
        {
            autoSkillTimers[skill] -= Time.deltaTime;

            if (autoSkillTimers[skill] <= 0f)
            {
                // Search for enemies in range
                Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(
                    transform.position,
                    skill.range,
                    enemyLayerMask
                );

                if (enemiesInRange.Length > 0 && skill.action != null)
                {
                    skill.action.Process(gameObject, skill);
                    autoSkillTimers[skill] = skill.interval;
                }
                else
                {
                    autoSkillTimers[skill] = skill.interval;
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.OnSkillUnlocked -= OnSkillUnlocked;
        }
    }
}
