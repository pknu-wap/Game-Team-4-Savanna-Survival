using UnityEngine;

public class EnemyBleed : Enemy
{
    [Header("Config")]
    [SerializeField] private float maxHp          = 50f;
    [SerializeField] private float damage         = 10f;
    [SerializeField] private float attackRange    = 2f;
    [SerializeField] private float attackInterval = 2f;

    [Header("Attack Telegraph")]
    [SerializeField] private GameObject attackIndicatorPrefab;

    [Header("Bleed")]
    [SerializeField] private int   bleedStacks    = 1;
    [SerializeField] private float bleedDotDamage = 3f;
    [SerializeField] private float bleedDuration  = 4f;

    [Header("Animation")]
    [SerializeField] private string dieClipName = "Die";

    private float      attackTimer;
    private bool       telegraph;
    private GameObject indicator;
    private Animator   anim;

    private static readonly int ParamSpeed  = Animator.StringToHash("Speed");
    private static readonly int ParamAttack = Animator.StringToHash("Attack");
    private static readonly int ParamDie    = Animator.StringToHash("Die");

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
    }

    protected override bool IsPlayerInDetection() => true;

    protected override void Move()
    {
        if (player == null || !player.gameObject.activeInHierarchy) { Wander(); return; }

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackRange) Attack();
        else                     Chase();
    }

    private void Chase()
    {
        MoveSmooth((player.position - transform.position).normalized * moveSpeed);
        attackTimer = 0f;
        telegraph   = false;
        HideIndicator();
    }

    private void Attack()
    {
        MoveSmooth(Vector2.zero);
        attackTimer += Time.deltaTime;

        if (!telegraph && attackTimer >= attackInterval - 1f)
        {
            ShowIndicator();
            anim?.SetTrigger(ParamAttack); // 인디케이터와 함께 모션 시작
            telegraph = true;
        }

        if (attackTimer >= attackInterval)
        {
            DamagePlayer(statManager.getStat(StatType.DAMAGE).calibratedValue);
            ApplyBleed();
            attackTimer = 0f;
            telegraph   = false;
            HideIndicator();
        }
    }

    private void ApplyBleed()
    {
        Entity target = player?.GetComponent<PlayerEffectTemp>();
        if (target == null) return;

        if (target.HasEffect<BleedEffect>(out BleedEffect existing))
            existing.AddStacks(bleedStacks, bleedDuration);
        else
            target.ApplyEffect(new BleedEffect(bleedStacks, bleedDotDamage, bleedDuration));
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

    // 적 자식으로 부착 — 적을 중심으로 attackRange 크기의 원형 표시
    private void ShowIndicator()
    {
        if (attackIndicatorPrefab == null) return;
        if (indicator == null)
        {
            indicator = Instantiate(attackIndicatorPrefab, transform);
            indicator.transform.localPosition = Vector3.zero;
            indicator.transform.localScale    = Vector3.one * attackRange * 2f;
        }
        indicator.SetActive(true);
    }

    private void HideIndicator()
    {
        if (indicator != null) indicator.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}