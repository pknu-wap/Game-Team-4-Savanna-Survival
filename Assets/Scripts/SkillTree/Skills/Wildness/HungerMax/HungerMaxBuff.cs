using UnityEngine;

public class HungerMaxBuff : MonoBehaviour
{
    public float damageBonus;
    public float speedBonus;
    public float buffDuration;

    [SerializeField] private ParticleSystem buffActiveVfx;

    private PlayerStatManager statManager;
    private PlayerHunger playerHunger;
    private bool buffActive = false;
    private float buffTimer = 0f;

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
            statManager.StatCore.addStat(StatType.DAMAGE, damageBonus);
            statManager.StatCore.addStat(StatType.MOVESPEED, speedBonus);
            buffActive = true;

            if (buffActiveVfx != null)
                buffActiveVfx.Play();

            Debug.Log($"[HungerMax] 버프 발동 — DAMAGE+{damageBonus}, SPEED+{speedBonus}, {buffDuration}s");
        }
        else
        {
            Debug.Log($"[HungerMax] 재트리거 — 타이머 리셋 ({buffDuration}s)");
        }

        // 재트리거 시 타이머만 리셋 (스탯 중복 적용 없음)
        buffTimer = buffDuration;
    }

    private void EndBuff()
    {
        statManager.StatCore.addStat(StatType.DAMAGE, -damageBonus);
        statManager.StatCore.addStat(StatType.MOVESPEED, -speedBonus);
        buffActive = false;
        buffTimer = 0f;

        if (buffActiveVfx != null)
            buffActiveVfx.Stop();

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
