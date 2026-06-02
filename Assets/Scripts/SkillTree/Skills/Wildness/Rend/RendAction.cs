using UnityEngine;

[CreateAssetMenu(menuName = "SkillTree/Actions/RendAction")]
public class RendAction : ActiveAction
{
    [SerializeField] string animTriggerName = "wild_skill";
    [SerializeField] float fanAngle = 120f;  // ScratchAction.fanAngle × 2
    [SerializeField] float range = 3f;       // ScratchAction.range × 2
    [SerializeField] GameObject vfxPrefab;
    [SerializeField] float damageMultiplier = 1.0f;
    [SerializeField] float vfxDuration = 0.2f;

#if UNITY_EDITOR
    void DrawDebugArc(Vector2 origin, Vector2 forward, float radius, float angle)
    {
        int segments = 20;
        float halfAngle = angle / 2f;
        float startAngle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg - halfAngle;

        Vector2 prevPoint = origin + (Vector2)(Quaternion.Euler(0, 0, startAngle) * Vector2.right) * radius;
        Debug.DrawLine(origin, prevPoint, Color.yellow, 0.3f);

        for (int i = 1; i <= segments; i++)
        {
            float a = startAngle + (angle / segments) * i;
            Vector2 nextPoint = origin + (Vector2)(Quaternion.Euler(0, 0, a) * Vector2.right) * radius;
            Debug.DrawLine(prevPoint, nextPoint, Color.yellow, 0.3f);
            prevPoint = nextPoint;
        }

        Debug.DrawLine(origin, prevPoint, Color.yellow, 0.3f);
    }
#endif

    public override void Process(GameObject player, ActiveSkillData data)
    {
        var animator = player.GetComponent<Animator>();
        if (animator != null && !string.IsNullOrEmpty(animTriggerName))
            animator.SetTrigger(animTriggerName);

        float damage = 0f;
        var statManager = player.GetComponent<PlayerStatManager>();
        if (statManager != null)
            damage = statManager.StatCore.getStat(StatType.SKILL_DAMAGE).calibratedValue * damageMultiplier;

        var hits = Physics2D.OverlapCircleAll(player.transform.position, range);
        var sr = player.GetComponentInChildren<SpriteRenderer>();
        Vector2 forward = (sr != null && sr.flipX) ? Vector2.right : Vector2.left;

        int hitCount = 0;

        foreach (var hit in hits)
        {
            if (hit.gameObject == player) continue;
            var enemy = hit.GetComponent<Enemy>();
            if (enemy == null) continue;

            Vector2 dir = ((Vector2)hit.transform.position - (Vector2)player.transform.position).normalized;
            if (Vector2.Angle(forward, dir) > fanAngle / 2f) continue;

            enemy.TakeDamage(damage);
            hitCount++;

            if (vfxPrefab != null)
            {
                var vfx = Instantiate(vfxPrefab, hit.transform.position, player.transform.rotation);
                Object.Destroy(vfx, vfxDuration);
            }
        }

#if UNITY_EDITOR
        DrawDebugArc(player.transform.position, forward, range, fanAngle);
#endif
        Debug.Log($"[Rend] 발동 — damage={damage:F1}, 적중 {hitCount}명");
    }
}
