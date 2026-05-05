using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float speed    = 8f;
    [SerializeField] private float lifeTime = 3f;

    private Vector2 direction;
    private float   damage;
    private bool    initialized = false;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 dir, float dmg)
    {
        direction   = dir;
        damage      = dmg;
        initialized = true;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        if (!initialized) return;
        rb.linearVelocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) 
        {
            if (!other.isTrigger)
                Destroy(gameObject);
            return;
        }

        PlayerStatCore statCore = other.GetComponent<PlayerStatManager>()?.StatCore;
        if (statCore != null)
        {
            float currentHp = statCore.getStat(StatType.HEALTH).rawValue;
            float next      = Mathf.Max(0f, currentHp - damage);
            statCore.registerStat(StatType.HEALTH, next);
        }

        Destroy(gameObject);
    }
}