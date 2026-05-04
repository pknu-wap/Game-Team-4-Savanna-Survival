public class EnemyStatManager : StatManager
{
    public void InitAttacker(float hp, float damage)
    {
        registerStat(StatType.HEALTH, hp);
        registerStat(StatType.DAMAGE, damage);
    }

    public void InitRunner(float hp)
    {
        registerStat(StatType.HEALTH, hp);
    }
}