using UnityEngine;

public class SatelliteOrbit : MonoBehaviour
{
    [SerializeField] float orbitRadius = 2f;
    [SerializeField] float orbitSpeed = 180f;
    [SerializeField] GameObject hitVfxPrefab;

    private Transform playerTransform;
    private SatelliteController controller;
    private Rigidbody2D rb;
    private float currentAngle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Transform player, SatelliteController ctrl)
    {
        playerTransform = player;
        controller = ctrl;
        int index = ctrl.activeSatellites.Count;
        int total = Mathf.Max(1, ctrl.maxSatelliteCount);
        currentAngle = index * (360f / total);
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Enemy")) return;
        if (controller == null || playerTransform == null) return;

        var statCore = playerTransform.GetComponent<PlayerStatManager>()?.StatCore;
        if (statCore == null) return;

        float dmg = statCore.getStat(StatType.SKILL_DAMAGE).calibratedValue * controller.damageMultiplier;
        var enemy = other.GetComponent<Enemy>();
        if (enemy == null) return;

        enemy.TakeDamage(dmg);

        if (controller.hasKnockback)
        {
            var rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 dir = ((Vector2)other.transform.position - (Vector2)playerTransform.position).normalized;
                rb.AddForce(dir * 5f, ForceMode2D.Impulse);
            }
        }

        if (hitVfxPrefab != null)
            Instantiate(hitVfxPrefab, other.transform.position, Quaternion.identity);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
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
