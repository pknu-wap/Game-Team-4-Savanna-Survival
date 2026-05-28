using UnityEngine;

[CreateAssetMenu(menuName = "SkillTree/Actions/ScratchAction")]
public class ScratchAction : ActiveAction
{
    [SerializeField] string animTriggerName = "Scratch";
    [SerializeField] GameObject vfxPrefab;
    [SerializeField] float range = 1.5f;
    [SerializeField] float arcAngle = 60f;
    [SerializeField] float damageMultiplier = 1.0f;
    [SerializeField] float vfxDuration = 0.2f;

#if UNITY_EDITOR
    void DrawDebugArc(Vector2 origin, Vector2 forward, float radius, float angle)
    {
        int segments = 20;
        float halfAngle = angle / 2f;
        float startAngle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg - halfAngle;

        Vector2 prevPoint = origin + (Vector2)(Quaternion.Euler(0, 0, startAngle) * Vector2.right) * radius;
        Debug.DrawLine(origin, prevPoint, Color.red, 0.3f);

        for (int i = 1; i <= segments; i++)
        {
            float a = startAngle + (angle / segments) * i;
            Vector2 nextPoint = origin + (Vector2)(Quaternion.Euler(0, 0, a) * Vector2.right) * radius;
            Debug.DrawLine(prevPoint, nextPoint, Color.red, 0.3f);
            prevPoint = nextPoint;
        }

        Debug.DrawLine(origin, prevPoint, Color.red, 0.3f);
    }
#endif

    public override void Process(GameObject player, ActiveSkillData data)
    {
        var animator = player.GetComponent<Animator>();
        if (animator != null && !string.IsNullOrEmpty(animTriggerName))
            animator.SetTrigger(animTriggerName);

        if (vfxPrefab != null)
        {
            var vfx = Instantiate(vfxPrefab, player.transform.position, player.transform.rotation);
            Object.Destroy(vfx, vfxDuration);
        }

        float damage = 0f;
        var statManager = player.GetComponent<PlayerStatManager>();
        if (statManager != null)
            damage = statManager.StatCore.getStat(StatType.SKILL_DAMAGE).calibratedValue * damageMultiplier;

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
            if (Vector2.Angle(forward, dir) > arcAngle / 2f) continue;

            float dist = Vector2.Distance(player.transform.position, hit.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = enemy;
            }
        }

        if (nearest != null)
            nearest.TakeDamage(damage);

#if UNITY_EDITOR
        DrawDebugArc(player.transform.position, forward, range, arcAngle);
#endif
        Debug.Log($"[Scratch] 발동 — damage={damage:F1}, 적중 {(nearest != null ? 1 : 0)}명");
    }
}
