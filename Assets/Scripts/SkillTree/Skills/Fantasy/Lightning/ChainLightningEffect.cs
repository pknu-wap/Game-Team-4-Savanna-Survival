using UnityEngine;

[CreateAssetMenu(menuName = "SkillTree/Effects/ChainLightningEffect")]
public class ChainLightningEffect : PassiveEffect
{
    [SerializeField] int chainCount;

    public override void Apply(GameObject player)
    {
        var state = player.GetComponent<LightningAugmentState>();
        if (state == null) state = player.AddComponent<LightningAugmentState>();
        state.isChainEnabled = true;
        state.maxChainCount = chainCount;
    }

    public override void Remove(GameObject player)
    {
        var state = player.GetComponent<LightningAugmentState>();
        if (state == null) return;
        state.isChainEnabled = false;
        state.maxChainCount = 0;
    }
}
