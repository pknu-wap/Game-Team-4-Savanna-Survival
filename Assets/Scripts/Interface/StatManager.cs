using System;
using System.Collections.Generic;

public enum StatType
{
    DAMAGE,
    DEFENSE,
    HEALTH,
    MAX_HEALTH,
    LEVEL,
    HUNGER,
    EXP,
    MAX_EXP,
    MOVESPEED,
    SKILL_DAMAGE,
    SKILL_COOLDOWN,
    HEALTH_REGEN,
    MAX_HUNGER,
    SKILL_POINTS,
    HUNGER_DECREASE
}

public class StatData
{
    public StatType statType  { get; }
    public float    rawValue  { get; }
    public float    calibratedValue { get; }

    public StatData(StatType statType, float rawValue, float calibratedValue)
    {
        this.statType        = statType;
        this.rawValue        = rawValue;
        this.calibratedValue = calibratedValue;
    }
}


public abstract class StatManager
{
    Dictionary<StatType, float>    rawStats        = new();
    Dictionary<StatType, StatData> calibratedStats = new();

    public event Action<StatRegisterEvent> onStatRegister;   // 등록 전, Cancellable
    public event Action<StatType, float>   onStatRegistered; // 등록 후

public StatData getStat(StatType statType)
{
    if (calibratedStats.TryGetValue(statType, out var stat))
        return stat;

    return new StatData(statType, 0f, 0f);
}
    public void registerStat(StatType statType, float value)
    {
        var events = new StatRegisterEvent(statType, value);
        onStatRegister?.Invoke(events);
        if (events.isCancelled) return;

        rawStats[statType] = value;

        float calibrated = statType switch
        {
            StatType.DAMAGE => StatCalibrater.calibrateUserDamage(value),
            StatType.HEALTH => StatCalibrater.calibrateUserHealth(value),
            _               => value
        };

        calibratedStats[statType] = new StatData(statType, value, calibrated);
        onStatRegistered?.Invoke(statType, value);
    }
    
    public void addStat(StatType statType, float value)
    {
        float currentValue = 0f;

        if (calibratedStats.ContainsKey(statType))
        {
            currentValue = calibratedStats[statType].rawValue;
        }

        registerStat(statType, currentValue + value);
    }
}

public class StatCalibrater
//스탯 보정 함수
{
    private static float userDamageBase = 2f;
    private static float userHealthBase = 2f;

    public static float calibrateUserDamage(float total)
    {
        return (float)Math.Log(total, userDamageBase);
    }

    public static float calibrateUserHealth(float total)
    {
        return (float)Math.Log(total, userHealthBase);
    }
}