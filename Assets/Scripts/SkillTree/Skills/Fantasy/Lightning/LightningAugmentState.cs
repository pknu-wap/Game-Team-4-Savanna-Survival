using UnityEngine;

public class LightningAugmentState : MonoBehaviour
{
    public bool isChainEnabled = false;
    public int maxChainCount = 0;
    public float chainDamageBonus = 0f;   // 기본 damageMultiplier에 더해지는 보너스
    public float chainDamageDecay = 0.7f; // 연쇄마다 곱해지는 감쇠율
}
