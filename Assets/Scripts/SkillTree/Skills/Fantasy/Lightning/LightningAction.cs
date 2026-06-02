using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SkillTree/Actions/LightningAction")]
public class LightningAction : AutoAction
{
    [SerializeField] GameObject warningVfxPrefab;
    [SerializeField] GameObject lightningVfxPrefab;
    [SerializeField] GameObject chainVfxPrefab;
    [SerializeField] float warningDuration = 0.4f;
    [SerializeField] float strikeVfxDuration = 0.3f;
    [SerializeField] float strikeRadius = 1f;
    [SerializeField] float hungerCost;
    [SerializeField] float damageMultiplier = 1.0f;

    public override void Process(GameObject player, AutoSkillData data)
    {
        var statCore = player.GetComponent<PlayerStatManager>()?.StatCore;
        if (statCore == null) return;

        float currentHunger = statCore.getStat(StatType.HUNGER).rawValue;
        if (currentHunger < hungerCost) return;
        statCore.addStat(StatType.HUNGER, -hungerCost);

        int enemyLayerMask = LayerMask.GetMask("Enemy");
        Collider2D[] enemies = Physics2D.OverlapCircleAll(player.transform.position, data.range, enemyLayerMask);
        if (enemies.Length == 0) return;

        Collider2D target = enemies[Random.Range(0, enemies.Length)];
        Vector3 strikePos = target.transform.position;

#if UNITY_EDITOR
        DrawDebugCircle(player.transform.position, data.range, Color.yellow, 0.3f);
        DrawDebugCircle(strikePos, strikeRadius, Color.red, warningDuration);
#endif

        var chainState = player.GetComponent<LightningAugmentState>();
        var controller = player.GetComponent<PlayerSkillController>();
        controller.StartCoroutine(StrikeCoroutine(strikePos, statCore, data.range, chainState));
    }

    private IEnumerator StrikeCoroutine(Vector3 pos, PlayerStatCore statCore, float chainSearchRange, LightningAugmentState chainState)
    {
        // 경고 표시 — strikeRadius 크기의 빨간 원, warningDuration 후 자동 소멸
        if (warningVfxPrefab != null)
        {
            var warningVfx = Object.Instantiate(warningVfxPrefab, pos, Quaternion.identity);
            warningVfx.GetComponent<LightningWarningVfxController>()?.Init(strikeRadius, warningDuration);
        }

        yield return new WaitForSeconds(warningDuration);

        // 타격 판정 + 피격 적마다 strike VFX 스폰
        int enemyLayerMask = LayerMask.GetMask("Enemy");
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, strikeRadius, enemyLayerMask);
        float dmg = statCore.getStat(StatType.SKILL_DAMAGE).calibratedValue * damageMultiplier;

        foreach (var hit in hits)
        {
            hit.GetComponent<Enemy>()?.TakeDamage(dmg);
            SpawnStrikeVfx(hit.transform);
        }

        if (chainState == null || !chainState.isChainEnabled || chainState.maxChainCount <= 0) yield break;

        // 연쇄 번개
        var hitTargets = new HashSet<GameObject>();
        foreach (var h in hits) hitTargets.Add(h.gameObject);

        Vector3 lastPos = pos;
        float chainDmg = statCore.getStat(StatType.SKILL_DAMAGE).calibratedValue
                       * (damageMultiplier + chainState.chainDamageBonus);
        float chainDamageDecay = chainState.chainDamageDecay;

        for (int i = 0; i < chainState.maxChainCount; i++)
        {
            Collider2D nearest = FindNearest(lastPos, chainSearchRange, enemyLayerMask, hitTargets);
            if (nearest == null) break;

            // 연결선 VFX
            if (chainVfxPrefab != null)
            {
                var chainVfx = Object.Instantiate(chainVfxPrefab, Vector3.zero, Quaternion.identity);
                chainVfx.GetComponent<LightningChainVfxController>()?.Init(lastPos, nearest.transform.position, strikeVfxDuration);
            }

            hitTargets.Add(nearest.gameObject);
            chainDmg *= chainDamageDecay;
            nearest.GetComponent<Enemy>()?.TakeDamage(chainDmg);
            SpawnStrikeVfx(nearest.transform);

            lastPos = nearest.transform.position;
        }
    }

    void SpawnStrikeVfx(Transform enemyTransform)
    {
        if (lightningVfxPrefab == null) return;
        var vfx = Object.Instantiate(lightningVfxPrefab, enemyTransform.position, Quaternion.identity, enemyTransform);
        Object.Destroy(vfx, strikeVfxDuration);
    }

    static Collider2D FindNearest(Vector3 origin, float range, int layerMask, HashSet<GameObject> exclude)
    {
        Collider2D nearest = null;
        float nearestDist = float.MaxValue;
        foreach (var c in Physics2D.OverlapCircleAll(origin, range, layerMask))
        {
            if (exclude.Contains(c.gameObject)) continue;
            float dist = Vector2.Distance(origin, c.transform.position);
            if (dist < nearestDist) { nearestDist = dist; nearest = c; }
        }
        return nearest;
    }

#if UNITY_EDITOR
    static void DrawDebugCircle(Vector2 origin, float radius, Color color, float duration)
    {
        int segments = 24;
        Vector2 prev = origin + Vector2.right * radius;
        for (int i = 1; i <= segments; i++)
        {
            float a = (360f / segments) * i * Mathf.Deg2Rad;
            Vector2 next = origin + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius;
            Debug.DrawLine(prev, next, color, duration);
            prev = next;
        }
    }
#endif
}
