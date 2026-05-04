using UnityEngine;

public class StatRegisterEvent
{
    public StatType statType { get; }

    public float value { get; set; }

    public bool isCancelled { get; set; } = false;

    public StatRegisterEvent(StatType statType, float value)
    {
        this.statType = statType;
        this.value = value;
    }
    
    
}