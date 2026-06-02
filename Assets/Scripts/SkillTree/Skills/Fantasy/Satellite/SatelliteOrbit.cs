using UnityEngine;

public class SatelliteOrbit : MonoBehaviour
{
    [SerializeField] float orbitRadius = 2f;
    [SerializeField] float orbitSpeed = 180f;
    [SerializeField] float hitRadius = 0.3f;
    [SerializeField] float hitCooldown = 0.5f;
    [SerializeField] GameObject hitVfxPrefab;

    private Transform playerTransform;
    private SatelliteController controller;
    private Rigidbody2D rb;
    private float currentAngle;
    private float hitTimer;
    private int enemyMask;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyMask = LayerMask.GetMask("Enemy");
    }

    public void Init(Transform player, SatelliteController ctrl)
    {
        playerTransform = player;
        controller = ctrl;
    }

    public void SetAngle(float angle)
    {
        currentAngle = angle;
    }

    private void FixedUpdate()
    {
        if (playerTransform == null) return;

        currentAngle += orbitSpeed * Time.fixedDeltaTime;
        float rad = currentAngle * Mathf.Deg2Rad;
        Vector2 nextPos = (Vector2)playerTransform.position
            + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * orbitRadius;

        if (rb != null)
            rb.MovePosition(nextPos);
        else
            transform.position = nextPos;

        hitTimer += Time.fixedDeltaTime;
        if (hitTimer < hitCooldown) return;

        var hits = Physics2D.OverlapCircleAll(transform.position, hitRadius, enemyMask);
        if (hits.Length == 0) return;

        hitTimer = 0f;

        var statCore = playerTransform.GetComponent<PlayerStatManager>()?.StatCore;
        if (statCore == null || controller == null) return;

        float dmg = statCore.getStat(StatType.SKILL_DAMAGE).calibratedValue * controller.damageMultiplier;

        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<Enemy>();
            if (enemy == null) continue;

            enemy.TakeDamage(dmg);

            if (controller.hasKnockback)
            {
                Vector2 dir = ((Vector2)hit.transform.position - (Vector2)transform.position).normalized;
                enemy.ApplyKnockback(dir * 8f);
            }

            if (hitVfxPrefab != null)
                Instantiate(hitVfxPrefab, hit.transform.position, Quaternion.identity);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitRadius);

        if (playerTransform == null) return;
        int segments = 36;
        Vector3 prev = playerTransform.position + Vector3.right * orbitRadius;
        for (int i = 1; i <= segments; i++)
        {
            float a = (360f / segments) * i * Mathf.Deg2Rad;
            Vector3 next = playerTransform.position
                + new Vector3(Mathf.Cos(a), Mathf.Sin(a)) * orbitRadius;
            Debug.DrawLine(prev, next, Color.cyan);
            prev = next;
        }
    }
#endif
}
