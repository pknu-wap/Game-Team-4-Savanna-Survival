using UnityEngine;

[CreateAssetMenu(menuName = "SkillTree/Actions/MissileAction")]
public class MissileAction : AutoAction
{
    [SerializeField] GameObject missilePrefab;
    [SerializeField] float hungerCost;
    [SerializeField] float damageMultiplier = 1f;

    public override void Process(GameObject player, AutoSkillData data)
    {
        var statCore = player.GetComponent<PlayerStatManager>()?.StatCore;
        if (statCore == null) return;

        float currentHunger = statCore.getStat(StatType.HUNGER).rawValue;
        if (currentHunger < hungerCost) return;
        statCore.addStat(StatType.HUNGER, -hungerCost);

        int enemyLayerMask = LayerMask.GetMask("Enemy");
        Collider2D[] enemies = Physics2D.OverlapCircleAll(player.transform.position, data.range, enemyLayerMask);

        Transform target = null;
        float nearestDist = float.MaxValue;
        foreach (var e in enemies)
        {
            float dist = Vector2.Distance(player.transform.position, e.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                target = e.transform;
            }
        }

        if (target == null || missilePrefab == null) return;

#if UNITY_EDITOR
        DrawDebugCircle(player.transform.position, data.range);
#endif

        var missileCtrl = player.GetComponent<MissileController>();
        bool explosion = missileCtrl != null && missileCtrl.hasExplosion;
        float explosionRad = missileCtrl != null ? missileCtrl.explosionRadius : 0f;
        float explosionDmgBonus = missileCtrl != null ? missileCtrl.explosionDamageBonus : 0f;

        GameObject missileObj = Object.Instantiate(missilePrefab, player.transform.position, Quaternion.identity);
        var homing = missileObj.GetComponent<HomingMissile>();
        if (homing != null)
            homing.Init(target, player.GetComponent<PlayerStatManager>(), explosion, explosionRad, damageMultiplier, explosionDmgBonus);
    }

#if UNITY_EDITOR
    static void DrawDebugCircle(Vector2 origin, float radius)
    {
        int segments = 24;
        Vector2 prev = origin + Vector2.right * radius;
        for (int i = 1; i <= segments; i++)
        {
            float a = (360f / segments) * i * Mathf.Deg2Rad;
            Vector2 next = origin + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius;
            Debug.DrawLine(prev, next, Color.yellow, 0.3f);
            prev = next;
        }
    }
#endif
}
