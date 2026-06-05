using UnityEngine;

/// LaserAction이 보스에서 실행될 때 감지하는 마커 컴포넌트.
/// BossSkillRegistry.Awake()에서 자동 추가되거나
/// Inspector에서 BossLion 오브젝트에 직접 붙입니다.
[DisallowMultipleComponent]
public class BossLaserController : MonoBehaviour
{
    [Tooltip("레이저가 타격할 레이어 — 보스는 Player 레이어 설정")]
    [SerializeField] private LayerMask targetLayer;

    public LayerMask TargetLayer => targetLayer.value == 0
        ? (LayerMask)LayerMask.GetMask("Player")
        : targetLayer;
}