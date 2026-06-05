using UnityEngine;

[CreateAssetMenu(menuName = "SkillTree/Effects/BasicAttackUpgradeEffect")]
public class BasicAttackUpgradeEffect : PassiveEffect
{
    public float damageBonus;
    public float rangeBonus;

    public override void Apply(GameObject player)
    {
        var attack = player.GetComponentInChildren<SkillsBasic_attack>();
        if (attack == null) return;

        attack.AddAttackDamageBonus(damageBonus);
        attack.AddAttackRangeBonus(rangeBonus);
    }

    public override void Remove(GameObject player)
    {
        var attack = player.GetComponentInChildren<SkillsBasic_attack>();
        if (attack == null) return;

        attack.AddAttackDamageBonus(-damageBonus);
        attack.AddAttackRangeBonus(-rangeBonus);
    }
}