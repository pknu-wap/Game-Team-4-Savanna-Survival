public class PlayerTempStatManager : StatManager
{
    public void Init(float hp, float damage)
    {
        registerStat(StatType.HEALTH, hp);
        registerStat(StatType.DAMAGE, damage);
    }
}