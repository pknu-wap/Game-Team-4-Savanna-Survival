using UnityEngine;

public class EnemyAttacking : Enemy
{
    [Header("Config")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float maxHp = 50f;
    [SerializeField] private float damage = 10f;

    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackInterval = 2f;

    [Header("Wander")]
    [SerializeField] private float wanderRadius = 3f;
    [SerializeField] private float arriveDistance = 0.2f;
    [SerializeField] private float idleChance = 0.2f;

    [Header("Attack Telegraph")]
    [SerializeField] private GameObject attackIndicatorPrefab;

    private Vector2 velocity;
    private Vector2 wanderTarget;

    private bool isIdle;
    private float idleTimer;

    private float attackTimer;
    private bool telegraph;

    private GameObject indicator;

    protected override void Awake()
    {
        base.Awake();

        statManager.InitAttacker(maxHp, damage);
        currentHp = statManager.getStat(StatType.HEALTH).rawValue;

        SetNewWanderTarget();
    }

    protected override void Move()
    {
        if (player == null || !player.gameObject.activeInHierarchy)
        {
            Wander();
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackRange)
            Attack();
        else if (dist <= detectionRange)
            Chase();
        else
            Wander();
    }

    protected override bool IsPlayerInDetection() => true;

    // Wander
    private void Wander()
    {
        if (isIdle)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer > 1f)
            {
                isIdle = false;
                idleTimer = 0f;
                SetNewWanderTarget();
            }

            MoveSmooth(Vector2.zero);
            return;
        }

        if (Vector2.Distance(transform.position, wanderTarget) < arriveDistance)
        {
            if (Random.value < idleChance)
            {
                isIdle = true;
                return;
            }

            SetNewWanderTarget();
        }

        Vector2 dir = (wanderTarget - (Vector2)transform.position).normalized;
        MoveSmooth(dir * moveSpeed * 0.7f);
    }

    private void SetNewWanderTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        wanderTarget = (Vector2)transform.position + randomOffset;
    }



    private void Chase()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        MoveSmooth(dir * moveSpeed);

        attackTimer = 0f;
        telegraph = false;
        HideIndicator();
    }

    private void Attack()
    {
        MoveSmooth(Vector2.zero);

        attackTimer += Time.deltaTime;

        if (!telegraph && attackTimer >= attackInterval - 1f)
        {
            ShowIndicator();
            telegraph = true;
        }

        if (attackTimer >= attackInterval)
        {
            float dmg = statManager.getStat(StatType.DAMAGE).calibratedValue;

            player.GetComponent<PlayerDummy>()
                ?.TakeDamage(dmg);

            attackTimer = 0f;
            telegraph = false;
            HideIndicator();
        }
    }


    private void MoveSmooth(Vector2 targetVel)
    {
        velocity = Vector2.Lerp(
            velocity,
            targetVel,
            Time.deltaTime * 10f
        );

        rb.linearVelocity = velocity;
    }


    private void ShowIndicator()
    {
        if (attackIndicatorPrefab == null) return;

        if (indicator == null)
            indicator = Instantiate(attackIndicatorPrefab, transform);

        indicator.transform.localPosition = Vector3.zero;
        indicator.transform.localScale = Vector3.one * attackRange * 2f;
        indicator.SetActive(true);
    }

    private void HideIndicator()
    {
        if (indicator != null)
            indicator.SetActive(false);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

    }
}