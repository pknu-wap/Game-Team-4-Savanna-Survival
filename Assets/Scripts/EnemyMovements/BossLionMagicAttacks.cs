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

    // ── M1 마법진 ────────────────────────────────────────────

    internal void EnterM1Pattern()
    {
        magicPattern      = MagicPattern.M1Fuse;
        magicPatternTimer = 0f;
        m1FiredCount      = 0;
        MoveSmooth(Vector2.zero);
        SpawnM1Indicator();
        Debug.Log("[BossLion-Magic] M1 시작");
    }

    internal void UpdateM1Fuse()
    {
        if (magicPatternTimer < m1FuseTime) return;

        ExplodeM1();
        m1FiredCount++;

        if (m1FiredCount >= m1Count)
        {
            magicPattern = MagicPattern.None;
            ExitPattern();
        }
        else
        {
            magicPattern      = MagicPattern.M1Interval;
            magicPatternTimer = 0f;
        }
    }

    internal void UpdateM1Interval()
    {
        if (magicPatternTimer < m1Interval) return;

        magicPatternTimer = 0f;
        magicPattern      = MagicPattern.M1Fuse;
        SpawnM1Indicator();
    }

    private void SpawnM1Indicator()
    {
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
    }

    private void ExplodeM1()
    {
        if (m1CurrentIndicator == null) return;

        Vector2 pos = m1CurrentIndicator.transform.position;
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

    // ── M2 순차 레이저 ───────────────────────────────────────

    internal void EnterM2Sequence()
    {
        magicPattern           = MagicPattern.M2Sequence;
        magicPatternTimer      = 0f;
        m2Used                 = true;
        m2CurrentLaserIndex    = 0;
        m2PlayerHitThisPattern = false;
        MoveSmooth(Vector2.zero);
        SpawnNextLaserIndicator();
        Debug.Log("[BossLion-Magic] M2 순차 레이저 시작");
    }

    internal void UpdateM2Sequence()
    {
        switch (m2Phase)
        {
            case M2Phase.Telegraph:
                if (magicPatternTimer < m2LaserTelegraph) return;
                FireCurrentLaser();
                m2Phase           = M2Phase.Interval;
                magicPatternTimer = 0f;
                break;

            case M2Phase.Interval:
                if (magicPatternTimer < m2LaserInterval) return;
                m2CurrentLaserIndex++;
                if (m2CurrentLaserIndex >= m2LaserCount)
                {
                    Debug.Log("[BossLion-Magic] M2 시퀀스 종료");
                    magicPattern = MagicPattern.None;
                    ExitPattern();
                }
                else
                {
                    SpawnNextLaserIndicator();
                }
                break;
        }
    }

    private void SpawnNextLaserIndicator()
    {
        // 보스 주위 랜덤 위치 — 최소/최대 반경 사이
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float dist  = Random.Range(m2MinRadius, m2MaxRadius);
        m2CurrentLaserPos = (Vector2)transform.position
            + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;

        if (m2DangerIndicatorPrefab != null)
        {
            if (m2CurrentLaserIndicator != null)
            {
                m2CurrentLaserIndicator.SetActive(false);
                Destroy(m2CurrentLaserIndicator);
            }
            m2CurrentLaserIndicator = Instantiate(m2DangerIndicatorPrefab,
                m2CurrentLaserPos, Quaternion.identity);
            m2CurrentLaserIndicator.transform.localScale = Vector3.one * m2ZoneRadius * 2f;
        }

        m2Phase           = M2Phase.Telegraph;
        magicPatternTimer = 0f;
        Debug.Log($"[BossLion-Magic] M2 레이저 {m2CurrentLaserIndex + 1}/{m2LaserCount} 전조");
    }

    private void FireCurrentLaser()
    {
        if (m2CurrentLaserIndicator != null)
        {
            m2CurrentLaserIndicator.SetActive(false);
            Destroy(m2CurrentLaserIndicator);
            m2CurrentLaserIndicator = null;
        }

        Vector3 vfxPos = new Vector3(m2CurrentLaserPos.x,
            m2CurrentLaserPos.y + m2VfxHeightOffset, 0f);
        BossAttackVfxController.Spawn(m2AttackVfxPrefab, vfxPos,
            Quaternion.identity, m2VfxDuration);
        anim?.SetTrigger(AnimAttack);

        // 이미 맞은 적 있으면 이후 레이저 데미지 없음
        if (!m2PlayerHitThisPattern && player != null
            && Vector2.Distance(m2CurrentLaserPos, player.position) <= m2ZoneRadius)
        {
            float maxHp = playerStatCore.getStat(StatType.MAX_HEALTH).rawValue;
            float dmg   = maxHp * m2DamageRatio;
            DamagePlayer(dmg);
            m2PlayerHitThisPattern = true;
            Debug.Log($"[BossLion-Magic] M2 명중 — 데미지 {dmg:F1} / 이후 면역");
        }
    }
}