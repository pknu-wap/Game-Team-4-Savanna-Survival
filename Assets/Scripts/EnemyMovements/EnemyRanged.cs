using UnityEngine;

public class EnemyRanged : Enemy
{
    [Header("Config")]
    [SerializeField] private float moveSpeed   = 2f;
    [SerializeField] private float maxHp       = 40f;
    [SerializeField] private float damage      = 8f;

    [SerializeField] private float detectionRange = 10f; 
    [SerializeField] private float preferredRange = 6f;  
    [SerializeField] private float attackRange    = 8f;  
    [SerializeField] private float attackInterval = 2.5f; 

    [Header("Wander")]
    [SerializeField] private float wanderRadius   = 3f;
    [SerializeField] private float arriveDistance = 0.2f;
    [SerializeField] private float idleChance     = 0.2f;

    [Header("Attack Indicator (Sprite)")]
    [SerializeField] private GameObject indicatorPrefab;

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;


    private Vector2 velocity;
    private Vector2 wanderTarget;

    private bool  isIdle;
    private float idleTimer;

    private float attackTimer;
    private bool  telegraph; 

    private GameObject indicator; 

    protected override void Awake()
    {
        base.Awake();

        statManager.InitAttacker(maxHp, damage);
        currentHp = statManager.getStat(StatType.HEALTH).rawValue;

        SetNewWanderTarget();
    }

    private void Update()
    {
        base.Update(); 
        if (telegraph && player != null && indicator != null && indicator.activeSelf)
            UpdateIndicatorTransform();
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
        {
            if (dist < preferredRange)
                Retreat();
            else if (dist <= attackRange)
                AttackPlayer();
            else
                Approach();
        }
        else
        {
            Wander();
        }
    }

    protected override bool IsPlayerInDetection() => true;


    private void Wander()
    {
        if (isIdle)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer > 1f)
            {
                isIdle    = false;
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

    private void Approach()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        MoveSmooth(dir * moveSpeed * 0.8f);

        ResetAttack();
    }

    private void Retreat()
    {
        Vector2 dir = (transform.position - player.position).normalized;
        MoveSmooth(dir * moveSpeed);

        ResetAttack();
    }

    private void ResetAttack()
    {
        attackTimer = 0f;
        telegraph   = false;
        HideIndicator();
    }


    private void AttackPlayer()
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
            FireProjectile();

            attackTimer = 0f;
            telegraph   = false;
            HideIndicator();
        }
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null) return;

        float   dmg = statManager.getStat(StatType.DAMAGE).calibratedValue;
        Vector2 dir = (player.position - transform.position).normalized;

        GameObject go   = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile proj = go.GetComponent<Projectile>();
        if (proj != null)
            proj.Init(dir, dmg);
    }


    private void ShowIndicator()
    {
        if (indicatorPrefab == null) return;

        if (indicator == null)
            indicator = Instantiate(indicatorPrefab);

        indicator.SetActive(true);
        UpdateIndicatorTransform(); 
    }

    private void HideIndicator()
    {
        if (indicator != null)
            indicator.SetActive(false);
    }

    private void UpdateIndicatorTransform()
    {
        Vector2 dir  = (player.position - transform.position).normalized;
        float   dist = Vector2.Distance(transform.position, player.position);

        float   halfLength      = attackRange * 0.5f;
        Vector2 indicatorCenter = (Vector2)transform.position + dir * halfLength;
        indicator.transform.position = indicatorCenter;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        indicator.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        indicator.transform.localScale = new Vector3(attackRange, 1f, 1f);
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

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, preferredRange);
    }
}