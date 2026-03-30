using UnityEngine;

[System.Serializable]
public class RunningStats
{
    [Header("Movement")]
    public float speed;

    [Header("Health")]
    public float maxHp;

    [Header("Detection")]
    public float detectionRange;
}