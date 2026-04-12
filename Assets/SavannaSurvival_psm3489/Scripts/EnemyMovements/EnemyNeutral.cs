using UnityEngine;

public class EnemyNeutral : Enemy
{
    [Header("Config")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float maxHp     = 200f;
    [SerializeField] private float damage    = 25f;

    [SerializeField] private float detectionRange = 8f;

    [Header("Melee (근거리)")]
    [SerializeField] private float meleeRange    = 2f;
    [SerializeField] private float meleeInterval = 1.5f;
    [SerializeField] private GameObject indicatorPrefab;

    [Header("AoE (광역)")]
    [SerializeField] private float aoeRange     = 4f;
    [SerializeField] private float aoeInterval  = 8f;
    [SerializeField] private float aoeTelegraph = 2.5f;
    [SerializeField] private float aoeDamage    = 40f;

    [Header("Water (원거리 물 분사)")]
    [SerializeField] private float waterRange     = 7f;
    [SerializeField] private float waterDuration  = 3f;
    [SerializeField] private float waterCooldown  = 5f;
    [SerializeField] private float waterTelegraph = 0.8f;
    [SerializeField] private float waterDamage    = 8f;
    [SerializeField] private float waterWidth     = 1.5f;
    [SerializeField] private GameObject waterZonePrefab;
    // 인디케이터는 indicatorPrefab 하나를 공용으로 사용

    [Header("Wander")]
    [SerializeField] private float wanderRadius   = 3f;
    [SerializeField] private float arriveDistance = 0.2f;
    [SerializeField] private float idleChance     = 0.2f;

    // 한 번 true가 되면 절대 false로 돌아오지 않음
    private bool isHostile = false;

    private enum AttackState
    {
        Ready,
        AoeWaiting,
        AoeTelegraph,
        Aoe,
        Water,
        Melee
    }
    private AttackState state = AttackState.Ready;

    private float meleeTimer;
    private float aoeTimer;
    private float waterTimer;
    private float patternTimer;

    private Vector2 velocity;
    private Vector2 wanderTarget;
    private bool    isIdle;
    private float   idleTimer;

    private GameObject meleeIndicator;
    private GameObject aoeIndicator;
    private GameObject waterIndicator;

    private bool waterZoneSpawned = false;

    protected override void Awake()
    {
        base.Awake();

        statManager.InitAttacker(maxHp, damage);
        currentHp = statManager.getStat(StatType.HEALTH).rawValue;

        SetNewWanderTarget();
        aoeTimer = aoeInterval * 0.5f;
    }

    public override void TakeDamage(float dmg)
    {
        isHostile = true;
        base.TakeDamage(dmg);
    }

    protected override bool IsPlayerInDetection() => isHostile;

    protected override void Move()
    {
        if (player == null || !player.gameObject.activeInHierarchy)
        {
            Wander();
            return;
        }

        if (!isHostile)
        {
            Wander();
            return;
        }

        aoeTimer   += Time.deltaTime;
        waterTimer += Time.deltaTime;

        float dist = Vector2.Distance(transform.position, player.position);

        switch (state)
        {
            case AttackState.Ready:        UpdateReady(dist);        break;
            case AttackState.AoeWaiting:   UpdateAoeWaiting(dist);   break;
            case AttackState.AoeTelegraph: UpdateAoeTelegraph();     break;
            case AttackState.Aoe:          UpdateAoe(dist);          break;
            case AttackState.Water:        UpdateWater();            break;
            case AttackState.Melee:        UpdateMelee(dist);        break;
        }
    }

    private void UpdateReady(float dist)
    {
        if (aoeTimer >= aoeInterval)        { EnterAoeWaiting(); return; }
        if (dist >= waterRange && waterTimer >= waterCooldown) { EnterWater(); return; }
        if (dist <= meleeRange)             { state = AttackState.Melee; return; }
        if (dist <= detectionRange)         Chase();
        else                                Wander();
    }

    private void EnterAoeWaiting()
    {
        state    = AttackState.AoeWaiting;
        aoeTimer = 0f;
    }

    private void UpdateAoeWaiting(float dist)
    {
        if (dist <= aoeRange) { EnterAoeTelegraph(); return; }
        Chase();
    }

    private void EnterAoeTelegraph()
    {
        state        = AttackState.AoeTelegraph;
        patternTimer = 0f;
        MoveSmooth(Vector2.zero);
        ShowAoeIndicator();
    }

    private void UpdateAoeTelegraph()
    {
        MoveSmooth(Vector2.zero);
        patternTimer += Time.deltaTime;

        if (patternTimer >= aoeTelegraph)
        {
            HideAoeIndicator();
            state        = AttackState.Aoe;
            patternTimer = 0f;
        }
    }

    private void UpdateAoe(float dist)
    {
        if (dist <= aoeRange)
            player.GetComponent<PlayerDummy>()?.TakeDamage(aoeDamage);

        state = AttackState.Ready;
    }

    private void EnterWater()
    {
        state            = AttackState.Water;
        patternTimer     = 0f;
        waterTimer       = 0f;
        waterZoneSpawned = false;
        MoveSmooth(Vector2.zero);
        ShowWaterIndicator();
    }

    private void UpdateWater()
    {
        MoveSmooth(Vector2.zero);
        patternTimer += Time.deltaTime;

        if (patternTimer < waterTelegraph)
        {
            UpdateWaterIndicatorTransform();
            return;
        }

        if (!waterZoneSpawned)
        {
            HideWaterIndicator();
            SpawnWaterZone();
            waterZoneSpawned = true;
        }

        if (patternTimer >= waterTelegraph + waterDuration)
        {
            waterZoneSpawned = false;
            state            = AttackState.Ready;
        }
    }

    private void SpawnWaterZone()
    {
        if (waterZonePrefab == null) return;

        Vector2 dir    = (player.position - transform.position).normalized;
        Vector2 center = (Vector2)transform.position + dir * (waterRange * 0.5f);
        float   angle  = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject go   = Instantiate(waterZonePrefab, center, Quaternion.Euler(0f, 0f, angle));
        WaterZone  zone = go.GetComponent<WaterZone>();
        if (zone != null)
            zone.Init(waterDamage, waterDuration, waterRange, waterWidth);
    }

    private void UpdateMelee(float dist)
    {
        if (dist > meleeRange)
        {
            meleeTimer = 0f;
            HideMeleeIndicator();
            state = AttackState.Ready;
            return;
        }

        MoveSmooth(Vector2.zero);
        meleeTimer += Time.deltaTime;

        if (meleeTimer >= meleeInterval - 0.5f)
            ShowMeleeIndicator();

        if (meleeTimer >= meleeInterval)
        {
            float dmg = statManager.getStat(StatType.DAMAGE).calibratedValue;
            player.GetComponent<PlayerDummy>()?.TakeDamage(dmg);

            meleeTimer = 0f;
            HideMeleeIndicator();

            if (aoeTimer >= aoeInterval) { EnterAoeWaiting(); return; }
            state = AttackState.Ready;
        }
    }

    private void Chase()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        MoveSmooth(dir * moveSpeed);
    }

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
            if (Random.value < idleChance) { isIdle = true; return; }
            SetNewWanderTarget();
        }

        Vector2 dir = (wanderTarget - (Vector2)transform.position).normalized;
        MoveSmooth(dir * moveSpeed * 0.7f);
    }

    private void SetNewWanderTarget()
    {
        wanderTarget = (Vector2)transform.position + Random.insideUnitCircle * wanderRadius;
    }

    private void MoveSmooth(Vector2 targetVel)
    {
        velocity = Vector2.Lerp(velocity, targetVel, Time.deltaTime * 10f);
        rb.linearVelocity = velocity;
    }

    private void ShowMeleeIndicator()
    {
        if (indicatorPrefab == null) return;
        if (meleeIndicator == null)
            meleeIndicator = Instantiate(indicatorPrefab, transform);
        meleeIndicator.transform.localPosition = Vector3.zero;
        meleeIndicator.transform.localScale    = Vector3.one * meleeRange * 2f;
        meleeIndicator.SetActive(true);
    }
    private void HideMeleeIndicator()
    {
        if (meleeIndicator != null) meleeIndicator.SetActive(false);
    }

    private void ShowAoeIndicator()
    {
        if (indicatorPrefab == null) return;
        if (aoeIndicator == null)
            aoeIndicator = Instantiate(indicatorPrefab, transform);
        aoeIndicator.transform.localPosition = Vector3.zero;
        aoeIndicator.transform.localScale    = Vector3.one * aoeRange * 2f;
        aoeIndicator.SetActive(true);
    }
    private void HideAoeIndicator()
    {
        if (aoeIndicator != null) aoeIndicator.SetActive(false);
    }

    private void ShowWaterIndicator()
    {
        if (indicatorPrefab == null) return;
        if (waterIndicator == null)
            waterIndicator = Instantiate(indicatorPrefab);
        waterIndicator.SetActive(true);
        UpdateWaterIndicatorTransform();
    }
    private void HideWaterIndicator()
    {
        if (waterIndicator != null) waterIndicator.SetActive(false);
    }
    private void UpdateWaterIndicatorTransform()
    {
        if (waterIndicator == null || player == null) return;
        Vector2 dir    = (player.position - transform.position).normalized;
        Vector2 center = (Vector2)transform.position + dir * (waterRange * 0.5f);
        float   angle  = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        waterIndicator.transform.position   = center;
        waterIndicator.transform.rotation   = Quaternion.Euler(0f, 0f, angle);
        waterIndicator.transform.localScale = new Vector3(waterRange, waterWidth, 1f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, aoeRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, waterRange);
    }
}