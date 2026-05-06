using UnityEngine;

public class SkillsBasic_attack : MonoBehaviour
{
    private const float value = 10f;
    private PlayerStatCore statCore;

    private void Start()
    {
        PlayerStatManager playerStatManager = GetComponentInParent<PlayerStatManager>();
        statCore = playerStatManager.StatCore;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null) return; 

        float damage = statCore.getStat(StatType.DAMAGE).calibratedValue * value / 3.5f;
        enemy.TakeDamage(damage);
    }
}