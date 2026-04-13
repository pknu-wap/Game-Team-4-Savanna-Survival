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
    SKILL_COOLDOWN
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
    public float getRawStat(StatType statType)                  //추가함, rawStats 확인 후 받아오기
    {
    if (rawStats.ContainsKey(statType))
        return rawStats[statType];

    return 0f;
    }

    public void addStat(StatType statType, float amount)        //추가함, 스탯 더하는 함수
    {
        float current = getRawStat(statType);
        registerStat(statType, current + amount);
    }
}

public class StatCalibrater
//스탯 보정 함수
{
    private static float userDamageBase = 2f;
    private static float userHealthBase = 2f;

    public static float calibrateUserDamage(float total)
    {//데미지 보정. log(total)Damage 반환
        return (float)Math.Log(userDamageBase, total);
    }

    public static float calibrateUserHealth(float total)
    {//체력 보정. log(total)Health 반환
        return (float)Math.Log(userHealthBase, total);
    }
}

