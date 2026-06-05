using UnityEngine;

[System.Serializable]
public class BossSkillSlot
{
    public enum SlotType { Active, Auto, Passive }

    [Tooltip("슬롯 종류")]
    public SlotType slotType = SlotType.Active;

    [Tooltip("Active 스킬 SO")]
    public ActiveSkillData activeSkill;

    [Tooltip("Auto 스킬 SO")]
    public AutoSkillData autoSkill;

    [Tooltip("Passive 스킬 SO")]
    public PassiveSkillData passiveSkill;

    [Tooltip("스킬 쿨타임(초) — Auto는 interval을 SO에서 읽음")]
    public float cooldown = 5f;

    [Tooltip("발동까지의 전조 시간(초)")]
    public float telegraph = 0.5f;

    [Tooltip("발동에 필요한 플레이어와의 최대 거리 — 0이면 제한 없음")]
    public float maxRange = 0f;

    [Tooltip("전조 인디케이터 프리팹 — null이면 생략")]
    public GameObject indicatorPrefab;

    [Tooltip("인디케이터 반지름 오버라이드 — 0이면 스킬 액션 SO에서 자동 탐색")]
    public float indicatorRadiusOverride = 0f;

    [HideInInspector] public float currentCooldown;
    public bool IsReady => currentCooldown <= 0f;

    // 슬롯에 할당된 Action SO (null 체크용 편의 프로퍼티)
    public bool HasAction => slotType switch
    {
        SlotType.Active  => activeSkill?.action  != null,
        SlotType.Auto    => autoSkill?.action    != null,
        SlotType.Passive => passiveSkill         != null,
        _                => false
    };

    public void Tick(float deltaTime)
    {
        if (currentCooldown > 0f) currentCooldown -= deltaTime;
    }

    public void ResetCooldown()
    {
        // Auto 슬롯은 SO의 interval을 쿨타임으로 사용
        currentCooldown = slotType == SlotType.Auto && autoSkill != null
            ? autoSkill.interval
            : cooldown;
    }
}