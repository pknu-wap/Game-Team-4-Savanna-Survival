using UnityEngine;

public class EnemyRunning : Enemy
{
    [Header("Config")]
    [SerializeField] private float maxHp          = 30f;
    [SerializeField] private float detectionRange = 6f;

    [Tooltip("도망 시작 후 이 거리까지 벗어나야 Wander로 전환 (detectionRange보다 커야 함)")]
    [SerializeField] private float fleeStopRange  = 9f;

    private bool     isFleeing;
    [Header("Animation")]
    [SerializeField] private string dieClipName = "Die";

    private Animator anim;

    private static readonly int ParamSpeed = Animator.StringToHash("Speed");
    private static readonly int ParamDie   = Animator.StringToHash("Die");

    protected override void Awake()
    {
        base.Awake();
        statManager.InitRunner(maxHp);
        currentHp = statManager.getStat(StatType.HEALTH).rawValue;
        SetNewWanderTarget();

        anim = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();
        UpdateAnimator();
        UpdateFacing();
    }

    protected override bool IsPlayerInDetection() => true;

    protected override void Move()
    {
        if (player == null || !player.gameObject.activeInHierarchy) { Wander(); return; }

        float dist = Vector2.Distance(transform.position, player.position);

        //진입과 해제 범위를 분리해 경계선에서의 상태 진동을 방지
        if (!isFleeing && dist <= detectionRange) isFleeing = true;
        if (isFleeing  && dist >= fleeStopRange)  isFleeing = false;

        if (isFleeing)
            MoveSmooth((transform.position - player.position).normalized * moveSpeed);
        else
            Wander();
    }

    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.linearVelocity = Vector2.zero;
        ApplyDeathRewards();

        anim?.SetTrigger(ParamDie);

        StartCoroutine(DieRoutine(GetDieClipLength(anim, dieClipName)));
    }

    // ── 애니메이터 / 방향 ─────────────────────────────────────
    private void UpdateAnimator()
    {
        if (anim == null) return;
        anim.SetFloat(ParamSpeed, rb.linearVelocity.magnitude);
    }

    private void UpdateFacing()
    {
        if (Mathf.Abs(velocity.x) < 0.05f) return;

        Vector3 s = transform.localScale;
        s.x = velocity.x > 0 ? -Mathf.Abs(s.x) : Mathf.Abs(s.x);
        transform.localScale = s;
    }

    // ─────────────────────────────────────────────────────────

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;  Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.cyan;  Gizmos.DrawWireSphere(transform.position, fleeStopRange);
        Gizmos.color = Color.green; Gizmos.DrawSphere(wanderTarget, 0.1f);
    }
}