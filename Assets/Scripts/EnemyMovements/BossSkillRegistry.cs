using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// 보스 스킬 등록 및 자동 실행 관리.
public class BossSkillRegistry : MonoBehaviour
{
    [Header("스킬 슬롯 (Inspector에서 설정)")]
    [SerializeField] private List<BossSkillSlot> slots = new();

    [Header("Auto 스킬 플레이어 감지 레이어")]
    [SerializeField] private LayerMask playerLayer;

    [Header("투사체 충돌 무시")]
    [Tooltip("보스 스킬 실행 중 보스를 통과할 투사체 레이어 이름 — 비워두면 무시 안 함")]
    [SerializeField] private string projectileLayerName = "PlayerProjectile";

    private BossStatBridge     statBridge;
    private BossMovementBridge movementBridge;
    private EnemyStatManager   bossStatManager;
    private int                executingSlot = -1;
    private int                lastActiveSlotIndex = -1;

    // ── 초기화 ───────────────────────────────────────────────

    private void Awake()
    {
        statBridge     = GetComponent<BossStatBridge>()     ?? gameObject.AddComponent<BossStatBridge>();
        movementBridge = GetComponent<BossMovementBridge>() ?? gameObject.AddComponent<BossMovementBridge>();
        if (GetComponent<BossSkillController>() == null)
            gameObject.AddComponent<BossSkillController>();
    }

    public void Init(EnemyStatManager enemyStatManager)
    {
        bossStatManager = enemyStatManager;
        statBridge.Init(bossStatManager);
    }

    private void Start()
    {
        if (bossStatManager == null)
        {
            bossStatManager = GetComponent<EnemyStatManager>();
            if (bossStatManager != null) statBridge.Init(bossStatManager);
        }
        ApplyPassiveSkills();
    }

    // ── 패시브 적용 ──────────────────────────────────────────

    private void ApplyPassiveSkills()
    {
        foreach (var slot in slots)
        {
            if (slot.slotType != BossSkillSlot.SlotType.Passive || slot.passiveSkill == null) continue;
            foreach (var effect in slot.passiveSkill.effects)
                effect.Apply(gameObject);
            Debug.Log($"[BossSkillRegistry] 패시브 적용: {slot.passiveSkill.skillName}");
        }
    }

    // ── 업데이트 ─────────────────────────────────────────────

    private void Update()
    {
        foreach (var slot in slots) slot.Tick(Time.deltaTime);

        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            if (slot.slotType == BossSkillSlot.SlotType.Auto && slot.IsReady)
                TryExecuteAutoSlot(i, slot);
        }
    }

    // ── Auto 슬롯 ────────────────────────────────────────────

    private void TryExecuteAutoSlot(int index, BossSkillSlot slot)
    {
        if (slot.autoSkill?.action == null) return;
        if (slot.maxRange > 0f && Physics2D.OverlapCircle(transform.position, slot.maxRange, playerLayer) == null)
            return;

        slot.ResetCooldown();
        statBridge.Sync();
        slot.autoSkill.action.Process(gameObject, slot.autoSkill);
        Debug.Log($"[BossSkillRegistry] Auto {index} ({slot.autoSkill.skillName}) 발동");
    }

    // ── Active 슬롯 — 직접 실행 ──────────────────────────────

    public bool TryExecute(int slotIndex)
    {
        if (executingSlot >= 0) return false;
        if (slotIndex < 0 || slotIndex >= slots.Count) return false;

        var slot = slots[slotIndex];
        if (slot.slotType != BossSkillSlot.SlotType.Active) return false;
        if (!slot.IsReady || !slot.HasAction) return false;

        StartCoroutine(ExecuteActiveRoutine(slotIndex, slot));
        return true;
    }

    public bool TryExecuteAny()
    {
        for (int i = 0; i < slots.Count; i++)
            if (TryExecute(i)) return true;
        return false;
    }

    // ── Active 슬롯 순환 ─────────────────────────────────────

    public bool TryExecuteNextActive()
    {
        if (executingSlot >= 0) return false;
        if (slots.Count == 0) return false;

        for (int offset = 1; offset <= slots.Count; offset++)
        {
            int i    = (lastActiveSlotIndex + offset) % slots.Count;
            var slot = slots[i];
            if (slot.slotType != BossSkillSlot.SlotType.Active) continue;
            if (!slot.IsReady || !slot.HasAction) continue;

            lastActiveSlotIndex = i;
            StartCoroutine(ExecuteActiveRoutine(i, slot));
            return true;
        }
        return false;
    }

    public bool HasAnyActiveReady()
    {
        foreach (var slot in slots)
            if (slot.slotType == BossSkillSlot.SlotType.Active && slot.IsReady && slot.HasAction)
                return true;
        return false;
    }

    public int ActiveSlotCount()
    {
        int count = 0;
        foreach (var slot in slots)
            if (slot.slotType == BossSkillSlot.SlotType.Active) count++;
        return count;
    }

    public bool IsReady(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return false;
        return slots[slotIndex].IsReady;
    }

    public bool IsBusy => executingSlot >= 0;

    // ── Active 실행 루틴 ─────────────────────────────────────

    private IEnumerator ExecuteActiveRoutine(int index, BossSkillSlot slot)
{
    executingSlot = index;
    slot.ResetCooldown();

    if (slot.telegraph > 0f)
        yield return new WaitForSeconds(slot.telegraph);

    statBridge.Sync();

    if (slot.activeSkill?.action != null)
        slot.activeSkill.action.Process(gameObject, slot.activeSkill);

    Debug.Log($"[BossSkillRegistry] Active {index} ({slot.activeSkill?.skillName}) 발동");
    executingSlot = -1;
}

    // ── 유틸 ─────────────────────────────────────────────────

    public float GetBossStat(StatType type)
    {
        if (bossStatManager == null) return 0f;
        try { return bossStatManager.getStat(type).calibratedValue; }
        catch { return 0f; }
    }

    public void UpdateMoveDirection(Vector2 dir) => movementBridge?.UpdateDirection(dir);

    // ── 투사체 충돌 제어 ─────────────────────────────────────

    /// 보스 스킬 실행 중 투사체가 보스를 통과하도록 레이어 충돌을 일시 해제합니다.
    private void SetProjectilePassthrough(bool ignore)
    {
        if (string.IsNullOrEmpty(projectileLayerName)) return;
        int projLayer = LayerMask.NameToLayer(projectileLayerName);
        if (projLayer < 0) return;
        Physics2D.IgnoreLayerCollision(gameObject.layer, projLayer, ignore);
    }
}