using System;
using System.Collections.Generic;


public class AttributeData
{
    public StatType statType { get; }
    public float    points   { get; } // 투자된 포인트
    public float    value    { get; } // 현재 어트리뷰트 값

    public AttributeData(StatType statType, float points, float value)
    {
        this.statType = statType;
        this.points   = points;
        this.value    = value;
    }
}


public class PlayerAttributeData
{
    public float points { get; }

    public Dictionary<StatType, AttributeData> attributes { get; }

    public PlayerAttributeData(float points, Dictionary<StatType, AttributeData> attributes)
    {
        this.points     = points;
        this.attributes = attributes;
    }

    public AttributeData getAttribute(StatType statType)
    {
        return attributes.TryGetValue(statType, out var data)
            ? data
            : new AttributeData(statType, 0f, 0f);
    }
}


public abstract class AttributeManager
{
    private float                               points     = 0f;
    private Dictionary<StatType, AttributeData> attributes = new();
    private StatManager                         statManager;

    public event Action<AttributeRegisterEvent> onAttributeRegister;   // 등록 전, Cancellable
    public event Action<StatType, float>        onAttributeRegistered; // 등록 후
    public event Action<float>                  onPointsChanged;       // 포인트 소모 후

    protected AttributeManager(StatManager statManager)
    {
        this.statManager = statManager;
    }

    public float getPoints()
    {
        return points;
    }

    public AttributeData getAttribute(StatType statType)
    {
        return attributes.TryGetValue(statType, out var data)
            ? data
            : new AttributeData(statType, 0f, 0f);
    }

    public PlayerAttributeData getData()
    {
        return new PlayerAttributeData(points, new Dictionary<StatType, AttributeData>(attributes));
    }

    public void grantPoints(float amount)
    {
        if (amount <= 0f) return;
        points += amount;
        onPointsChanged?.Invoke(points);
    }

    public bool consumePoints(float amount)
    {
        if (amount <= 0f) return false;
        if (points < amount) return false;
        points -= amount;
        onPointsChanged?.Invoke(points);
        return true;
    }

    // 어트리뷰트에 절대값으로 등록 (points 기준, value는 AttributeCalibrater로 환산)
    public void registerAttribute(StatType statType, float pointsInvested)
    {
        var events = new AttributeRegisterEvent(statType, pointsInvested);
        onAttributeRegister?.Invoke(events);
        if (events.isCancelled) return;

        float value = AttributeCalibrater.calibrate(statType, events.value);
        attributes[statType] = new AttributeData(statType, events.value, value);
        onAttributeRegistered?.Invoke(statType, events.value);
    }

    public bool investPoint(StatType statType, float value)
    {
        if (value <= 0f) return false;
        if (!consumePoints(value)) return false;

        float currentPoints = getAttribute(statType).points;
        registerAttribute(statType, currentPoints + value);
        statManager.addStat(statType, AttributeCalibrater.calibrate(statType, value));
        return true;
    }

    public bool retrievePoint(StatType statType, float value)
    {
        if (value <= 0f) return false;

        float currentPoints = getAttribute(statType).points;
        if (currentPoints < value) return false;

        registerAttribute(statType, currentPoints - value);
        grantPoints(value);
        statManager.addStat(statType, -AttributeCalibrater.calibrate(statType, value));
        return true;
    }
}


public class AttributeCalibrater
// 어트리뷰트 포인트 → 스탯 증감량 환산
{
    // 1포인트당 증/감 배율. 음수면 투자 시 스탯이 감소 (예: 쿨다운 단축)
    public static float calibrate(StatType statType, float points)
    {
        return statType switch
        {
            StatType.DAMAGE         => points * 2f,
            StatType.DEFENSE        => points * 1f,
            StatType.MAX_HEALTH     => points * 10f,
            StatType.MAX_HUNGER     => points * 5f,
            StatType.MOVESPEED      => points * 0.5f,
            StatType.SKILL_DAMAGE   => points * 3f,
            StatType.SKILL_COOLDOWN => points * -0.05f,
            StatType.HEALTH_REGEN   => points * 0.1f,
            _                       => points
        };
    }
}
