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
    private int targetMask;

    // ✅ 보스 소유 여부 캐시
    private bool isBossOwned;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Transform owner, SatelliteController ctrl)
    {
        playerTransform = owner;
        controller      = ctrl;

        // ✅ BossSatelliteController면 Player 레이어, 아니면 Enemy 레이어
        if (ctrl is BossSatelliteController bossCtrl)
        {
            isBossOwned = true;
            targetMask  = (int)bossCtrl.TargetLayer;
        }
        else
        {
            isBossOwned = false;
            targetMask  = LayerMask.GetMask("Enemy");
        }
    }

    public void SetAngle(float angle) => currentAngle = angle;

    private void FixedUpdate()
    {
        if (playerTransform == null) return;

        currentAngle += orbitSpeed * Time.fixedDeltaTime;
        float  rad     = currentAngle * Mathf.Deg2Rad;
        Vector2 nextPos = (Vector2)playerTransform.position
            + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * orbitRadius;

        if (rb != null) rb.MovePosition(nextPos);
        else            transform.position = nextPos;

        hitTimer += Time.fixedDeltaTime;
        if (hitTimer < hitCooldown) return;

        var hits = Physics2D.OverlapCircleAll(transform.position, hitRadius, targetMask);
        if (hits.Length == 0) return;

        hitTimer = 0f;

        var statCore = playerTransform.GetComponent<PlayerStatManager>()?.StatCore;
        if (statCore == null || controller == null) return;

        float dmg = statCore.getStat(StatType.SKILL_DAMAGE).calibratedValue
                  * controller.damageMultiplier;

        foreach (var hit in hits)
        {
            if (isBossOwned)
            {
                // ✅ 보스 위성 — 플레이어에게 데미지
                var playerEffect = hit.GetComponent<PlayerEffectTemp>();
                if (playerEffect != null)
                {
                    playerEffect.TakeDamage(dmg);
                }
                else
                {
                    var statManager = hit.GetComponent<PlayerStatManager>();
                    if (statManager == null) continue;
                    float current = statManager.StatCore.getStat(StatType.HEALTH).rawValue;
                    statManager.StatCore.registerStat(StatType.HEALTH,
                                                      Mathf.Max(0f, current - dmg));
                }
            }
            else
            {
                // 플레이어 위성 — 적에게 데미지
                var enemy = hit.GetComponent<Enemy>();
                if (enemy == null) continue;

                enemy.TakeDamage(dmg);

                if (controller.hasKnockback)
                {
                    Vector2 dir = ((Vector2)hit.transform.position
                                 - (Vector2)transform.position).normalized;
                    enemy.ApplyKnockback(dir * 8f);
                }
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
            float   a    = (360f / segments) * i * Mathf.Deg2Rad;
            Vector3 next = playerTransform.position
                         + new Vector3(Mathf.Cos(a), Mathf.Sin(a)) * orbitRadius;
            Debug.DrawLine(prev, next, Color.cyan);
            prev = next;
        }
    }
#endif
}
