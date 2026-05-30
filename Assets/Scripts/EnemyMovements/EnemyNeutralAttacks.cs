using UnityEngine;

public partial class EnemyNeutral
{
    private GameObject meleeIndicator;
    private GameObject aoeIndicator;
    private GameObject waterIndicator;
    private bool       waterZoneSpawned;
    private WaterZone  activeWaterZone;

    // ── AoE ──────────────────────────────────────────────────

    private void EnterAoeWaiting()
    {
        state    = AttackState.AoeWaiting;
        aoeTimer = 0f;
    }

    private void UpdateAoeWaiting(float dist)
    {
        if (dist <= aoeRange) EnterAoeTelegraph();
        else MoveSmooth((player.position - transform.position).normalized * moveSpeed);
    }

    private void EnterAoeTelegraph()
    {
        state        = AttackState.AoeTelegraph;
        patternTimer = 0f;
        MoveSmooth(Vector2.zero);
        ShowAoeIndicator();
        anim?.SetBool(ParamIsAoe, true);  // 예고 시작 — 모션 유지
    }

    private void UpdateAoeTelegraph()
    {
        MoveSmooth(Vector2.zero);
        patternTimer += Time.deltaTime;
        if (patternTimer >= aoeTelegraph)
        {
            HideAoeIndicator();
            anim?.SetBool(ParamIsAoe, false); // 예고 종료
            state        = AttackState.Aoe;
            patternTimer = 0f;
        }
    }

    private void UpdateAoe(float dist)
    {
        if (dist <= aoeRange) DamagePlayer(aoeDamage);
        state = AttackState.Ready;
    }

    // ── Water ────────────────────────────────────────────────

    private void EnterWater()
    {
        state            = AttackState.Water;
        patternTimer     = 0f;
        waterTimer       = 0f;
        waterZoneSpawned = false;
        activeWaterZone  = null;
        MoveSmooth(Vector2.zero);
        ShowWaterIndicator();
        anim?.SetBool(ParamIsWater, true); // 예고 시작 — 모션 유지
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
            DestroyWaterIndicator();
            SpawnWaterZone();
            waterZoneSpawned = true;
        }

        // 워터존 소멸 감지 → 마지막 모션 해제 후 Ready
        if (activeWaterZone == null)
        {
            anim?.SetBool(ParamIsWater, false);
            waterZoneSpawned = false;
            state            = AttackState.Ready;
        }
    }

    private void SpawnWaterZone()
    {
        if (waterZonePrefab == null) return;
        Vector2 dir    = (player.position - transform.position).normalized;
        // 발사 기준점: 몸 중앙 + 로컬 오프셋(코 끝 등). 좌우 방향에 따라 X 오프셋 반전.
        float   flipX  = transform.localScale.x > 0 ? 1f : -1f;
        Vector2 origin = (Vector2)transform.position
                       + new Vector2(waterOriginOffset.x * flipX, waterOriginOffset.y);
        Vector2 center = origin + dir * (waterRange * 0.5f);
        GameObject go  = Instantiate(waterZonePrefab, center, Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg));
        activeWaterZone = go.GetComponent<WaterZone>();
        activeWaterZone?.Init(waterDamage, waterDuration, waterRange, waterWidth);
    }

    // ── Melee ────────────────────────────────────────────────

    private void UpdateMelee(float dist)
    {
        if (dist > meleeRange)
        {
            meleeTimer = 0f;
            HideMeleeIndicator();
            anim?.SetBool(ParamIsAttacking, false);
            state = AttackState.Ready;
            return;
        }

        MoveSmooth(Vector2.zero);
        meleeTimer += Time.deltaTime;

        if (meleeTimer >= meleeInterval - 0.5f)
        {
            ShowMeleeIndicator();
            anim?.SetBool(ParamIsAttacking, true); // 인디케이터와 함께 모션 시작
        }

        if (meleeTimer >= meleeInterval)
        {
            DamagePlayer(statManager.getStat(StatType.DAMAGE).calibratedValue);
            meleeTimer = 0f;
            HideMeleeIndicator();
            anim?.SetBool(ParamIsAttacking, false);
            if (aoeTimer >= aoeInterval) { EnterAoeWaiting(); return; }
            state = AttackState.Ready;
        }
    }

    // ── 인디케이터 헬퍼 ──────────────────────────────────────

    private void SetWorldScale(GameObject go, float diameter)
    {
        var sr = go.GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null) return;
        go.transform.localScale = Vector3.one;
        float current = sr.bounds.size.x;
        if (current <= 0f) return;
        go.transform.localScale = new Vector3(diameter / current, diameter / current, 1f);
    }

    private void ShowMeleeIndicator()
    {
        if (indicatorPrefab == null) return;
        if (meleeIndicator == null)
        {
            meleeIndicator = Instantiate(indicatorPrefab, transform);
            meleeIndicator.transform.localPosition = Vector3.zero;
            SetWorldScale(meleeIndicator, meleeRange * 2f);
        }
        meleeIndicator.SetActive(true);
    }
    private void HideMeleeIndicator() { if (meleeIndicator != null) meleeIndicator.SetActive(false); }

    private void ShowAoeIndicator()
    {
        if (indicatorPrefab == null) return;
        if (aoeIndicator == null)
        {
            aoeIndicator = Instantiate(indicatorPrefab, transform);
            aoeIndicator.transform.localPosition = Vector3.zero;
            SetWorldScale(aoeIndicator, aoeRange * 2f);
        }
        aoeIndicator.SetActive(true);
    }
    private void HideAoeIndicator() { if (aoeIndicator != null) aoeIndicator.SetActive(false); }

    private void ShowWaterIndicator()
    {
        if (indicatorPrefab == null) return;
        waterIndicator ??= Instantiate(indicatorPrefab);
        waterIndicator.SetActive(true);
        UpdateWaterIndicatorTransform();
    }

    private void DestroyWaterIndicator()
    {
        if (waterIndicator == null) return;
        Destroy(waterIndicator);
        waterIndicator = null;
    }

    private void UpdateWaterIndicatorTransform()
    {
        if (waterIndicator == null || player == null) return;
        Vector2 dir    = (player.position - transform.position).normalized;
        float   dist   = Vector2.Distance(transform.position, player.position);
        Vector2 center = (Vector2)transform.position + dir * (dist * 0.5f);
        waterIndicator.transform.SetPositionAndRotation(center, Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg));
        waterIndicator.transform.localScale = new Vector3(dist, waterWidth, 1f);
    }
}