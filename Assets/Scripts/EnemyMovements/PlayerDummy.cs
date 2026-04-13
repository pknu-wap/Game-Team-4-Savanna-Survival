using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDummy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxHp = 100f;
    [SerializeField] private float damage = 10f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1f;

    private float currentHp;
    private float currentExp;
    private Vector2 moveInput;

    private float attackTimer;

    private Rigidbody2D rb;
    private PlayerInputActions inputActions;

    private PlayerTempStatManager statManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        statManager = new PlayerTempStatManager();
        statManager.Init(maxHp, damage);

        currentHp = statManager
            .getStat(StatType.HEALTH)
            .rawValue;

        inputActions = new PlayerInputActions();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Attack.performed += _ => TryAttack();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }


    private void TryAttack()
    {
        if (attackTimer < attackCooldown)
            return;

        Attack();
        attackTimer = 0f;
    }

    private void Attack()
    {
        Debug.Log("플레이어 공격!");

        float finalDamage = statManager
            .getStat(StatType.DAMAGE)
            .calibratedValue;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            attackRange
        );

        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(finalDamage);
            }
        }
    }


    public void TakeDamage(float damage)
    {
        currentHp -= damage;

        Debug.Log($"플레이어 체력: {currentHp}");

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("플레이어 사망");
        Destroy(gameObject);
    }


    public void AddExp(int amount)
    {
        currentExp += amount;
        Debug.Log($"EXP 획득: +{amount} / 현재 EXP: {currentExp}");
    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}