using System;
using System.Collections.Generic;

public enum StatType
{
    DAMAGE,
    HEALTH,
    LEVEL,
    HUNGER,
    EXP
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
        return calibratedStats[statType];
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
}

public class StatCalibrater
{
    private static float userDamageBase = 2f;
    private static float userHealthBase = 2f;

    public static float calibrateUserDamage(float total)
    {
        return (float)Math.Log(userDamageBase, total);
    }

    public static float calibrateUserHealth(float total)
    {
        return (float)Math.Log(userHealthBase, total);
    }
}