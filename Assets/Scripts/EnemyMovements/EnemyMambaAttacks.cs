using UnityEngine;

public partial class EnemyMamba
{
    private GameObject biteIndicator;
    private GameObject spitIndicator;

    internal void UpdateBite(float dist)
    {
        if (dist > biteRange) { HideBiteIndicator(); state = State.Chase; return; }

        MoveSmooth(Vector2.zero);
        biteTimer += Time.deltaTime;

        if (biteTimer >= biteInterval - 0.4f) ShowBiteIndicator();

        if (biteTimer >= biteInterval)
        {
            anim?.SetTrigger(ParamBite);
            DamagePlayer(statManager.getStat(StatType.DAMAGE).calibratedValue);
            ApplyPoisonToPlayer();
            biteTimer = 0f;
            HideBiteIndicator();
        }
    }

    internal void EnterSpitTelegraph()
    {
        state = State.SpitTelegraph; patternTimer = 0f; spitTimer = 0f;
        MoveSmooth(Vector2.zero);
        anim?.SetTrigger(ParamSpit);
        ShowSpitIndicator();
    }

    internal void UpdateSpitTelegraph()
    {
        MoveSmooth(Vector2.zero);
        patternTimer += Time.deltaTime;
        UpdateSpitIndicatorTransform();

        if (patternTimer >= spitTelegraph)
        {
            HideSpitIndicator();
            state = State.Spit; patternTimer = 0f;
        }
    }

    internal void FireSpitProjectile()
    {
        if (spitProjectilePrefab == null) return;
        Vector2 dir = (player.position - transform.position).normalized;
        Instantiate(spitProjectilePrefab, transform.position, Quaternion.identity)
            .GetComponent<PoisonProjectile>()?.Init(dir, spitZoneRadius, spitZoneDuration, this);
    }

    internal void ApplyPoisonToPlayer()
    {
        if (player == null) return;

        Entity target = player.GetComponent<Entity>();
        if (target == null) return;

        if (target.HasEffect<PoisonEffect>(out PoisonEffect existing))
        {
            existing.Refresh(poisonDuration);
        }
        else
        {
            target.ApplyEffect(new PoisonEffect(poisonPercent, poisonTickInterval, poisonDuration));
        }
    }

    private void ShowBiteIndicator()
    {
        if (biteIndicatorPrefab == null) return;
        if (biteIndicator == null)
        {
            biteIndicator = Instantiate(biteIndicatorPrefab, transform);
            biteIndicator.transform.localPosition = Vector3.zero;
            biteIndicator.transform.localScale    = Vector3.one * biteRange * 2f;
        }
        biteIndicator.SetActive(true);
    }

    internal void HideBiteIndicator() { if (biteIndicator != null) biteIndicator.SetActive(false); }

    private void ShowSpitIndicator()
    {
        if (spitIndicatorPrefab == null) return;
        spitIndicator ??= Instantiate(spitIndicatorPrefab);
        spitIndicator.SetActive(true);
        UpdateSpitIndicatorTransform();
    }

    internal void HideSpitIndicator() { if (spitIndicator != null) spitIndicator.SetActive(false); }

    private void UpdateSpitIndicatorTransform()
    {
        if (spitIndicator == null || player == null) return;
        Vector2 dir    = (player.position - transform.position).normalized;
        float   dist   = Vector2.Distance(transform.position, player.position);
        Vector2 center = (Vector2)transform.position + dir * (dist * 0.5f);
        spitIndicator.transform.SetPositionAndRotation(
            center, Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg));
        spitIndicator.transform.localScale = new Vector3(dist, spitIndicator.transform.localScale.y, 1f);
    }
}