using UnityEngine;

/// 플레이어 상태이상 처리 컴포넌트.
/// StatCore를 외부(PoisonEffect 등)에서 참조할 수 있도록 프로퍼티로 노출.
public class PlayerEffectTemp : Entity
{
    public PlayerStatCore StatCore { get; private set; }

    private void Awake()
    {
        StatCore = GetComponent<PlayerStatManager>().StatCore;
    }

    private void Update()
    {
        // 스턴 중에도 상태이상 틱은 계속 진행 (출혈, 중독 등은 스턴과 무관하게 지속)
        TickEffects();
    }

    public override void TakeDamage(float damage)
    {
        float current = StatCore.getStat(StatType.HEALTH).rawValue;
        StatCore.registerStat(StatType.HEALTH, Mathf.Max(0f, current - damage));
    }
}