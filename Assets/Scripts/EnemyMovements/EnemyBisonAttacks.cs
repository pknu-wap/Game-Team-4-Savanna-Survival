using UnityEngine;

public partial class EnemyBison
{
    private GameObject meleeIndicator;
    private GameObject chargeIndicator;

    // ── Melee ────────────────────────────────────────────────

    private void UpdateMelee(float dist)
    {
        if (chargeTimer >= chargeCooldown && dist <= chargeDetectionRange)
        {
            HideMeleeIndicator();
            EnterChargeTelegraph();
            return;
        }

        if (dist > meleeRange)
        {
            HideMeleeIndicator();
            state = State.Chase;
            return;
        }

        MoveSmooth(Vector2.zero);
        meleeTimer += Time.deltaTime;

        if (meleeTimer >= meleeInterval - 0.5f) ShowMeleeIndicator();

        if (meleeTimer >= meleeInterval)
        {
            anim?.SetTrigger(ParamAttack);
            DamagePlayer(statManager.getStat(StatType.DAMAGE).calibratedValue);
            meleeTimer = 0f;
            HideMeleeIndicator();
        }
    }

    // ── Charge ───────────────────────────────────────────────

    private void EnterChargeTelegraph()
    {
        state        = State.ChargeTelegraph;
        patternTimer = 0f;
        chargeTimer  = 0f;
        chargeDir    = (player.position - transform.position).normalized;
        MoveSmooth(Vector2.zero);
        ShowChargeIndicator();
        anim?.SetTrigger(ParamCharge);
    }

    private void UpdateChargeTelegraph()
    {
        MoveSmooth(Vector2.zero);
        patternTimer += Time.deltaTime;
        UpdateChargeIndicatorTransform();

        if (patternTimer >= chargeTelegraph)
        {
            HideChargeIndicator();
            state             = State.Charging;
            patternTimer      = 0f;
            chargeDamageDealt = false;
        }
    }

    private void UpdateCharging()
    {
        patternTimer      += Time.deltaTime;
        rb.linearVelocity  = chargeDir * chargeSpeed;

        if (patternTimer >= chargeMaxDuration) EnterChargeRecovery();
    }

    private void UpdateChargeRecovery()
    {
        MoveSmooth(Vector2.zero);
        patternTimer += Time.deltaTime;
        if (patternTimer >= chargeRecoveryTime) state = State.Chase;
    }

    private void EnterChargeRecovery()
    {
        state             = State.ChargeRecovery;
        patternTimer      = 0f;
        rb.linearVelocity = Vector2.zero;
        velocity          = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (state != State.Charging || chargeDamageDealt) return;
        if (!col.gameObject.CompareTag("Player")) return;
        chargeDamageDealt = true;
        DamagePlayer(chargeDamage);
        EnterChargeRecovery();
    }

    // ── 인디케이터 ────────────────────────────────────────────

    private void ShowMeleeIndicator()
    {
        if (meleeIndicatorPrefab == null) return;
        if (meleeIndicator == null)
        {
            meleeIndicator = Instantiate(meleeIndicatorPrefab, transform);
            meleeIndicator.transform.localPosition = Vector3.zero;
            meleeIndicator.transform.localScale    = Vector3.one * meleeRange * 2f;
        }
        meleeIndicator.SetActive(true);
    }

    private void HideMeleeIndicator() { if (meleeIndicator != null) meleeIndicator.SetActive(false); }

    private void ShowChargeIndicator()
    {
        if (chargeIndicatorPrefab == null) return;
        chargeIndicator ??= Instantiate(chargeIndicatorPrefab);
        chargeIndicator.SetActive(true);
        UpdateChargeIndicatorTransform();
    }

    private void HideChargeIndicator() { if (chargeIndicator != null) chargeIndicator.SetActive(false); }

    private void UpdateChargeIndicatorTransform()
    {
        if (chargeIndicator == null || player == null) return;
        Vector2 dir    = (player.position - transform.position).normalized;
        float   dist   = Vector2.Distance(transform.position, player.position);
        Vector2 center = (Vector2)transform.position + dir * (dist * 0.5f);
        chargeIndicator.transform.SetPositionAndRotation(
            center,
            Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg));
        chargeIndicator.transform.localScale = new Vector3(dist, chargeIndicator.transform.localScale.y, 1f);
    }
}