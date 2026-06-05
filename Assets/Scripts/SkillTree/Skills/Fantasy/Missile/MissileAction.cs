using UnityEngine;

[CreateAssetMenu(menuName = "SkillTree/Actions/MissileAction")]
public class MissileAction : AutoAction
{
    [SerializeField] GameObject missilePrefab;
    [SerializeField] float hungerCost;
    [SerializeField] float damageMultiplier = 1f;

    public override void Process(GameObject player, AutoSkillData data)
    {
        var statManager = player.GetComponent<PlayerStatManager>();
        var statCore    = statManager?.StatCore;
        if (statCore == null) return;

        if (hungerCost > 0f)
        {
            try
            {
                float currentHunger = statCore.getStat(StatType.HUNGER).rawValue;
                if (currentHunger < hungerCost) return;
                statCore.addStat(StatType.HUNGER, -hungerCost);
            }
            catch { /* 보스는 HUNGER 없음 — 비용 없이 발사 */ }
        }

        int enemyLayerMask = LayerMask.GetMask("Enemy");

        // ✅ 발사자 콜라이더 수집 — 타겟 탐색 및 미사일 Init에 사용
        Collider2D[] ownerCols = player.GetComponents<Collider2D>();

        Collider2D[] enemies = Physics2D.OverlapCircleAll(player.transform.position,
                                                          data.range, enemyLayerMask);
        Transform target      = null;
        float     nearestDist = float.MaxValue;

        foreach (var e in enemies)
        {
            // ✅ 발사자 콜라이더 전부 제외
            bool isOwner = false;
            foreach (var oc in ownerCols)
                if (e == oc) { isOwner = true; break; }
            if (isOwner) continue;

            float dist = Vector2.Distance(player.transform.position, e.transform.position);
            if (dist < nearestDist) { nearestDist = dist; target = e.transform; }
        }

        if (target == null || missilePrefab == null) return;

#if UNITY_EDITOR
        DrawDebugCircle(player.transform.position, data.range);
#endif

        var missileCtrl        = player.GetComponent<MissileController>();
        bool  explosion        = missileCtrl != null && missileCtrl.hasExplosion;
        float explosionRad     = missileCtrl != null ? missileCtrl.explosionRadius : 0f;
        float explosionDmgBonus = missileCtrl != null ? missileCtrl.explosionDamageBonus : 0f;

        // ✅ 미사일을 보스 중심이 아닌 타겟 방향으로 약간 오프셋해서 스폰
        //    보스 콜라이더 반경만큼 밀어내 처음부터 충돌 범위 밖에서 시작
        Vector2 spawnDir    = ((Vector2)target.position - (Vector2)player.transform.position).normalized;
        float   spawnOffset = 0.6f; // 보스 콜라이더 반경보다 크게 설정
        Vector3 spawnPos    = player.transform.position + (Vector3)(spawnDir * spawnOffset);

        GameObject missileObj = Object.Instantiate(missilePrefab, spawnPos, Quaternion.identity);
        var homing = missileObj.GetComponent<HomingMissile>();
        if (homing != null)
        {
            // ✅ ownerCols[0]을 넘기면 Init 내부에서 GetComponentsInParent로 나머지도 처리
            Collider2D ownerCol = ownerCols.Length > 0 ? ownerCols[0] : null;
            homing.Init(target, statManager, explosion, explosionRad,
                        damageMultiplier, explosionDmgBonus, ownerCol);
        }
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