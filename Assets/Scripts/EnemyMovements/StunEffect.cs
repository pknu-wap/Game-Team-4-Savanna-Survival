using System.Collections.Generic;
using UnityEngine;

/// 스턴(기절) 상태이상.
/// OnApply 시 플레이어의 행동 관련 컴포넌트를 비활성화,
/// OnRemove 시 복원합니다. 코드 수정 없이 enabled 토글만 사용합니다.
///
/// 차단 대상 컴포넌트 타입을 blockedTypes에 추가하면 확장 가능합니다.
public class StunEffect : StatusEffectBase
{
    private float duration;

    // 스턴 중 비활성화할 컴포넌트 타입 목록
    // 새 행동 스크립트가 추가되면 여기에 추가하면 됩니다
    private static readonly System.Type[] blockedTypes = new System.Type[]
    {
        typeof(PlayerMovement),
        typeof(PlayerInputRelay),
        typeof(PlayerSkill),
    };

    // 실제로 비활성화된 컴포넌트만 기록 (원래 꺼져있던 건 복원하지 않음)
    private readonly List<MonoBehaviour> disabledComponents = new();

    public StunEffect(float duration)
    {
        EffectName    = "Stun";
        this.duration = duration;
    }

    public override void OnApply(Entity target)
    {
        disabledComponents.Clear();
        foreach (System.Type type in blockedTypes)
        {
            MonoBehaviour comp = target.GetComponent(type) as MonoBehaviour;
            if (comp == null || !comp.enabled) continue;
            comp.enabled = false;
            disabledComponents.Add(comp);
        }
        Debug.Log($"[StunEffect] 적용 — {duration:F1}s / 차단 컴포넌트 {disabledComponents.Count}개");
    }

    public override void OnRemove(Entity target)
    {
        foreach (MonoBehaviour comp in disabledComponents)
            if (comp != null) comp.enabled = true;
        disabledComponents.Clear();
        Debug.Log("[StunEffect] 해제");
    }

    public override void OnTick(Entity target, float deltaTime)
    {
        duration -= deltaTime;
        if (duration <= 0f) IsExpired = true;
    }
}