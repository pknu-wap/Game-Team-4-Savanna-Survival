using UnityEngine;
using System.Collections.Generic;

public class PlayerSkillController : MonoBehaviour
{
    private List<ActiveSkillData> unlockedActiveSkills = new();
    private List<AutoSkillData> unlockedAutoSkills = new();

    private Dictionary<ActiveSkillData, float> activeSkillCooldowns = new();
    private Dictionary<AutoSkillData, float> autoSkillTimers = new();
    private Dictionary<ActiveSkillData, KeyCode> keyBindings = new();

    private int enemyLayerMask;

    private void Awake()
    {
        enemyLayerMask = LayerMask.GetMask("Enemy");
    }

    private void Start()
    {
        SkillManager.Instance.OnSkillUnlocked += OnSkillUnlocked;
        SkillManager.Instance.OnSkillRemoved += OnSkillRemoved;

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
            autoSkillTimers[autoSkill] = 0f;
        }
    }

private void OnSkillRemoved(BaseSkillData skill)
    {
        if (skill is ActiveSkillData activeSkill)
        {
            unlockedActiveSkills.Remove(activeSkill);
            activeSkillCooldowns.Remove(activeSkill);
            keyBindings.Remove(activeSkill);
        }
        else if (skill is AutoSkillData autoSkill)
        {
            unlockedAutoSkills.Remove(autoSkill);
            autoSkillTimers.Remove(autoSkill);
        }
    }


    public void RequestBinding(ActiveSkillData skill)
    {
        KeyBindingWindow.Instance?.Open(skill, this);
    }

    public void SetBinding(ActiveSkillData skill, KeyCode key)
    {
        // 충돌 확인: 같은 키를 사용하는 다른 스킬 탐색
        ActiveSkillData conflicting = null;
        foreach (var pair in keyBindings)
        {
            if (pair.Value == key && pair.Key != skill)
            {
                conflicting = pair.Key;
                break;
            }
        }

        // 충돌 시 스왑: 기존 스킬에 현재 스킬의 이전 키 배정
        if (conflicting != null)
        {
            keyBindings.TryGetValue(skill, out KeyCode oldKey);
            keyBindings[conflicting] = oldKey;
        }

        keyBindings[skill] = key;
    }

    public KeyCode GetBinding(ActiveSkillData skill)
    {
        return keyBindings.TryGetValue(skill, out KeyCode key) ? key : KeyCode.None;
    }

    private void HandleActiveSkills()
    {
        foreach (var skill in unlockedActiveSkills)
        {
            // 쿨다운 업데이트
            if (activeSkillCooldowns[skill] > 0f)
                activeSkillCooldowns[skill] -= Time.deltaTime;

            if (!keyBindings.TryGetValue(skill, out KeyCode key)) continue;
            if (key == KeyCode.None) continue;

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
                bool canProcess = true;

                if (skill.requiresTarget)
                {
                    Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(
                        transform.position, skill.range, enemyLayerMask);
                    canProcess = enemiesInRange.Length > 0;
                }

                if (canProcess && skill.action != null)
                {
                    skill.action.Process(gameObject, skill);
                    autoSkillTimers[skill] = skill.interval;
                }
                // 적이 없으면 타이머 0 유지 → 적 등장 즉시 발동
            }
        }
    }

    private void OnDestroy()
    {
        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.OnSkillUnlocked -= OnSkillUnlocked;
            SkillManager.Instance.OnSkillRemoved -= OnSkillRemoved;
        }
    }
}
