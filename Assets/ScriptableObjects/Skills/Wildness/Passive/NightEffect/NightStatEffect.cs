using UnityEngine;

[CreateAssetMenu(menuName = "SkillTree/Effects/NightStatEffect")]
public class NightStatEffect : PassiveEffect
{
    [SerializeField] private float damageBonus;
    [SerializeField] private float moveSpeedBonus;

    public override void Apply(GameObject player)
    {
        var statManager = player.GetComponent<PlayerStatManager>();
        if (statManager == null) return;

        statManager.StatCore.addStat(StatType.DAMAGE, damageBonus);
        statManager.StatCore.addStat(StatType.MOVESPEED, moveSpeedBonus);
    }

    public override void Remove(GameObject player)
    {
        var statManager = player.GetComponent<PlayerStatManager>();
        if (statManager == null) return;

        statManager.StatCore.addStat(StatType.DAMAGE, -damageBonus);
        statManager.StatCore.addStat(StatType.MOVESPEED, -moveSpeedBonus);
    }
}