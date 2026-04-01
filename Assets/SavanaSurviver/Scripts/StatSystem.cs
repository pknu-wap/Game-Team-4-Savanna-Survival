using System;
using UnityEngine;

public enum statType //임시 스탯 구현
{
    maxHp,
    attack,
    defense,
    moveSpeed,
    skillDamage,
    skillCooldown
}

public enum statChangeType //연산 방식
{
    Add,
    Multiply
}

[Serializable]
public struct statChange //리스트 입력용
{  
    public statType statType;
    public statChangeType statChangeType;
    public float value;
}
