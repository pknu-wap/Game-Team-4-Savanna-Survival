using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Drop")]
    [SerializeField] protected DropTable dropTable;

    protected Rigidbody2D rb;
    protected Transform player;

    protected EnemyStatManager statManager;
    protected float currentHp;


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


    protected abstract void Move();

    protected abstract bool IsPlayerInDetection();


    public virtual void TakeDamage(float damage)
    {
        currentHp -= damage;

        if (currentHp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (dropTable != null)
        {
            dropTable.Drop(transform.position);
        }

        Destroy(gameObject);
    }
}