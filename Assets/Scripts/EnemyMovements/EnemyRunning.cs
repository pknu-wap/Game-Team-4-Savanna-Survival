using UnityEngine;

public class EnemyRunning : Enemy
{
    [Header("Stats")]
    [SerializeField] private RunningStats stats;

    protected override void Awake()
    {
        base.Awake();
        currentHp = stats.maxHp;
    }

    protected override void Move()
    {
        if (player == null) return;

        Vector2 direction = (transform.position - player.position).normalized;

        rb.linearVelocity = direction * stats.speed;
    }

    protected override bool IsPlayerInDetection()
    {
        if (player == null) return false;

        float distance = Vector2.Distance(transform.position, player.position);
        return distance <= stats.detectionRange;
    }

    private void OnDrawGizmosSelected()
    {
        if (stats == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stats.detectionRange);
    }
}