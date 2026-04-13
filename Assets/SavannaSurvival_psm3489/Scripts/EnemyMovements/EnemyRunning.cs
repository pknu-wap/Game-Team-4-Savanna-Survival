using UnityEngine;

public class EnemyRunning : Enemy
{
    [Header("Config")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float maxHp = 30f;
    [SerializeField] private float detectionRange = 6f;

    [Header("Wander")]
    [SerializeField] private float wanderRadius = 3f;
    [SerializeField] private float arriveDistance = 0.2f;
    [SerializeField] private float idleChance = 0.2f;

    private Vector2 velocity;
    private Vector2 wanderTarget;

    private bool isIdle;
    private float idleTimer;

    protected override void Awake()
    {
        base.Awake();

        statManager.InitRunner(maxHp);
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

        if (dist <= detectionRange)
            Flee();
        else
            Wander();
    }

    protected override bool IsPlayerInDetection() => true;

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

    private void Flee()
    {
        Vector2 dir = (transform.position - player.position).normalized;
        MoveSmooth(dir * moveSpeed);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(wanderTarget, 0.1f);
    }
}