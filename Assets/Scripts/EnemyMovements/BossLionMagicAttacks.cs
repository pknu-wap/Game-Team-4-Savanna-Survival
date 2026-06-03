using System.Collections.Generic;
using UnityEngine;

/// 마법 트리 패턴 실행 로직.
public partial class BossLion
{
    // ── M1 마법진 소환 ────────────────────────────────────────

    internal void EnterM1Spawning()
    {
        magicPattern      = MagicPattern.M1Spawning;
        magicPatternTimer = 0f;
        m1SpawnedCount    = 0;
        m1SpawnTimer      = 0f;
        m1ActiveIndicators.Clear();
        m1Positions.Clear();
        MoveSmooth(Vector2.zero);
        Debug.Log("[BossLion-Magic] M1 마법진 소환 시작");
    }

    internal void UpdateM1Spawning()
    {
        m1SpawnTimer += Time.deltaTime;

        // 첫 마법진은 즉시, 이후 m1Delay 간격으로 순차 생성
        if (m1SpawnTimer >= m1Delay && m1SpawnedCount < m1Count)
        {
            Vector2 pos = player != null ? (Vector2)player.position : (Vector2)transform.position;
            SpawnArcaneIndicator(pos);
            m1SpawnedCount++;
            m1SpawnTimer = 0f;
            Debug.Log($"[BossLion-Magic] M1 마법진 {m1SpawnedCount}/{m1Count} 생성");
        }

        // 모두 생성되면 폭발 대기로 전환 — fuseTimer를 별도로 관리
        if (m1SpawnedCount >= m1Count)
        {
            magicPattern      = MagicPattern.M1Waiting;
            magicPatternTimer = 0f; // 마지막 마법진 생성 시점부터 fuseTime 측정
        }
    }

    internal void UpdateM1Waiting()
    {
        if (magicPatternTimer < m1FuseTime) return;

        foreach (Vector2 pos in m1Positions) ExplodeArcane(pos);
        foreach (GameObject ind in m1ActiveIndicators) if (ind != null) Destroy(ind);

        m1ActiveIndicators.Clear();
        m1Positions.Clear();
        magicPattern = MagicPattern.None;
        ExitPattern();
        Debug.Log("[BossLion-Magic] M1 마법진 전체 폭발");
    }

    private void SpawnArcaneIndicator(Vector2 pos)
    {
        if (m1IndicatorPrefab == null) return;
        GameObject ind = Instantiate(m1IndicatorPrefab, pos, Quaternion.identity);
        ind.transform.localScale = Vector3.one * m1Radius * 2f;
        m1ActiveIndicators.Add(ind);
        m1Positions.Add(pos);
    }

    private void ExplodeArcane(Vector2 pos)
    {
        if (player == null) return;
        if (Vector2.Distance(pos, player.position) <= m1Radius)
        {
            DamagePlayer(m1Damage);
            Debug.Log($"[BossLion-Magic] M1 마법진 폭발 명중 — 데미지 {m1Damage}");
        }
        if (m1ExplosionPrefab != null)
            Instantiate(m1ExplosionPrefab, pos, Quaternion.identity);
    }

    // ── M2 광역 심판 ─────────────────────────────────────────

    internal void EnterM2Telegraph()
    {
        magicPattern      = MagicPattern.M2Telegraph;
        magicPatternTimer = 0f;
        m2Used            = true;
        m2Indicators.Clear();
        m2HitPositions.Clear();
        MoveSmooth(Vector2.zero);
        PlaceM2Indicators();
        Debug.Log("[BossLion-Magic] M2 광역 심판 전조 시작");
    }

    private void PlaceM2Indicators()
    {
        int total = m2ZoneCount + m2SafeZoneCount;

        List<Vector2> allPositions = new();
        for (int i = 0; i < total; i++)
        {
            float angle  = i * (360f / total) * Mathf.Deg2Rad;
            float radius = Random.Range(m2MapRadius * 0.3f, m2MapRadius);
            allPositions.Add((Vector2)transform.position +
                             new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius));
        }

        HashSet<int> safeIndices = new();
        while (safeIndices.Count < m2SafeZoneCount)
            safeIndices.Add(Random.Range(0, total));

        for (int i = 0; i < total; i++)
        {
            if (safeIndices.Contains(i)) continue;
            m2HitPositions.Add(allPositions[i]);
            if (m2IndicatorPrefab != null)
            {
                GameObject ind = Instantiate(m2IndicatorPrefab, allPositions[i], Quaternion.identity);
                ind.transform.localScale = Vector3.one * m2ZoneRadius * 2f;
                m2Indicators.Add(ind);
            }
        }
    }

    internal void UpdateM2Telegraph()
    {
        if (magicPatternTimer < m2Telegraph) return;

        foreach (GameObject ind in m2Indicators) if (ind != null) Destroy(ind);
        m2Indicators.Clear();

        magicPatternTimer = 0f;
        magicPattern      = MagicPattern.M2Strike;
    }

    internal void UpdateM2Strike()
    {
        if (player != null)
        {
            foreach (Vector2 pos in m2HitPositions)
            {
                if (Vector2.Distance(pos, player.position) > m2ZoneRadius) continue;

                // 고정 피해: 방어력/데미지 감소 무시 → registerStat 직접 접근
                float maxHp    = playerStatCore.getStat(StatType.MAX_HEALTH).rawValue;
                float fixedDmg = maxHp * m2DamageRatio;
                float current  = playerStatCore.getStat(StatType.HEALTH).rawValue;
                playerStatCore.registerStat(StatType.HEALTH, Mathf.Max(0f, current - fixedDmg));
                Debug.Log($"[BossLion-Magic] M2 광역 심판 명중 — 고정 피해 {fixedDmg:F1}");
                break;
            }
        }

        m2HitPositions.Clear();
        magicPattern = MagicPattern.None;
        ExitPattern();
    }
}