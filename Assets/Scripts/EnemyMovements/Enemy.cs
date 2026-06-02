using System.Collections;
using UnityEngine;

public abstract class Enemy : Entity
{
    [Header("Drop")]
    [SerializeField] protected DropTable dropTable;

    [Header("Contact Damage")]
    [SerializeField] private float contactDamage   = 5f;
    [SerializeField] private float contactCooldown = 1f;

    [Header("Kill Reward")]
    [SerializeField] private float hungerReward = 10f;

    // ── 공통 컴포넌트 ──────────────────────────────────────────
    protected Rigidbody2D    rb;
    protected Transform      player;
    protected PlayerStatCore playerStatCore;

    protected EnemyStatManager statManager;
    protected float            currentHp;

    public float CurrentHp => currentHp;
    public float MaxHp     => statManager.getStat(StatType.HEALTH).rawValue;

    private float contactTimer;

    // ── 공통 배회(Wander) 상태 ─────────────────────────────────
    protected Vector2 velocity;
    protected Vector2 wanderTarget;
    protected bool    isIdle;
    protected float   idleTimer;

    // 스폰 위치를 홈으로 고정하고 반경 내에서만 배회
    protected Vector2 wanderHome;

    [Header("Wander")]
    [SerializeField] protected float wanderRadius   = 3f;
    [SerializeField] protected float arriveDistance = 0.2f;
    [SerializeField] protected float idleChance     = 0.2f;
    [SerializeField] protected float moveSpeed      = 3f;

    protected bool isDead;
    public bool IsDead => isDead;

    protected virtual void Awake()
    {
        rb          = GetComponent<Rigidbody2D>();
        statManager = new EnemyStatManager();
        wanderHome  = transform.position;
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
        if (!isDead) Move();
    }

    protected virtual void Update()
    {
        if (contactTimer < contactCooldown)
            contactTimer += Time.deltaTime;
    }

    protected abstract void Move();
    protected abstract bool IsPlayerInDetection();

    // ── 이동 헬퍼 ─────────────────────────────────────────────

    protected void MoveSmooth(Vector2 targetVel)
    {
        velocity          = Vector2.Lerp(velocity, targetVel, Time.deltaTime * 10f);
        rb.linearVelocity = velocity;
    }

    protected void Wander()
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
            if (Random.value < idleChance) { isIdle = true; return; }
            SetNewWanderTarget();
        }

        MoveSmooth((wanderTarget - (Vector2)transform.position).normalized * moveSpeed * 0.7f);
    }

    protected void SetNewWanderTarget()
    {
        // 현재 위치가 아닌 홈 기준으로 목표를 잡아 한쪽으로 쏠리는 현상을 방지
        wanderTarget = wanderHome + Random.insideUnitCircle * wanderRadius;
    }

    // ── 플레이어 상호작용 ──────────────────────────────────────

    protected void DamagePlayer(float damage)
    {
        if (playerStatCore == null) return;
        float next = Mathf.Max(0f, playerStatCore.getStat(StatType.HEALTH).rawValue - damage);
        playerStatCore.registerStat(StatType.HEALTH, next);
    }

    // ── 피격 / 사망 ───────────────────────────────────────────

    protected bool isDead;

    public override void TakeDamage(float damage)
    {
        if (isDead) return;

        float maxHp = statManager.getStat(StatType.HEALTH).rawValue;

        // 데미지 이벤트 발행 — 패시브 스킬이 e.setDamage()로 최종값 수정 가능
        EnemyDamageEvent dmgEvent = new EnemyDamageEvent(
            victim:     this,
            attacker:   player?.GetComponent<Entity>(),
            damage:     damage,
            currentHp:  currentHp,
            maxHp:      maxHp,
            position:   transform.position
        );
        EnemyEvents.PublishDamage(dmgEvent);

        currentHp -= dmgEvent.getDamage();
        if (currentHp <= 0) Die();
    }

    // velocity 필드를 직접 세팅해 MoveSmooth 자연 감쇠로 넉백 표현
    public virtual void ApplyKnockback(Vector2 force)
    {
        velocity = force;
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.linearVelocity = Vector2.zero;
        ApplyDeathRewards();

        // 사망 이벤트 발행
        EnemyDeathEvent deathEvent = new EnemyDeathEvent(
            victim:   this,
            killer:   player?.GetComponent<Entity>(),
            position: transform.position,
            maxHp:    statManager.getStat(StatType.HEALTH).rawValue
        );
        EnemyEvents.PublishDeath(deathEvent);

        StartCoroutine(DieRoutine(0f));
    }

    protected void ApplyDeathRewards()
    {
        AddPlayerHunger(hungerReward);
        dropTable?.Drop(transform.position);
    }

    protected IEnumerator DieRoutine(float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    // Animator에서 사망 클립 길이를 읽는 헬퍼/클립이 없으면 0 반환
    // clipName을 생략하면 "Die"를 기본값으로 사용
    protected float GetDieClipLength(Animator animator, string clipName = "Die")
    {
        if (animator == null) return 0f;
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            if (clip.name == clipName) return clip.length;
        return 0f;
    }

    // ── 접촉 데미지 ───────────────────────────────────────────

    private void OnCollisionEnter2D(Collision2D collision) => TryApplyContactDamage(collision.gameObject);

    private void TryApplyContactDamage(GameObject other)
    {
        if (isDead) return;
        if (!other.CompareTag("Player") || contactTimer < contactCooldown) return;
        DamagePlayer(contactDamage);
        contactTimer = 0f;
    }

    // ── 내부 유틸리티 ─────────────────────────────────────────────

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