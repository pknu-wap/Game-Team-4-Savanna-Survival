using UnityEngine;

public class PoisonProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;

    private Rigidbody2D rb;
    private float       zoneRadius;
    private float       zoneDuration;
    private EnemyMamba  owner;

    public void Init(Vector2 dir, float zoneRadius, float zoneDuration, EnemyMamba owner)
    {
        this.zoneRadius   = zoneRadius;
        this.zoneDuration = zoneDuration;
        this.owner        = owner;

        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity  = dir * speed;
        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Wall")) return;
        owner?.SpawnPoisonZone(transform.position, zoneRadius, zoneDuration);
        Destroy(gameObject);
    }
}