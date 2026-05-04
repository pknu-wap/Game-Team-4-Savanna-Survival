using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Drop")]
    [SerializeField] protected DropTable dropTable;

    [Header("Contact Damage")]
    [SerializeField] private float contactDamage   = 5f;  
    [SerializeField] private float contactCooldown = 1f;  

    protected Rigidbody2D rb;
    protected Transform player;

    protected EnemyStatManager statManager;
    protected float currentHp;

    private float contactTimer; 


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        statManager = new EnemyStatManager();
    }

    protected virtual void FixedUpdate()
    {
        Move();
    }

    protected virtual void Update()
    {
        if (contactTimer < contactCooldown)
            contactTimer += Time.deltaTime;
    }


    protected abstract void Move();

    protected abstract bool IsPlayerInDetection();



    private void OnTriggerEnter2D(Collider2D other)
    {
        TryApplyContactDamage(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryApplyContactDamage(collision.gameObject);
    }

    private void TryApplyContactDamage(GameObject other)
    {
        if (!other.CompareTag("Player")) return;

        if (contactTimer < contactCooldown) return;

        PlayerDummy playerDummy = other.GetComponent<PlayerDummy>();
        if (playerDummy == null) return;

        playerDummy.TakeDamage(contactDamage);
        contactTimer = 0f; 
    }


    public virtual void TakeDamage(float damage)
    {
        currentHp -= damage;

        if (currentHp <= 0)
            Die();
    }

    protected virtual void Die()
    {
        if (dropTable != null)
            dropTable.Drop(transform.position);

        Destroy(gameObject);
    }
}