using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 8f;
    [SerializeField] float lifetime = 5f;
    [SerializeField] GameObject hitVfxPrefab;
    [SerializeField] GameObject explosionVfxPrefab;

    private bool hasExplosion;
    private float explosionRadius;
    private float damageMultiplier;
    private float explosionDamageBonus;
    private Transform target;
    private PlayerStatManager statManager;
    private bool dead;
    private Vector2 lastTargetPos;
    private Rigidbody2D rb;

    public void Init(Transform t, PlayerStatManager sm, bool explosion, float explosionRad,
                     float dmgMultiplier = 1f, float explosionDmgBonus = 0f,
                     Collider2D ownerCol = null)
    {
        target               = t;
        statManager          = sm;
        hasExplosion         = explosion;
        explosionRadius      = explosionRad;
        damageMultiplier     = dmgMultiplier;
        explosionDamageBonus = explosionDmgBonus;
        lastTargetPos        = t != null ? (Vector2)t.position : (Vector2)transform.position;
        rb                   = GetComponent<Rigidbody2D>();

        // ✅ 발사자 콜라이더와 미사일 콜라이더 간 충돌을 물리 레벨에서 직접 끔
        if (ownerCol != null)
        {
            var myCol = GetComponent<Collider2D>();
            if (myCol != null)
                Physics2D.IgnoreCollision(myCol, ownerCol, true);

            // 보스에 콜라이더가 여러 개인 경우도 대응
            var ownerCols = ownerCol.GetComponentsInParent<Collider2D>();
            foreach (var c in ownerCols)
                if (myCol != null) Physics2D.IgnoreCollision(myCol, c, true);
        }
    }

    private void FixedUpdate()
    {
        if (dead) return;

        lifetime -= Time.fixedDeltaTime;
        if (lifetime <= 0f) { Destroy(gameObject); return; }

        Vector2 dir;
        if (target != null && target.gameObject.activeSelf)
        {
            lastTargetPos = target.position;
            dir = ((Vector2)target.position - (Vector2)transform.position).normalized;
        }
        else
        {
            Vector2 toLastPos = lastTargetPos - (Vector2)transform.position;
            if (toLastPos.magnitude < 0.15f) { Destroy(gameObject); return; }
            dir = toLastPos.normalized;
        }

        if (rb != null)
            rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
        else
            transform.position += (Vector3)(dir * moveSpeed * Time.fixedDeltaTime);

        transform.up = dir;

        // ✅ OverlapCircle 수동 체크 제거 — OnTriggerEnter2D에만 의존
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (dead) return;
        if (other.GetComponent<Enemy>() == null) return;
        Die(other);
    }

    private void Die(Collider2D hitCollider)
    {
        dead = true;

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;
        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        float baseDmg = statManager != null
            ? statManager.StatCore.getStat(StatType.SKILL_DAMAGE).calibratedValue
            : 0f;

        if (!hasExplosion)
        {
            hitCollider.GetComponent<Enemy>()?.TakeDamage(baseDmg * damageMultiplier);
        }
        else
        {
            float explosionDmg = baseDmg * (damageMultiplier + explosionDamageBonus);
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius,
                                                           LayerMask.GetMask("Enemy"));
            foreach (var hit in hits)
                hit.GetComponent<Enemy>()?.TakeDamage(explosionDmg);

            if (explosionVfxPrefab != null)
            {
                var vfxObj = Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);
                vfxObj.GetComponent<MissileExplosionVfxController>()?.Init(explosionRadius);
            }
        }

        if (hitVfxPrefab != null)
            Instantiate(hitVfxPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}