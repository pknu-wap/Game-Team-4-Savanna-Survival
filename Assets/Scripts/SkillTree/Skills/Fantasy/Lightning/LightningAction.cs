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

        // ✅ 보스 여부 확인
        var bossCtrl = player.GetComponent<BossLightningController>();
        bool isBoss  = bossCtrl != null;

        // 허기 체크 — 보스는 스킵
        if (!isBoss)
        {
            float currentHunger = statCore.getStat(StatType.HUNGER).rawValue;
            if (currentHunger < hungerCost) return;
            statCore.addStat(StatType.HUNGER, -hungerCost);
        }

        // ✅ 타겟 탐색 레이어 분기
        //    보스 → Player 레이어, 플레이어 → Enemy 레이어
        int searchLayer = isBoss
            ? (int)bossCtrl.PlayerLayer
            : LayerMask.GetMask("Enemy");

        Collider2D[] candidates = Physics2D.OverlapCircleAll(
            player.transform.position, data.range, searchLayer);
        if (candidates.Length == 0) return;

        Collider2D target    = candidates[Random.Range(0, candidates.Length)];
        Vector3    strikePos = target.transform.position;

#if UNITY_EDITOR
        DrawDebugCircle(player.transform.position, data.range, Color.yellow, 0.3f);
        DrawDebugCircle(strikePos, strikeRadius, Color.red, warningDuration);
#endif

        var chainState = player.GetComponent<LightningAugmentState>();

        // ✅ 코루틴 실행자 — 보스는 BossSkillController, 플레이어는 PlayerSkillController
        //    둘 다 PlayerSkillController를 상속하므로 GetComponent<PlayerSkillController>()로 통일
        var controller = player.GetComponent<PlayerSkillController>();
        if (controller == null) return;

        controller.StartCoroutine(StrikeCoroutine(
            strikePos, statCore, data.range, chainState, searchLayer));
    }

    private IEnumerator StrikeCoroutine(Vector3 pos, PlayerStatCore statCore,
                                        float chainSearchRange,
                                        LightningAugmentState chainState,
                                        int searchLayer)
    {
        if (warningVfxPrefab != null)
        {
            var warningVfx = Object.Instantiate(warningVfxPrefab, pos, Quaternion.identity);
            warningVfx.GetComponent<LightningWarningVfxController>()
                      ?.Init(strikeRadius, warningDuration);
        }

        yield return new WaitForSeconds(warningDuration);

        // ✅ searchLayer로 타격 — 보스면 Player, 플레이어면 Enemy
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, strikeRadius, searchLayer);
        float dmg = statCore.getStat(StatType.SKILL_DAMAGE).calibratedValue * damageMultiplier;

        foreach (var hit in hits)
        {
            // ✅ 보스가 쏜 번개는 플레이어 피해 처리
            DamageTarget(hit, dmg);
            SpawnStrikeVfx(hit.transform);
        }

        if (chainState == null || !chainState.isChainEnabled || chainState.maxChainCount <= 0)
            yield break;

        // 연쇄 번개
        var hitTargets = new HashSet<GameObject>();
        foreach (var h in hits) hitTargets.Add(h.gameObject);

        Vector3 lastPos    = pos;
        float   chainDmg   = statCore.getStat(StatType.SKILL_DAMAGE).calibratedValue
                           * (damageMultiplier + chainState.chainDamageBonus);
        float   chainDecay = chainState.chainDamageDecay;

        for (int i = 0; i < chainState.maxChainCount; i++)
        {
            Collider2D nearest = FindNearest(lastPos, chainSearchRange, searchLayer, hitTargets);
            if (nearest == null) break;

            if (chainVfxPrefab != null)
            {
                var chainVfx = Object.Instantiate(chainVfxPrefab, Vector3.zero, Quaternion.identity);
                chainVfx.GetComponent<LightningChainVfxController>()
                        ?.Init(lastPos, nearest.transform.position, strikeVfxDuration);
            }

            hitTargets.Add(nearest.gameObject);
            chainDmg *= chainDecay;
            DamageTarget(nearest, chainDmg);
            SpawnStrikeVfx(nearest.transform);
            lastPos = nearest.transform.position;
        }
    }

    /// Enemy면 Enemy.TakeDamage, 플레이어면 PlayerEffectTemp 경유
    private void DamageTarget(Collider2D hit, float dmg)
    {
        var enemy = hit.GetComponent<Enemy>();
        if (enemy != null) { enemy.TakeDamage(dmg); return; }

        // 보스가 쏜 번개가 플레이어를 맞힌 경우
        var playerEffect = hit.GetComponent<PlayerEffectTemp>();
        if (playerEffect != null) { playerEffect.TakeDamage(dmg); return; }

        var statManager = hit.GetComponent<PlayerStatManager>();
        if (statManager == null) return;
        float current = statManager.StatCore.getStat(StatType.HEALTH).rawValue;
        statManager.StatCore.registerStat(StatType.HEALTH, Mathf.Max(0f, current - dmg));
    }

    void SpawnStrikeVfx(Transform target)
    {
        if (lightningVfxPrefab == null) return;
        var vfx = Object.Instantiate(lightningVfxPrefab, target.position,
                                     Quaternion.identity, target);
        Object.Destroy(vfx, strikeVfxDuration);
    }

    static Collider2D FindNearest(Vector3 origin, float range, int layerMask,
                                  HashSet<GameObject> exclude)
    {
        Collider2D nearest     = null;
        float      nearestDist = float.MaxValue;
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
        int     segments = 24;
        Vector2 prev     = origin + Vector2.right * radius;
        for (int i = 1; i <= segments; i++)
        {
            float   a    = (360f / segments) * i * Mathf.Deg2Rad;
            Vector2 next = origin + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius;
            Debug.DrawLine(prev, next, color, duration);
            prev = next;
        }
    }
#endif
}