using UnityEngine;

public class EnemyRanged : Enemy
{
    [Header("Config")]
    [SerializeField] private float maxHp          = 40f;
    [SerializeField] private float damage         = 8f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float preferredRange = 6f;
    [SerializeField] private float attackRange    = 8f;
    [SerializeField] private float attackInterval = 2.5f;

    [Tooltip("교전 시작 후 이 거리까지 벗어나야 Wander로 전환 (detectionRange보다 커야 함)")]
    [SerializeField] private float disengageRange = 14f;

    [Header("Attack Indicator")]
    [SerializeField] private GameObject indicatorPrefab;

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;

    [Header("Animation")]
    [SerializeField] private string dieClipName = "Die";

    private float      attackTimer;
    private bool       telegraph;
    private GameObject indicator;
    private bool       isEngaged;
    private bool       isRetreating;

    private Animator anim;

    private static readonly int ParamSpeed = Animator.StringToHash("Speed");
    private static readonly int ParamShoot = Animator.StringToHash("Shoot");
    private static readonly int ParamDie   = Animator.StringToHash("Die");

    protected override void Awake()
    {
        base.Awake();
        statManager.InitAttacker(maxHp, damage);
        currentHp = statManager.getStat(StatType.HEALTH).rawValue;
        SetNewWanderTarget();
        anim = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();
        anim?.SetFloat(ParamSpeed, rb.linearVelocity.magnitude);
        UpdateFacing();
        if (telegraph && player != null && indicator != null && indicator.activeSelf)
            UpdateIndicatorTransform();
    }

    protected override bool IsPlayerInDetection() => true;

    protected override void Move()
    {
        if (player == null || !player.gameObject.activeInHierarchy) { Wander(); return; }

        float dist = Vector2.Distance(transform.position, player.position);

        if (!isEngaged && dist <= detectionRange) isEngaged = true;
        if (isEngaged  && dist >= disengageRange) isEngaged = false;

        if (!isEngaged) { Wander(); return; }

        if (!isRetreating && dist < preferredRange) isRetreating = true;
        if (isRetreating  && dist >= attackRange)   isRetreating = false;

        if      (isRetreating)        Retreat();
        else if (dist <= attackRange) AttackPlayer();
        else                          Approach();
    }

    private void Approach()
    {
        MoveSmooth((player.position - transform.position).normalized * moveSpeed * 0.8f);
        ResetAttack();
    }

    private void Retreat()
    {
        MoveSmooth((transform.position - player.position).normalized * moveSpeed);
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
            anim?.SetTrigger(ParamShoot);
            FireProjectile();
            attackTimer = 0f;
            telegraph   = false;
            HideIndicator();
        }
    }

    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.linearVelocity = Vector2.zero;
        ApplyDeathRewards();
        anim?.SetTrigger(ParamDie);
        if (indicator != null) Destroy(indicator);
        StartCoroutine(DieRoutine(GetDieClipLength(anim, dieClipName)));
    }

    private void UpdateFacing()
    {
        if (Mathf.Abs(velocity.x) < 0.05f) return;
        Vector3 s = transform.localScale;
        s.x = velocity.x > 0 ? -Mathf.Abs(s.x) : Mathf.Abs(s.x);
        transform.localScale = s;
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null) return;
        Vector2    dir = (player.position - transform.position).normalized;
        GameObject go  = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        go.GetComponent<Projectile>()?.Init(dir, statManager.getStat(StatType.DAMAGE).calibratedValue);
    }

    private void ShowIndicator()
    {
        if (indicatorPrefab == null) return;
        indicator ??= Instantiate(indicatorPrefab);
        indicator.SetActive(true);
        UpdateIndicatorTransform();
    }

    private void HideIndicator()
    {
        if (indicator != null) indicator.SetActive(false);
    }

    private void UpdateIndicatorTransform()
    {
        if (indicator == null || player == null) return;
        Vector2 dir    = (player.position - transform.position).normalized;
        float   dist   = Vector2.Distance(transform.position, player.position);
        Vector2 center = (Vector2)transform.position + dir * (dist * 0.5f);
        indicator.transform.SetPositionAndRotation(center, Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg));
        indicator.transform.localScale = new Vector3(dist, indicator.transform.localScale.y, 1f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;   Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;    Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, preferredRange);
        Gizmos.color = Color.gray;   Gizmos.DrawWireSphere(transform.position, disengageRange);
    }
}