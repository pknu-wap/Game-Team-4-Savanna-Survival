using UnityEngine;

public class BabyLionAI : MonoBehaviour
{
    public float attackRange = 0.8f;
    public float chaseRange = 8f;    // 플레이어 기준 적 탐색 반경
    public float leashRange = 6f;    // 플레이어로부터 이탈 허용 최대 거리
    public float moveSpeed = 3f;
    public float attackCooldown = 1f;

    [HideInInspector] public Transform player;
    [HideInInspector] public PlayerStatManager statManager;
    [HideInInspector] public PetController petController;

    private Transform target;
    private SpriteRenderer spriteRenderer;
    
    private Vector2 wanderTarget;
    private float wanderTimer;
    private const float WanderRadius = 2.5f;
    private const float WanderInterval = 2.5f;
private const float SeparationRadius = 1.2f;
    private const float SeparationForce = 4f;


    private float attackTimer;
    private int enemyLayerMask;

private void Start()
    {
        enemyLayerMask = LayerMask.GetMask("Enemy");
        attackTimer = attackCooldown;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

private void Update()
    {
        if (player == null) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);
        if (distToPlayer > leashRange)
        {
            MoveToward(player.position);
            ApplySeparation();
            return;
        }

        FindNearestEnemyToPlayer();

        if (target == null)
        {
            Wander();
            ApplySeparation();
            return;
        }

        float distToTarget = Vector2.Distance(transform.position, target.position);
        if (distToTarget > attackRange)
            MoveToward(target.position);

        ApplySeparation();

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f && distToTarget <= attackRange)
        {
            target.GetComponent<Enemy>()?.TakeDamage(GetCurrentDamage());
            attackTimer = attackCooldown;
        }
    }

    private void FindNearestEnemyToPlayer()
    {
        var hits = Physics2D.OverlapCircleAll(player.position, chaseRange, enemyLayerMask);
        float nearest = float.MaxValue;
        target = null;
        foreach (var h in hits)
        {
            float d = Vector2.Distance(player.position, h.transform.position);
            if (d < nearest) { nearest = d; target = h.transform; }
        }
    }

    private float GetCurrentDamage()
    {
        float skillDamage = statManager != null
            ? statManager.StatCore.getStat(StatType.SKILL_DAMAGE).calibratedValue
            : 1f;
        float multiplier = petController != null ? petController.petDamageMultiplier : 1f;
        return skillDamage * multiplier;
    }

private void MoveToward(Vector3 dest)
    {
        Vector2 dir = ((Vector2)dest - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
        if (spriteRenderer != null)
            spriteRenderer.flipX = dir.x > 0;
    }

private void ApplySeparation()
    {
        if (petController == null) return;
        Vector2 push = Vector2.zero;
        foreach (var pet in petController.activePets)
        {
            if (pet == null || pet == gameObject) continue;
            Vector2 diff = (Vector2)(transform.position - pet.transform.position);
            float dist = diff.magnitude;
            if (dist < SeparationRadius && dist > 0.01f)
                push += diff.normalized * (SeparationRadius - dist);
        }
        if (push != Vector2.zero)
            transform.position += (Vector3)(push * SeparationForce * Time.deltaTime);
    }

private void Wander()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0f || Vector2.Distance(transform.position, wanderTarget) < 0.3f)
        {
            wanderTarget = (Vector2)player.position + Random.insideUnitCircle.normalized * Random.Range(0.8f, WanderRadius);
            wanderTimer = WanderInterval;
        }
        MoveToward(wanderTarget);
    }


}
