using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 스탯을 패시브 보유 동안 가감하는 범용 효과.
/// modifiers에 (스탯, 증감값)을 넣으면 Apply 시 더하고 Remove 시 되돌린다.
/// 이벤트가 필요 없는(상시 적용) 스탯 패시브 전용.
/// 예) 마법 집중(SKILL_DAMAGE+), 마력 과부하(HUNGER_DECREASE+, DAMAGE+).
/// </summary>
[CreateAssetMenu(menuName = "SkillTree/Effects/StatModifierEffect")]
public class StatModifierEffect : PassiveEffect
{
    [System.Serializable]
    public struct StatModifier
    {
        public StatType statType;
        [Tooltip("이 스탯에 더할 값 (음수면 감소)")]
        public float value;
    }

    public List<StatModifier> modifiers = new();

    public override void Apply(GameObject player)
    {
        var statManager = player.GetComponent<PlayerStatManager>();
        if (statManager == null) return;

        foreach (var mod in modifiers)
            statManager.StatCore.addStat(mod.statType, mod.value);
    }

    public override void Remove(GameObject player)
    {
        var statManager = player.GetComponent<PlayerStatManager>();
        if (statManager == null) return;

        foreach (var mod in modifiers)
            statManager.StatCore.addStat(mod.statType, -mod.value);
    }
}
