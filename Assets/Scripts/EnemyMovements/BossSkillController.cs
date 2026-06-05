using UnityEngine;

/// LeapAction / PounceAction이 GetComponent<PlayerSkillController>()로
/// StartCoroutine을 호출하는 것을 받아내는 보스 전용 컴포넌트.
/// MonoBehaviour를 상속한 PlayerSkillController와 동일한 타입이어야 하므로
/// PlayerSkillController를 상속합니다.
/// 보스 패턴 / AI 로직은 BossSkillRegistry에서 담당하므로
/// 이 컴포넌트는 코루틴 실행자 역할만 합니다.
[DisallowMultipleComponent]
public class BossSkillController : PlayerSkillController
{
    // PlayerSkillController의 Start()에서 SkillManager.Instance를 구독하는 것을 막습니다.
    // 보스는 SkillManager를 사용하지 않습니다.
    private new void Start() { }
    private new void Update() { }
    private new void OnDestroy() { }
}