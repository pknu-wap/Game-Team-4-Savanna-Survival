using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 8f;
    [SerializeField] float lifetime = 5f;
    [SerializeField] GameObject hitVfxPrefab;
    [SerializeField] GameObject explosionVfxPrefab;

    private bool hasExplosion;
    private float explosionRadius;
    private Transform target;
    private PlayerStatManager statManager;
    private bool dead;
    private Vector2 lastTargetPos;
    private Rigidbody2D rb;

    public void Init(Transform t, PlayerStatManager sm, bool explosion, float explosionRad)
    {
        target = t;
        statManager = sm;
        hasExplosion = explosion;
        explosionRadius = explosionRad;
        lastTargetPos = t != null ? (Vector2)t.position : (Vector2)transform.position;
        rb = GetComponent<Rigidbody2D>();
        Debug.Log($"[Missile] Init: target={t?.name ?? "null"}, rb={rb != null}, hasExplosion={explosion}");
    }

    private void FixedUpdate()
    {
        if (dead) return;

        lifetime -= Time.fixedDeltaTime;
        if (lifetime <= 0f)
        {
            Debug.Log("[Missile] FixedUpdate: lifetime 만료 → Destroy");
            Destroy(gameObject);
            return;
        }

        Vector2 dir;
        if (target != null && target.gameObject.activeSelf)
        {
            lastTargetPos = target.position;
            dir = ((Vector2)target.position - (Vector2)transform.position).normalized;
        }
        else
        {
            Vector2 toLastPos = lastTargetPos - (Vector2)transform.position;
            if (toLastPos.magnitude < 0.15f)
            {
                Debug.Log("[Missile] FixedUpdate: 마지막 위치 도달 → Destroy");
                Destroy(gameObject);
                return;
            }
            dir = toLastPos.normalized;
        }

        if (rb != null)
            rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
        else
            transform.position += (Vector3)(dir * moveSpeed * Time.fixedDeltaTime);

        transform.up = dir;

        // Physics 2D 레이어 매트릭스와 무관하게 적 충돌 수동 체크
        var col = GetComponent<CircleCollider2D>();
        if (col != null)
        {
            float checkRadius = col.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
            Collider2D hit = Physics2D.OverlapCircle((Vector2)transform.position, checkRadius, LayerMask.GetMask("Enemy"));
            if (hit != null)
            {
                Debug.Log($"[Missile] OverlapCircle hit: {hit.gameObject.name}");
                Die(hit);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        Debug.Log($"[Missile] OnTriggerEnter2D: hit={other.gameObject.name}, layer={other.gameObject.layer}(Enemy={enemyLayer}), dead={dead}, hasEnemy={other.GetComponent<Enemy>() != null}");

        if (dead) return;
        if (other.GetComponent<Enemy>() == null) return;

        Die(other);
    }

    private void Die(Collider2D hitCollider)
    {
        dead = true;
        Debug.Log($"[Missile] Die: pos={transform.position}, hasExplosion={hasExplosion}");

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;
        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        float dmg = 0f;
        if (statManager != null)
            dmg = statManager.StatCore.getStat(StatType.SKILL_DAMAGE).calibratedValue;

        Debug.Log($"[Missile] Die: dmg={dmg}");

        int enemyLayerMask = LayerMask.GetMask("Enemy");

        if (!hasExplosion)
        {
            var enemy = hitCollider.GetComponent<Enemy>();
            Debug.Log($"[Missile] Die: 단일 타격 → enemy={enemy?.name ?? "null"}");
            enemy?.TakeDamage(dmg);
        }
        else
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayerMask);
            Debug.Log($"[Missile] Die: 폭발 → 범위 내 적 {hits.Length}체");
            foreach (var hit in hits)
                hit.GetComponent<Enemy>()?.TakeDamage(dmg);

            if (explosionVfxPrefab != null)
                Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);
        }

        if (hitVfxPrefab != null)
            Instantiate(hitVfxPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
