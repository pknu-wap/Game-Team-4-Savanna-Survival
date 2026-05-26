using UnityEngine;

public class HungerMaxBuff : MonoBehaviour
{
    public float damageBonus;
    public float speedBonus;
    public float buffDuration;

    [SerializeField] private ParticleSystem buffActiveVfx;

    private PlayerStatManager statManager;
    private bool buffActive = false;
    private float buffTimer = 0f;
    private float prevHungerRatio = 0f;

    private void Start()
    {
        statManager = GetComponent<PlayerStatManager>();
    }

    private void Update()
    {
        float hunger = statManager.StatCore.getStat(StatType.HUNGER).rawValue;
        float maxHunger = statManager.StatCore.getStat(StatType.MAX_HUNGER).rawValue;
        float currentRatio = hunger / maxHunger;

        // PlayerHunger가 허기 100% 도달 시 60%로 리셋하는 타이밍 감지
        if (prevHungerRatio >= 0.95f && currentRatio <= 0.65f)
            TriggerBuff();

        prevHungerRatio = currentRatio;

        if (buffActive)
        {
            buffTimer -= Time.deltaTime;
            if (buffTimer <= 0f)
                EndBuff();
        }
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
    }

    private void OnDestroy()
    {
        if (buffActive)
            EndBuff();
    }
}
