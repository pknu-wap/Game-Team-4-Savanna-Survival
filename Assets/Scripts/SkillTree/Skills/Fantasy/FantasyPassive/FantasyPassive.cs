using UnityEngine;


public class FantasyPassive : MonoBehaviour
{
    [Header("적 처치 시 배고픔 회복 (OnDeath)")]
    public float hungerRestoreAmount = 10f;

    [Header("공격 시 데미지 증가 (OnDamage)")]
    [Tooltip("플레이어가 가하는 데미지 증가율 (%). 예: 20 → +20%")]
    public float damageBonusPercent = 20f;

    [Header("스킬 데미지 상승 (SKILL_DAMAGE 스탯)")]
    [Tooltip("패시브 보유 동안 SKILL_DAMAGE 스탯에 더할 값")]
    public float skillDamageBonus = 15f;

    private PlayerStatCore statCore;
    private Entity ownerEntity;
    private bool skillDamageApplied = false;

    private void Start()
    {
        statCore = GetComponent<PlayerStatManager>().StatCore;
        ownerEntity = GetComponent<Entity>();

        ApplySkillDamageBonus();
    }

    private void ApplySkillDamageBonus()
    {
        if (statCore == null || skillDamageApplied) return;
        statCore.addStat(StatType.SKILL_DAMAGE, skillDamageBonus);
        skillDamageApplied = true;
        Debug.Log("Stat Registered: SKILL_DAMAGE -> "+skillDamageBonus);
    }

    private void RemoveSkillDamageBonus()
    {
        if (statCore == null || !skillDamageApplied) return;
        statCore.addStat(StatType.SKILL_DAMAGE, -skillDamageBonus);
        skillDamageApplied = false;
        Debug.Log("Stat removed: SKILL_DAMAGE -> "+skillDamageBonus);
    }

    private void OnDestroy()
    {
        RemoveSkillDamageBonus();
    }

    private void OnEnable()
    {
        EnemyEvents.OnDeath  += OnEnemyDeath;
        EnemyEvents.OnDamage += OnEnemyDamage;
    }

    private void OnDisable()
    {
        EnemyEvents.OnDeath  -= OnEnemyDeath;
        EnemyEvents.OnDamage -= OnEnemyDamage;
    }

    public void OnEnemyDeath(EnemyDeathEvent e)
    {
        if (statCore == null) return;

        float currentHunger = statCore.getStat(StatType.HUNGER).rawValue;
        float maxHunger     = statCore.getStat(StatType.MAX_HUNGER).rawValue;
        float newHunger     = Mathf.Min(currentHunger + hungerRestoreAmount, maxHunger);

        statCore.registerStat(StatType.HUNGER, newHunger);
        Debug.Log(newHunger);
    }

    public void OnEnemyDamage(EnemyDamageEvent e)
    {
        if (ownerEntity != null && e.getAttacker() != ownerEntity) return;

        float before = e.getDamage();
        float after  = before * (1f + damageBonusPercent / 100f);
        e.setDamage(after);

        Debug.Log(after);
    }
}
