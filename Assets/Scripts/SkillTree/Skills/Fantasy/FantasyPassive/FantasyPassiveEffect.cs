using UnityEngine;

[CreateAssetMenu(menuName = "SkillTree/Effects/FantasyPassiveEffect")]
public class FantasyPassiveEffect : PassiveEffect
{
    [Tooltip("적 처치 시 회복할 배고픔 수치")]
    public float hungerRestoreAmount = 10f;

    [Tooltip("플레이어 데미지 증가율 (%)")]
    public float damageBonusPercent = 20f;

    [Tooltip("스킬 데미지(SKILL_DAMAGE) 증가량")]
    public float skillDamageBonus = 15f;

    public override void Apply(GameObject player)
    {
        var passive = player.GetComponent<FantasyPassive>();
        if (passive == null)
            passive = player.AddComponent<FantasyPassive>();

        passive.hungerRestoreAmount = hungerRestoreAmount;
        passive.damageBonusPercent  = damageBonusPercent;
        passive.skillDamageBonus    = skillDamageBonus;
    }

    public override void Remove(GameObject player)
    {
        var passive = player.GetComponent<FantasyPassive>();
        if (passive != null)
            Object.Destroy(passive);
    }
}
