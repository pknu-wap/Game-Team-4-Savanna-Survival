using UnityEngine;

[CreateAssetMenu(menuName = "SkillTree/Effects/ChainLightningEffect")]
public class ChainLightningEffect : PassiveEffect
{
    [SerializeField] int chainCount;
    [SerializeField] float chainDamageBonus = 0.5f;   // 기본 damageMultiplier에 더해지는 보너스
    [SerializeField] float chainDamageDecay = 0.7f;   // 연쇄마다 곱해지는 감쇠율

    public override void Apply(GameObject player)
    {
        var state = player.GetComponent<LightningAugmentState>();
        if (state == null) state = player.AddComponent<LightningAugmentState>();
        state.isChainEnabled = true;
        state.maxChainCount = chainCount;
        state.chainDamageBonus = chainDamageBonus;
        state.chainDamageDecay = chainDamageDecay;
    }

    public override void Remove(GameObject player)
    {
        var state = player.GetComponent<LightningAugmentState>();
        if (state == null) return;
        state.isChainEnabled = false;
        state.maxChainCount = 0;
        state.chainDamageBonus = 0f;
        state.chainDamageDecay = 0.7f;
    }
}
