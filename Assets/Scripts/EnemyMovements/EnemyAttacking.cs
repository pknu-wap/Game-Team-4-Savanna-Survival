using UnityEngine;

public class EnemyAttacking : Enemy
{
    [Header("Stats")]
    [SerializeField] private AttackingStats stats;

    private float attackTimer;
    private float contactTimer;

    protected override void Awake()
    {
        base.Awake();
        currentHp = stats.maxHp;
    }

    protected override void Move()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * stats.speed * Time.deltaTime);

        HandleAttack();
    }

    protected override bool IsPlayerInDetection()
    {
        if (player == null) return false;

        float distance = Vector2.Distance(transform.position, player.position);
        return distance <= stats.detectionRange;
    }

    private void HandleAttack()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= stats.attackRange)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= stats.attackInterval)
            {
                player.GetComponent<PlayerDummy>()?.TakeDamage(stats.attackDamage);
                attackTimer = 0f;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        contactTimer += Time.deltaTime;

        if (contactTimer >= stats.contactInterval)
        {
            collision.gameObject
                .GetComponent<PlayerDummy>()
                ?.TakeDamage(stats.contactDamage);

            contactTimer = 0f;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            contactTimer = 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (stats == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stats.detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stats.attackRange);
    }
}