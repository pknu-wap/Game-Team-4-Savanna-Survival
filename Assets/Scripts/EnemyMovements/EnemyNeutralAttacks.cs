using UnityEngine;

public partial class EnemyNeutral
{
    private GameObject meleeIndicator;
    private GameObject aoeIndicator;
    private GameObject waterIndicator;
    private bool       waterZoneSpawned;

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
        anim?.SetBool(ParamIsAoeTelegraph, true);
        ShowAoeIndicator();
    }

    private void UpdateAoeTelegraph()
    {
        MoveSmooth(Vector2.zero);
        patternTimer += Time.deltaTime;
        if (patternTimer >= aoeTelegraph)
        {
            anim?.SetBool(ParamIsAoeTelegraph, false);
            HideAoeIndicator();
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
        MoveSmooth(Vector2.zero);
        anim?.SetBool(ParamIsWater, true);
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
            DestroyWaterIndicator();
            SpawnWaterZone();
            waterZoneSpawned = true;
        }

        if (patternTimer >= waterTelegraph + waterDuration)
        {
            anim?.SetBool(ParamIsWater, false);
            waterZoneSpawned = false;
            state            = AttackState.Ready;
        }
    }

    private void SpawnWaterZone()
    {
        if (waterZonePrefab == null) return;
        Vector2    dir    = (player.position - transform.position).normalized;
        Vector2    origin = (Vector2)transform.position + dir * waterSpawnOffset;
        Vector2    center = origin + dir * (waterRange * 0.5f);
        GameObject go     = Instantiate(waterZonePrefab, center, Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg));
        go.GetComponent<WaterZone>()?.Init(waterDamage, waterDuration, waterRange, waterWidth);
    }

    // ── Melee ────────────────────────────────────────────────

    private void UpdateMelee(float dist)
    {
        if (dist > meleeRange)
        {
            meleeTimer = 0f;
            anim?.SetBool(ParamIsMelee, false);
            HideMeleeIndicator();
            state = AttackState.Ready;
            return;
        }

        MoveSmooth(Vector2.zero);
        meleeTimer += Time.deltaTime;

        if (meleeTimer >= meleeInterval - 0.5f)
        {
            anim?.SetBool(ParamIsMelee, true);
            ShowMeleeIndicator();
        }

        if (meleeTimer >= meleeInterval)
        {
            DamagePlayer(statManager.getStat(StatType.DAMAGE).calibratedValue);
            meleeTimer = 0f;
            anim?.SetBool(ParamIsMelee, false);
            HideMeleeIndicator();
            if (aoeTimer >= aoeInterval) { EnterAoeWaiting(); return; }
            state = AttackState.Ready;
        }
    }

    // ── 인디케이터 ────────────────────────────────────────────

    private void SetWorldScale(GameObject go, float diameter)
    {
        var sr = go.GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null) return;
        go.transform.localScale = Vector3.one;
        float current = sr.bounds.size.x;
        if (current <= 0f) return;
        float ratio = diameter / current;
        go.transform.localScale = new Vector3(ratio, ratio, 1f);
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
        Vector2 origin = (Vector2)transform.position + dir * waterSpawnOffset;
        float   dist   = Vector2.Distance(origin, player.position);
        Vector2 center = origin + dir * (dist * 0.5f);
        waterIndicator.transform.SetPositionAndRotation(center, Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg));
        waterIndicator.transform.localScale = new Vector3(dist, waterWidth, 1f);
    }
}