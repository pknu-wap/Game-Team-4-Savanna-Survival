public class EnemyStatManager : StatManager
{
    public void InitAttacker(float hp, float damage)
    {
        registerStat(StatType.HEALTH, hp);
        registerStat(StatType.DAMAGE, damage);

        // 보스 스킬용 기본 스탯
        registerStat(StatType.SKILL_DAMAGE, 20f);
        registerStat(StatType.SKILL_COOLDOWN, 1f);
        registerStat(StatType.MOVESPEED, 0f);
    }

    public void InitRunner(float hp)
    {
        registerStat(StatType.HEALTH, hp);

        // 참조 시 오류 방지를 위한 기본값
        registerStat(StatType.DAMAGE, 0f);
        registerStat(StatType.SKILL_DAMAGE, 20f);
        registerStat(StatType.SKILL_COOLDOWN, 1f);
        registerStat(StatType.MOVESPEED, 0f);
    }
}