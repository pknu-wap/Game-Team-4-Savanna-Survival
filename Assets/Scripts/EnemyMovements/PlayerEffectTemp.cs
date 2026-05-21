using UnityEngine;

/// 플레이어 루트 Entity. 상태이상 틱 처리 담당.
/// 플레이어 오브젝트에 이 컴포넌트를 추가하면 출혈 등 상태이상을 받을 수 있습니다.
public class PlayerEffectTemp : Entity
{
    private PlayerStatCore statCore;

    private void Start()
    {
        statCore = GetComponent<PlayerStatManager>().StatCore;
    }

    private void Update()
    {
        TickEffects();
    }

    public override void TakeDamage(float damage)
    {
        float current = statCore.getStat(StatType.HEALTH).rawValue;
        statCore.registerStat(StatType.HEALTH, Mathf.Max(0f, current - damage));
    }
}