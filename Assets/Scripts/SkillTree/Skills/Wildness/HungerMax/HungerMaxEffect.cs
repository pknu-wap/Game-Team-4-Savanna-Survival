using UnityEngine;

[CreateAssetMenu(menuName = "SkillTree/Effects/HungerMaxEffect")]
public class HungerMaxEffect : PassiveEffect
{
    public float damageBonusPercent;
    public float speedBonusPercent;
    public float buffDuration;
    [Tooltip("배고픔 감소량을 줄이는 수치 (5레벨 마스터 특성, 0이면 미적용)")]
    public float hungerDecreaseReduction;
    public ParticleSystem buffVfxPrefab;

    public override void Apply(GameObject player)
    {
        var buff = player.GetComponent<HungerMaxBuff>();
        if (buff == null)
            buff = player.AddComponent<HungerMaxBuff>();

        buff.damageBonusPercent = damageBonusPercent;
        buff.speedBonusPercent = speedBonusPercent;
        buff.buffDuration = buffDuration;
        buff.buffVfxPrefab = buffVfxPrefab;

        if (hungerDecreaseReduction > 0f)
        {
            var statManager = player.GetComponent<PlayerStatManager>();
            statManager?.StatCore.addStat(StatType.HUNGER_DECREASE, -hungerDecreaseReduction);
        }
    }

    public override void Remove(GameObject player)
    {
        if (hungerDecreaseReduction > 0f)
        {
            var statManager = player.GetComponent<PlayerStatManager>();
            statManager?.StatCore.addStat(StatType.HUNGER_DECREASE, hungerDecreaseReduction);
        }

        var buff = player.GetComponent<HungerMaxBuff>();
        if (buff != null)
            Object.Destroy(buff);
    }
}
