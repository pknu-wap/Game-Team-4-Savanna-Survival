using UnityEngine;

/// LightningAction이 보스에서 실행될 때
/// GetComponent<BossLightningController>()로 감지해
/// 타겟을 Enemy가 아닌 Player로 전환합니다.
/// BossSkillRegistry.Awake()에서 자동으로 추가되거나
/// Inspector에서 BossLion 오브젝트에 직접 붙입니다.
[DisallowMultipleComponent]
public class BossLightningController : MonoBehaviour
{
    [Tooltip("플레이어 탐색 레이어")]
    [SerializeField] private LayerMask playerLayer;

    public LayerMask PlayerLayer => playerLayer.value == 0
        ? LayerMask.GetMask("Player")
        : playerLayer;
}