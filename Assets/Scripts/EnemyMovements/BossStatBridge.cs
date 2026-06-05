using UnityEngine;

/// PlayerStatManager를 상속해 보스 본체에 붙이는 브리지.
/// 기존 PlayerStatManager 대신 이 컴포넌트가 GetComponent<PlayerStatManager>()에 응답합니다.
/// EnemyStatManager와 충돌하지 않도록 스탯은 읽기 전용으로 동기화합니다.
[DisallowMultipleComponent]
public class BossStatBridge : PlayerStatManager
{
    private EnemyStatManager bossStatManager;

    public void Init(EnemyStatManager enemyStatManager)
    {
        bossStatManager = enemyStatManager;
        Sync();
    }

    /// 스킬 발동 직전 호출해 최신 스탯을 반영합니다.
    public void Sync()
    {
        if (bossStatManager == null) return;
        TrySync(StatType.DAMAGE);
        TrySync(StatType.SKILL_DAMAGE);
        TrySync(StatType.SKILL_COOLDOWN);
        TrySync(StatType.MOVESPEED);
        TrySync(StatType.HEALTH);
    }

    private void TrySync(StatType type)
    {
        try
        {
            float val = bossStatManager.getStat(type).calibratedValue;
            StatCore.registerStat(type, val);
        }
        catch { }
    }
}