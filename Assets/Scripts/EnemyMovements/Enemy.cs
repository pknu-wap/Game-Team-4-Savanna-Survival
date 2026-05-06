using System.Collections;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Drop")]
    [SerializeField] protected DropTable dropTable;

    [Header("Contact Damage")]
    [SerializeField] private float contactDamage   = 5f;
    [SerializeField] private float contactCooldown = 1f;

    [Header("Kill Reward")]
    [SerializeField] private float hungerReward = 10f; 

    protected Rigidbody2D    rb;
    protected Transform      player;
    protected PlayerStatCore playerStatCore; 

    protected EnemyStatManager statManager;
    protected float            currentHp;

    private float contactTimer;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        statManager = new EnemyStatManager();
    }

    protected virtual void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player         = playerObj.transform;
            playerStatCore = playerObj.GetComponent<PlayerStatManager>()?.StatCore;
        }
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


    protected void DamagePlayer(float damage)
    {
        if (playerStatCore == null) return;

        float currentHp = playerStatCore.getStat(StatType.HEALTH).rawValue;
        float next      = Mathf.Max(0f, currentHp - damage);
        playerStatCore.registerStat(StatType.HEALTH, next);
    }


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
        if (!other.CompareTag("Player"))    return;
        if (contactTimer < contactCooldown) return;

        DamagePlayer(contactDamage);
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
        AddPlayerHunger(hungerReward);

        if (dropTable != null)
            dropTable.Drop(transform.position);

        Destroy(gameObject);
    }
    // public void ApplySlow(float duration, float slowPercent)
    // {
    //     if (slowCoroutine != null)
    //     {
    //         StopCoroutine(slowCoroutine);
    //     }

    //     slowCoroutine = StartCoroutine(Slow(duration, slowPercent));
    // }

    // IEnumerator Slow(float duration, float slowPercent)
    // {
    //     moveSpeed = originalSpeed * (1f - slowPercent);

    //     // if (sr != null)
    //     //     sr.color = Color.blue;

    //     yield return new WaitForSeconds(duration);

    //     moveSpeed = originalSpeed;

    //     // if (sr != null)
    //     //     sr.color = Color.white;
    // }
    private void AddPlayerHunger(float amount)
    {
        if (playerStatCore == null) return;

        float current   = playerStatCore.getStat(StatType.HUNGER).rawValue;
        float maxHunger = playerStatCore.getStat(StatType.MAX_HUNGER).rawValue;
        float next      = Mathf.Min(current + amount, maxHunger);

        playerStatCore.registerStat(StatType.HUNGER, next);

        Debug.Log($"[적 처치 보상] {gameObject.name} 처치" +
                  $"\n 배고픔 +{amount} ({current:F1} → {next:F1} / {maxHunger:F1})");
    }
}
