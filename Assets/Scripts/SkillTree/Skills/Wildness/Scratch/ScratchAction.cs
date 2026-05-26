using UnityEngine;

[CreateAssetMenu(menuName = "SkillTree/Actions/ScratchAction")]
public class ScratchAction : ActiveAction
{
    [SerializeField] string animTriggerName = "Scratch";
    [SerializeField] GameObject vfxPrefab;
    [SerializeField] float range = 1.5f;
    [SerializeField] float arcAngle = 120f;
    [SerializeField] float damageMultiplier = 1.0f;

    public override void Process(GameObject player, ActiveSkillData data)
    {
        var animator = player.GetComponent<Animator>();
        if (animator != null && !string.IsNullOrEmpty(animTriggerName))
            animator.SetTrigger(animTriggerName);

        if (vfxPrefab != null)
            Instantiate(vfxPrefab, player.transform.position, player.transform.rotation);

        float damage = 0f;
        var statManager = player.GetComponent<PlayerStatManager>();
        if (statManager != null)
            damage = statManager.StatCore.getStat(StatType.DAMAGE).calibratedValue * damageMultiplier;

        var hits = Physics2D.OverlapCircleAll(player.transform.position, range);
        Vector2 forward = player.transform.right;

        foreach (var hit in hits)
        {
            if (hit.gameObject == player) continue;
            var enemy = hit.GetComponent<Enemy>();
            if (enemy == null) continue;

            Vector2 dir = ((Vector2)hit.transform.position - (Vector2)player.transform.position).normalized;
            if (Vector2.Angle(forward, dir) <= arcAngle / 2f)
                enemy.TakeDamage(damage);
        }
    }
}
