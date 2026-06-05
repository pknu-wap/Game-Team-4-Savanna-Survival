using UnityEngine;

/// <summary>
/// 마력 과부하: 공격력(DAMAGE)을 올리는 대신 배고픔 소비율(HUNGER_DECREASE)이 증가한다.
/// (쿨타임 증가는 SKILL_COOLDOWN이 실제 쿨타임에 반영되지 않아 미적용 — 이벤트 선행 필요)
/// </summary>
[CreateAssetMenu(menuName = "SkillTree/Effects/MagicOverloadEffect")]
public class MagicOverloadEffect : PassiveEffect
{
    [Tooltip("공격력(DAMAGE) 증가량")]
    public float damageBonus = 10f;

    [Tooltip("배고픔 소비율(HUNGER_DECREASE) 증가량")]
    public float hungerDecreaseBonus = 0.5f;

    public override void Apply(GameObject player)
    {
        var statManager = player.GetComponent<PlayerStatManager>();
        if (statManager == null) return;
        statManager.StatCore.addStat(StatType.DAMAGE, damageBonus);
        statManager.StatCore.addStat(StatType.HUNGER_DECREASE, hungerDecreaseBonus);
    }

    public override void Remove(GameObject player)
    {
        var statManager = player.GetComponent<PlayerStatManager>();
        if (statManager == null) return;
        statManager.StatCore.addStat(StatType.DAMAGE, -damageBonus);
        statManager.StatCore.addStat(StatType.HUNGER_DECREASE, -hungerDecreaseBonus);
    }
}
