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

    protected Rigidbody2D    rb;
    protected Transform      player;
    protected PlayerStatCore playerStatCore;

    protected EnemyStatManager statManager;
    protected float            currentHp;

    public float CurrentHp => currentHp;
    public float MaxHp     => statManager.getStat(StatType.HEALTH).rawValue;

    private float contactTimer;

    protected Vector2 velocity;
    protected Vector2 wanderTarget;
    protected bool    isIdle;
    protected float   idleTimer;
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
        TickEffects();
    }

    protected abstract void Move();
    protected abstract bool IsPlayerInDetection();

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
        wanderTarget = wanderHome + Random.insideUnitCircle * wanderRadius;
    }

    protected void DamagePlayer(float damage)
    {
        if (playerStatCore == null) return;
        float next = Mathf.Max(0f, playerStatCore.getStat(StatType.HEALTH).rawValue - damage);
        playerStatCore.registerStat(StatType.HEALTH, next);
    }

    // 상태이상 적용을 위한 연결점
    protected void ApplyStatusEffect(PlayerStatCore target /*, StatusEffect effect */)
    {
        // TODO: target에 상태이상(effect)을 적용하는 로직 연결
    }

    public override void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHp -= damage;
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

    protected float GetDieClipLength(Animator animator, string clipName = "Die")
    {
        if (animator == null) return 0f;
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            if (clip.name == clipName) return clip.length;
        return 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision) => TryApplyContactDamage(collision.gameObject);

    private void TryApplyContactDamage(GameObject other)
    {
        if (isDead) return;
        if (!other.CompareTag("Player") || contactTimer < contactCooldown) return;
        DamagePlayer(contactDamage);
        contactTimer = 0f;
    }

    private void AddPlayerHunger(float amount)
    {
        if (playerStatCore == null) return;

        float current   = playerStatCore.getStat(StatType.HUNGER).rawValue;
        float maxHunger = playerStatCore.getStat(StatType.MAX_HUNGER).rawValue;
        float next      = Mathf.Min(current + amount, maxHunger);
        playerStatCore.registerStat(StatType.HUNGER, next);

        Debug.Log($"[적 처치 보상] {gameObject.name} 처치\n 배고픔 +{amount} ({current:F1} → {next:F1} / {maxHunger:F1})");
    }
}