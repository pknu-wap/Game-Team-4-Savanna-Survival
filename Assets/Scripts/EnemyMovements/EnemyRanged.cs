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

    private float      attackTimer;
    private bool       telegraph;
    private GameObject indicator;

    // 감지 진입과 해제 범위를 분리해 경계선 진동을 방지
    private bool isEngaged;

    // preferredRange 경계에서의 진동을 방지
    // true  : preferredRange 안으로 들어온 상태 → 공격 가능 거리까지 후퇴
    // false : 충분히 멀어진 상태 → 공격 or 접근
    private bool isRetreating;

    protected override void Awake()
    {
        base.Awake();
        statManager.InitAttacker(maxHp, damage);
        currentHp = statManager.getStat(StatType.HEALTH).rawValue;
        SetNewWanderTarget();
    }

    protected override void Update()
    {
        base.Update();
        if (telegraph && player != null && indicator != null && indicator.activeSelf)
            UpdateIndicatorTransform();
    }

    protected override bool IsPlayerInDetection() => true;

    protected override void Move()
    {
        if (player == null || !player.gameObject.activeInHierarchy) { Wander(); return; }

        float dist = Vector2.Distance(transform.position, player.position);

        // detectionRange 진입 시 교전 시작, disengageRange 이탈 시 교전 해제
        if (!isEngaged && dist <= detectionRange) isEngaged = true;
        if (isEngaged  && dist >= disengageRange) isEngaged = false;

        if (!isEngaged) { Wander(); return; }

        // 너무 가까워지면(< preferredRange) 후퇴 시작
        // 공격 가능 거리(attackRange) 밖으로 나가야 후퇴 해제
        if (!isRetreating && dist < preferredRange)  isRetreating = true;
        if (isRetreating  && dist >= attackRange)    isRetreating = false;

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
            FireProjectile();
            attackTimer = 0f;
            telegraph   = false;
            HideIndicator();
        }
    }

    protected override void Die()
    {
        if (indicator != null) Destroy(indicator);
        base.Die();
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null) return;
        Vector2 dir = (player.position - transform.position).normalized;
        GameObject go = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
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