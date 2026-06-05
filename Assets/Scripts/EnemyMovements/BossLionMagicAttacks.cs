using System.Collections.Generic;
using UnityEngine;

/// 마법 트리 패턴 실행 로직.
public partial class BossLion
{
    [Header("Magic - Attack VFX")]
    [SerializeField] private GameObject m1AttackVfxPrefab;
    [SerializeField] private float      m1VfxDuration     = 0.4f;
    [SerializeField] private GameObject m2AttackVfxPrefab;
    [SerializeField] private float      m2VfxDuration     = 0.5f;
    [SerializeField] private float      m2VfxHeightOffset = 5f;

    // ── M1 마법진 — 생성→전조→즉시폭발 반복 ─────────────────

    /// 패턴 진입 — 카운터 초기화 후 첫 마법진 생성
    internal void EnterM1Pattern()
    {
        magicPattern      = MagicPattern.M1Fuse;
        magicPatternTimer = 0f;
        m1FiredCount      = 0;
        MoveSmooth(Vector2.zero);
        SpawnM1Indicator();
        Debug.Log("[BossLion-Magic] M1 시작");
    }

    /// M1Fuse — 전조 시간 대기, 끝나면 즉시 폭발
    internal void UpdateM1Fuse()
    {
        if (magicPatternTimer < m1FuseTime) return;

        ExplodeM1();
        m1FiredCount++;

        if (m1FiredCount >= m1Count)
        {
            // 모두 소진 → 패턴 종료
            magicPattern = MagicPattern.None;
            ExitPattern();
            Debug.Log("[BossLion-Magic] M1 패턴 종료");
        }
        else
        {
            // 다음 마법진 생성 전 짧은 인터벌
            magicPattern      = MagicPattern.M1Interval;
            magicPatternTimer = 0f;
        }
    }

    /// M1Interval — 다음 마법진 생성까지 잠깐 대기
    internal void UpdateM1Interval()
    {
        if (magicPatternTimer < m1Interval) return;

        magicPatternTimer = 0f;
        magicPattern      = MagicPattern.M1Fuse;
        SpawnM1Indicator(); // 플레이어 현재 위치에 새 마법진 생성
    }

    /// 플레이어 현재 위치에 마법진 인디케이터 스폰
    private void SpawnM1Indicator()
{
    // ✅ Destroy 대신 즉시 비활성화 후 파괴 예약
    //    같은 프레임에 새 인디케이터를 만들어도 이전 것이 씬에 남지 않음
    if (m1CurrentIndicator != null)
    {
        m1CurrentIndicator.SetActive(false);
        Destroy(m1CurrentIndicator);
        m1CurrentIndicator = null;
    }

    if (m1IndicatorPrefab == null) return;

    Vector2 pos = player != null ? (Vector2)player.position : (Vector2)transform.position;
    m1CurrentIndicator = Instantiate(m1IndicatorPrefab, pos, Quaternion.identity);
    m1CurrentIndicator.transform.localScale = Vector3.one * m1Radius * 2f;

    Debug.Log($"[BossLion-Magic] M1 마법진 생성 ({m1FiredCount + 1}/{m1Count}) @ {pos}");
}

    /// 현재 마법진 위치에서 폭발 판정
    private void ExplodeM1()
{
    if (m1CurrentIndicator == null) return;

    Vector2 pos = m1CurrentIndicator.transform.position;

    // ✅ 동일하게 비활성화 후 파괴
    m1CurrentIndicator.SetActive(false);
    Destroy(m1CurrentIndicator);
    m1CurrentIndicator = null;

    if (m1ExplosionPrefab != null)
        Instantiate(m1ExplosionPrefab, pos, Quaternion.identity);

    BossAttackVfxController.Spawn(m1AttackVfxPrefab, pos, Quaternion.identity, m1VfxDuration);
    anim?.SetTrigger(AnimAttack);

    if (player != null && Vector2.Distance(pos, player.position) <= m1Radius)
    {
        DamagePlayer(m1Damage);
        Debug.Log($"[BossLion-Magic] M1 폭발 명중 — 데미지 {m1Damage}");
    }
}

    // ── M2 광역 심판 — 맵 전체 레이저, 안전구역 표시 ─────────

    internal void EnterM2Telegraph()
    {
        magicPattern      = MagicPattern.M2Telegraph;
        magicPatternTimer = 0f;
        m2Used            = true;
        m2DangerIndicators.Clear();
        m2SafeIndicators.Clear();
        m2HitPositions.Clear();
        m2SafePositions.Clear();
        MoveSmooth(Vector2.zero);
        PlaceM2Zones();
        Debug.Log("[BossLion-Magic] M2 광역 심판 전조 시작");
    }

    private void PlaceM2Zones()
    {
        int total = m2ZoneCount + m2SafeZoneCount;

        // 맵 중앙(0,0) 기준으로 균등하게 구역 배치
        List<Vector2> allPositions = new();
        for (int i = 0; i < total; i++)
        {
            float angle  = i * (360f / total) * Mathf.Deg2Rad;
            float radius = Random.Range(m2MapRadius * 0.25f, m2MapRadius * 0.9f);
            // ★ 맵 중앙(0,0) 기준
            allPositions.Add(new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius));
        }

        // 피셔-예이츠 셔플로 안전구역 인덱스 선택 — 무한루프 없음
        List<int> indices = new();
        for (int i = 0; i < total; i++) indices.Add(i);
        for (int i = total - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }

        int clampedSafe = Mathf.Clamp(m2SafeZoneCount, 0, total - 1);
        HashSet<int> safeSet = new(indices.GetRange(0, clampedSafe));

        for (int i = 0; i < total; i++)
        {
            Vector2 pos = allPositions[i];
            if (safeSet.Contains(i))
            {
                // ★ 안전구역 — 별도 프리팹(다른 색)으로 표시
                m2SafePositions.Add(pos);
                if (m2SafeIndicatorPrefab != null)
                {
                    var ind = Instantiate(m2SafeIndicatorPrefab, pos, Quaternion.identity);
                    ind.transform.localScale = Vector3.one * m2ZoneRadius * 2f;
                    m2SafeIndicators.Add(ind);
                }
            }
            else
            {
                // 위험구역
                m2HitPositions.Add(pos);
                if (m2DangerIndicatorPrefab != null)
                {
                    var ind = Instantiate(m2DangerIndicatorPrefab, pos, Quaternion.identity);
                    ind.transform.localScale = Vector3.one * m2ZoneRadius * 2f;
                    m2DangerIndicators.Add(ind);
                }
            }
        }
    }

    internal void UpdateM2Telegraph()
    {
        if (magicPatternTimer < m2Telegraph) return;

        // 전조 끝 — 인디케이터 제거 후 타격
        foreach (var ind in m2DangerIndicators) if (ind != null) Destroy(ind);
        foreach (var ind in m2SafeIndicators)   if (ind != null) Destroy(ind);
        m2DangerIndicators.Clear();
        m2SafeIndicators.Clear();

        magicPatternTimer = 0f;
        magicPattern      = MagicPattern.M2Strike;
    }

    internal void UpdateM2Strike()
    {
        anim?.SetTrigger(AnimAttack);

        // ★ 안전구역 안에 있으면 데미지 없음
        bool inSafeZone = false;
        if (player != null)
        {
            foreach (Vector2 safePos in m2SafePositions)
            {
                if (Vector2.Distance(safePos, player.position) <= m2ZoneRadius)
                {
                    inSafeZone = true;
                    break;
                }
            }
        }

        if (!inSafeZone && player != null)
        {
            // ★ 최대 HP의 70% 데미지 — DamagePlayer() 경유 (방어력 등 정상 적용)
            float maxHp  = playerStatCore.getStat(StatType.MAX_HEALTH).rawValue;
            float dmg    = maxHp * m2DamageRatio;
            DamagePlayer(dmg);
            Debug.Log($"[BossLion-Magic] M2 명중 — 데미지 {dmg:F1} (최대 HP {m2DamageRatio * 100f:F0}%)");
        }
        else
        {
            Debug.Log("[BossLion-Magic] M2 — 플레이어 안전구역 내 (데미지 없음)");
        }

        // 모든 위험 구역에 레이저 VFX
        foreach (Vector2 pos in m2HitPositions)
        {
            Vector3 vfxPos = new Vector3(pos.x, pos.y + m2VfxHeightOffset, 0f);
            BossAttackVfxController.Spawn(m2AttackVfxPrefab, vfxPos, Quaternion.identity, m2VfxDuration);
        }

        m2HitPositions.Clear();
        m2SafePositions.Clear();
        magicPattern = MagicPattern.None;
        ExitPattern();
    }
}