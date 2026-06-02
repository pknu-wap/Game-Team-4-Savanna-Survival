using UnityEngine;

[CreateAssetMenu(menuName = "SkillTree/Actions/StackSlaughterAction")]
public class StackSlaughterAction : ActiveAction
{
    [SerializeField] string animTriggerName = "wild_skill";
    [SerializeField] float fanAngle = 60f;
    [SerializeField] float range = 1.5f;
    [SerializeField] public float damageIncreasePerKill = 2f;
    [SerializeField] GameObject vfxPrefab;
    [SerializeField] GameObject killStackVfxPrefab;
    [SerializeField] float vfxDuration = 0.2f;
    [SerializeField] float killVfxDuration = 0.5f;

    public override void Process(GameObject player, ActiveSkillData data)
    {
        var animator = player.GetComponent<Animator>();
        if (animator != null && !string.IsNullOrEmpty(animTriggerName))
            animator.SetTrigger(animTriggerName);

        float baseDmg = 0f;
        var statManager = player.GetComponent<PlayerStatManager>();
        if (statManager != null)
            baseDmg = statManager.StatCore.getStat(StatType.SKILL_DAMAGE).calibratedValue;

        var ctrl = player.GetComponent<StackSlaughterController>();
        float finalDmg = baseDmg + (ctrl != null ? ctrl.bonusDamage : 0f);

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

        nearest.TakeDamage(finalDmg);

        if (vfxPrefab != null)
        {
            var vfx = Instantiate(vfxPrefab, nearest.transform.position, player.transform.rotation);
            Object.Destroy(vfx, vfxDuration);
        }

        if (nearest.IsDead && ctrl != null)
        {
            ctrl.OnKill();
            SpawnKillStackVfx(player);
        }

        Debug.Log($"[StackSlaughter] 발동 — finalDmg={finalDmg:F1}, bonus={ctrl?.bonusDamage:F1}");
    }

    void SpawnKillStackVfx(GameObject player)
    {
        if (killStackVfxPrefab == null) return;
        var vfx = Instantiate(killStackVfxPrefab, player.transform.position, Quaternion.identity, player.transform);
        Object.Destroy(vfx, killVfxDuration);
    }

    public override void OnUnlock(GameObject player)
    {
        var ctrl = player.GetComponent<StackSlaughterController>();
        if (ctrl == null)
            ctrl = player.AddComponent<StackSlaughterController>();
        ctrl.damageIncreasePerKill = damageIncreasePerKill;

        // 기존 소환물 제거
        var satCtrl = player.GetComponent<SatelliteController>();
        if (satCtrl != null)
        {
            foreach (var sat in satCtrl.activeSatellites)
                if (sat != null) Object.Destroy(sat);
            satCtrl.activeSatellites.Clear();
        }

        var petCtrl = player.GetComponent<PetController>();
        petCtrl?.ClearAllPets();
    }

    public override void Clear(GameObject player)
    {
        var ctrl = player.GetComponent<StackSlaughterController>();
        if (ctrl != null)
            Object.Destroy(ctrl);
    }
}
