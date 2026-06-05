using UnityEngine;

/// <summary>
/// 마법 집중: 스킬 공격력(SKILL_DAMAGE)을 패시브 보유 동안 상승시킨다.
/// </summary>
[CreateAssetMenu(menuName = "SkillTree/Effects/MagicFocusEffect")]
public class MagicFocusEffect : PassiveEffect
{
    [Tooltip("스킬 공격력(SKILL_DAMAGE) 증가량")]
    public float skillDamageBonus = 15f;

    public override void Apply(GameObject player)
    {
        var statManager = player.GetComponent<PlayerStatManager>();
        if (statManager == null) return;
        statManager.StatCore.addStat(StatType.SKILL_DAMAGE, skillDamageBonus);
    }

    public override void Remove(GameObject player)
    {
        var statManager = player.GetComponent<PlayerStatManager>();
        if (statManager == null) return;
        statManager.StatCore.addStat(StatType.SKILL_DAMAGE, -skillDamageBonus);
    }
}
