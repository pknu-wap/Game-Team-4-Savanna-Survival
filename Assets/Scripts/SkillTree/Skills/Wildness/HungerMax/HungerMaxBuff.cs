using UnityEngine;

public class HungerMaxBuff : MonoBehaviour
{
    public float damageBonusPercent;   // 예: 5 → 5%
    public float speedBonusPercent;    // 예: 1 → 1%
    public float buffDuration;

    public ParticleSystem buffVfxPrefab;

    private PlayerStatManager statManager;
    private PlayerHunger playerHunger;
    private bool buffActive = false;
    private float buffTimer = 0f;
    private float appliedDamageBonus;
    private float appliedSpeedBonus;

    private void Start()
    {
        statManager = GetComponent<PlayerStatManager>();
        playerHunger = GetComponent<PlayerHunger>();
        if (playerHunger != null)
            playerHunger.OnHungerFilled += TriggerBuff;
    }

    private void Update()
    {
        if (!buffActive) return;

        buffTimer -= Time.deltaTime;
        if (buffTimer <= 0f)
            EndBuff();
    }

    private void TriggerBuff()
    {
        if (!buffActive)
        {
            float baseDamage = statManager.StatCore.getStat(StatType.DAMAGE).calibratedValue;
            float baseSpeed  = statManager.StatCore.getStat(StatType.MOVESPEED).calibratedValue;
            appliedDamageBonus = baseDamage * (damageBonusPercent / 100f);
            appliedSpeedBonus  = baseSpeed  * (speedBonusPercent  / 100f);

            statManager.StatCore.addStat(StatType.DAMAGE,    appliedDamageBonus);
            statManager.StatCore.addStat(StatType.MOVESPEED, appliedSpeedBonus);
            buffActive = true;

            if (buffVfxPrefab != null)
            {
                var vfx = Instantiate(buffVfxPrefab, transform.position, Quaternion.identity, transform);
                Object.Destroy(vfx.gameObject, vfx.main.duration + vfx.main.startLifetime.constantMax);
            }

            Debug.Log($"[HungerMax] 버프 발동 — DAMAGE+{damageBonusPercent}%({appliedDamageBonus:F1}), SPEED+{speedBonusPercent}%({appliedSpeedBonus:F2}), {buffDuration}s");
        }
        else
        {
            Debug.Log($"[HungerMax] 재트리거 — 타이머 리셋 ({buffDuration}s)");
        }

        buffTimer = buffDuration;
    }

    private void EndBuff()
    {
        statManager.StatCore.addStat(StatType.DAMAGE,    -appliedDamageBonus);
        statManager.StatCore.addStat(StatType.MOVESPEED, -appliedSpeedBonus);
        buffActive = false;
        buffTimer = 0f;
        appliedDamageBonus = 0f;
        appliedSpeedBonus  = 0f;

        Debug.Log("[HungerMax] 버프 종료 — 스탯 원복");
    }

    private void OnDestroy()
    {
        if (playerHunger != null)
            playerHunger.OnHungerFilled -= TriggerBuff;

        if (buffActive)
            EndBuff();
    }
}
