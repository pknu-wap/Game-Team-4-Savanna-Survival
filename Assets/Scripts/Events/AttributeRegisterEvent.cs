using UnityEngine;

public class AttributeRegisterEvent
{
    public StatType statType { get; }

    public float value { get; set; }

    public bool isCancelled { get; set; } = false;

    public AttributeRegisterEvent(StatType statType, float value)
    {
        this.statType = statType;
        this.value = value;
    }
    
    
}