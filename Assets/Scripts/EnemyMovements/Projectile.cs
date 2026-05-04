using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float speed         = 8f;   
    [SerializeField] private float lifeTime      = 3f;   

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
        PlayerDummy playerDummy = other.GetComponent<PlayerDummy>();
        if (playerDummy != null)
        {
            playerDummy.TakeDamage(damage);
            Destroy(gameObject); 
            return;
        }


        if (!other.isTrigger)
            Destroy(gameObject);
    }
}