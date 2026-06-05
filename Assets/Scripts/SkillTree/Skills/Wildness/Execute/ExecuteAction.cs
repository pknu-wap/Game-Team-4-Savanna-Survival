using UnityEngine;

[CreateAssetMenu(menuName = "SkillTree/Actions/ExecuteAction")]
public class ExecuteAction : ActiveAction
{
    [SerializeField] string animTriggerName = "wild_skill";
    [SerializeField] float fanAngle = 60f;
    [SerializeField] float range = 2.25f;
    [SerializeField] float damageMultiplier = 1.0f;
    [SerializeField] float executeThreshold = 0.3f;
    [SerializeField] float executeDamageMultiplier = 2.0f;
    [SerializeField] GameObject vfxPrefab;
    [SerializeField] GameObject executeVfxPrefab;
    [SerializeField] float vfxDuration = 0.2f;

    public override void Process(GameObject player, ActiveSkillData data)
    {
        var animator = player.GetComponent<Animator>();
        if (animator != null && !string.IsNullOrEmpty(animTriggerName))
            animator.SetTrigger(animTriggerName);

        float baseDmg = 0f;
        var statManager = player.GetComponent<PlayerStatManager>();
        if (statManager != null)
            baseDmg = statManager.StatCore.getStat(StatType.SKILL_DAMAGE).calibratedValue * damageMultiplier;

        var bossCtrl = player.GetComponent<BossWildController>();
        bool isBoss  = bossCtrl != null;
        int  mask    = isBoss ? (int)bossCtrl.TargetLayer : LayerMask.GetMask("Enemy");

        var sr = player.GetComponentInChildren<SpriteRenderer>();
        Vector2 forward = (sr != null && sr.flipX) ? Vector2.right : Vector2.left;

        // ✅ 레이어 마스크 적용
        var hits = Physics2D.OverlapCircleAll(player.transform.position, range, mask);

        Collider2D nearest     = null;
        float      nearestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit.gameObject == player) continue;

            Vector2 dir = ((Vector2)hit.transform.position - (Vector2)player.transform.position).normalized;
            if (Vector2.Angle(forward, dir) > fanAngle / 2f) continue;

            float dist = Vector2.Distance(player.transform.position, hit.transform.position);
            if (dist < nearestDist) { nearestDist = dist; nearest = hit; }
        }

        if (nearest == null) return;

        if (vfxPrefab != null)
        {
            var vfx = Instantiate(vfxPrefab, player.transform.position, player.transform.rotation);
            Object.Destroy(vfx, vfxDuration);
        }

        if (isBoss)
        {
            // ✅ 보스 — 처형 조건 없이 배율만 적용 (플레이어 HP 비율 체크)
            var playerStat = nearest.GetComponent<PlayerStatManager>();
            float hpRatio  = 1f;
            if (playerStat != null)
            {
                float cur = playerStat.StatCore.getStat(StatType.HEALTH).rawValue;
                float max = playerStat.StatCore.getStat(StatType.MAX_HEALTH).rawValue;
                hpRatio = max > 0f ? cur / max : 1f;
            }

            if (hpRatio <= executeThreshold)
            {
                bossCtrl.DamagePlayer(nearest, baseDmg * executeDamageMultiplier);
                if (executeVfxPrefab != null)
                    Instantiate(executeVfxPrefab, nearest.transform.position, Quaternion.identity);
                Debug.Log($"[Execute-Boss] 처형 발동 — damage={baseDmg * executeDamageMultiplier:F1}");
            }
            else
            {
                bossCtrl.DamagePlayer(nearest, baseDmg);
                Debug.Log($"[Execute-Boss] 발동 — damage={baseDmg:F1}");
            }
        }
        else
        {
            var enemy    = nearest.GetComponent<Enemy>();
            if (enemy == null) return;

            float hpRatio = enemy.MaxHp > 0f ? enemy.CurrentHp / enemy.MaxHp : 1f;
            if (hpRatio <= executeThreshold)
            {
                enemy.TakeDamage(baseDmg * executeDamageMultiplier);
                if (executeVfxPrefab != null)
                    Instantiate(executeVfxPrefab, enemy.transform.position, Quaternion.identity);
                Debug.Log($"[Execute] 처형 발동 — damage={baseDmg * executeDamageMultiplier:F1}, hpRatio={hpRatio:P0}");
            }
            else
            {
                enemy.TakeDamage(baseDmg);
                Debug.Log($"[Execute] 발동 — damage={baseDmg:F1}, hpRatio={hpRatio:P0}");
            }
        }

#if UNITY_EDITOR
        DrawDebugArc(player.transform.position, forward, range, fanAngle);
#endif
    }

#if UNITY_EDITOR
    void DrawDebugArc(Vector2 origin, Vector2 forward, float radius, float angle)
    {
        int segments = 20;
        float halfAngle  = angle / 2f;
        float startAngle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg - halfAngle;
        Vector2 prevPoint = origin + (Vector2)(Quaternion.Euler(0, 0, startAngle) * Vector2.right) * radius;
        Debug.DrawLine(origin, prevPoint, Color.magenta, 0.3f);
        for (int i = 1; i <= segments; i++)
        {
            float a = startAngle + (angle / segments) * i;
            Vector2 nextPoint = origin + (Vector2)(Quaternion.Euler(0, 0, a) * Vector2.right) * radius;
            Debug.DrawLine(prevPoint, nextPoint, Color.magenta, 0.3f);
            prevPoint = nextPoint;
        }
        Debug.DrawLine(origin, prevPoint, Color.magenta, 0.3f);
    }
#endif
}
