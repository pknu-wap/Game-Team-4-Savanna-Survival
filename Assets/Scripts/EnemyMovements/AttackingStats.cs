using UnityEngine;

[System.Serializable]
public class AttackingStats
{
    [Header("Movement")]
    public float speed;

    [Header("Health")]
    public float maxHp;

    [Header("Detection")]
    public float detectionRange;

    [Header("Contact Damage")]
    public float contactDamage;
    public float contactInterval;


    [Header("Attack")]
    public float attackDamage;
    public float attackRange;
    public float attackInterval; 
}