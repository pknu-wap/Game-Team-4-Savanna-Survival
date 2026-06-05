using UnityEngine;

/// Scratch / Rend / Execute / Leap / Pounce가 보스에서 실행될 때 감지하는 컨트롤러.
/// 타겟 탐색 레이어를 Player로 전환하고 플레이어 데미지 처리를 담당합니다.
[DisallowMultipleComponent]
public class BossWildController : MonoBehaviour
{
    [Tooltip("Wild 스킬이 탐색할 타겟 레이어 — 보스는 Player 레이어 설정")]
    [SerializeField] private LayerMask targetLayer;

    public LayerMask TargetLayer => targetLayer.value == 0
        ? (LayerMask)LayerMask.GetMask("Player")
        : targetLayer;

    /// 플레이어에게 데미지를 줍니다.
    public void DamagePlayer(Collider2D hit, float damage)
    {
        var playerEffect = hit.GetComponent<PlayerEffectTemp>();
        if (playerEffect != null) { playerEffect.TakeDamage(damage); return; }

        var statManager = hit.GetComponent<PlayerStatManager>();
        if (statManager == null) return;
        float current = statManager.StatCore.getStat(StatType.HEALTH).rawValue;
        statManager.StatCore.registerStat(StatType.HEALTH, Mathf.Max(0f, current - damage));
    }

    public void DamagePlayer(GameObject hitObj, float damage)
    {
        var playerEffect = hitObj.GetComponent<PlayerEffectTemp>();
        if (playerEffect != null) { playerEffect.TakeDamage(damage); return; }

        var statManager = hitObj.GetComponent<PlayerStatManager>();
        if (statManager == null) return;
        float current = statManager.StatCore.getStat(StatType.HEALTH).rawValue;
        statManager.StatCore.registerStat(StatType.HEALTH, Mathf.Max(0f, current - damage));
    }
}