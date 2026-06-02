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

#if UNITY_EDITOR
    void DrawDebugArc(Vector2 origin, Vector2 forward, float radius, float angle)
    {
        int segments = 20;
        float halfAngle = angle / 2f;
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

    public override void Process(GameObject player, ActiveSkillData data)
    {
        var animator = player.GetComponent<Animator>();
        if (animator != null && !string.IsNullOrEmpty(animTriggerName))
            animator.SetTrigger(animTriggerName);

        float baseDmg = 0f;
        var statManager = player.GetComponent<PlayerStatManager>();
        if (statManager != null)
            baseDmg = statManager.StatCore.getStat(StatType.SKILL_DAMAGE).calibratedValue * damageMultiplier;

        var hits = Physics2D.OverlapCircleAll(player.transform.position, range);
        var sr = player.GetComponentInChildren<SpriteRenderer>();
        Vector2 forward = (sr != null && sr.flipX) ? Vector2.right : Vector2.left;

        Enemy nearest = null;
        float nearestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit.gameObject == player) continue;
            var enemy = hit.GetComponent<Enemy>();
            if (enemy == null) continue;

            Vector2 dir = ((Vector2)hit.transform.position - (Vector2)player.transform.position).normalized;
            if (Vector2.Angle(forward, dir) > fanAngle / 2f) continue;

            float dist = Vector2.Distance(player.transform.position, hit.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = enemy;
            }
        }

        if (nearest == null) return;

        if (vfxPrefab != null)
        {
            var vfx = Instantiate(vfxPrefab, player.transform.position, player.transform.rotation);
            Object.Destroy(vfx, vfxDuration);
        }

        float hpRatio = nearest.MaxHp > 0f ? nearest.CurrentHp / nearest.MaxHp : 1f;

        if (hpRatio <= executeThreshold)
        {
            nearest.TakeDamage(baseDmg * executeDamageMultiplier);
            if (executeVfxPrefab != null)
                Instantiate(executeVfxPrefab, nearest.transform.position, Quaternion.identity);
            Debug.Log($"[Execute] 처형 발동 — damage={baseDmg * executeDamageMultiplier:F1}, hpRatio={hpRatio:P0}");
        }
        else
        {
            nearest.TakeDamage(baseDmg);
            Debug.Log($"[Execute] 발동 — damage={baseDmg:F1}, hpRatio={hpRatio:P0}");
        }

#if UNITY_EDITOR
        DrawDebugArc(player.transform.position, forward, range, fanAngle);
#endif
    }
}
