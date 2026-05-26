using UnityEngine;

public class BabyLionAI : MonoBehaviour
{
    public float damage = 5f;
    public float attackRange = 0.8f;
    public float chaseRange = 8f;
    public float moveSpeed = 3f;
    public float attackCooldown = 1f;

    private Transform target;
    private float attackTimer;
    private int enemyLayerMask;

    private void Start()
    {
        enemyLayerMask = LayerMask.GetMask("Enemy");
        attackTimer = attackCooldown;
    }

    private void Update()
    {
        FindNearestEnemy();

        if (target == null) return;

        float dist = Vector2.Distance(transform.position, target.position);
        if (dist > attackRange)
        {
            Vector2 dir = ((Vector2)target.position - (Vector2)transform.position).normalized;
            transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
        }

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f && dist <= attackRange)
        {
            var enemy = target.GetComponent<Enemy>();
            enemy?.TakeDamage(damage);
            attackTimer = attackCooldown;
        }
    }

    private void FindNearestEnemy()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, chaseRange, enemyLayerMask);
        float nearest = float.MaxValue;
        target = null;
        foreach (var h in hits)
        {
            float d = Vector2.Distance(transform.position, h.transform.position);
            if (d < nearest) { nearest = d; target = h.transform; }
        }
    }
}
