using System.Collections.Generic;
using UnityEngine;

public class LaserBeamController : MonoBehaviour
{
    private readonly HashSet<object> damagedTargets = new();
    private float      damage;
    private GameObject vfxPrefab;
    private float      vfxDuration;
    private int        targetMask;
    private bool       isBoss;

    public void Init(float laserDamage, float duration,
                     GameObject hitVfxPrefab, float hitVfxDuration,
                     int mask = 0, bool boss = false)
    {
        damage      = laserDamage;
        vfxPrefab   = hitVfxPrefab;
        vfxDuration = hitVfxDuration;
        targetMask  = mask == 0 ? LayerMask.GetMask("Enemy") : mask;
        isBoss      = boss;

        DamageOverlappingTargets();
        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageTarget(other);
    }

    private void DamageOverlappingTargets()
    {
        Physics2D.SyncTransforms();

        Collider2D area = GetComponent<Collider2D>();
        ContactFilter2D filter = new();
        filter.useTriggers = true;
        filter.useLayerMask = true;
        filter.layerMask = targetMask;          // ✅ 레이어 필터 적용

        Collider2D[] hits = new Collider2D[64];
        int hitCount = area.Overlap(filter, hits);
        for (int i = 0; i < hitCount; i++)
            DamageTarget(hits[i]);
    }

    private void DamageTarget(Collider2D hit)
    {
        if (isBoss)
        {
            // ✅ 보스 레이저 — 플레이어 타격
            var playerEffect = hit.GetComponent<PlayerEffectTemp>();
            if (playerEffect != null && damagedTargets.Add(playerEffect))
            {
                playerEffect.TakeDamage(damage);
                SpawnVfx(hit.transform.position);
                return;
            }

            var statManager = hit.GetComponent<PlayerStatManager>();
            if (statManager != null && damagedTargets.Add(statManager))
            {
                float current = statManager.StatCore.getStat(StatType.HEALTH).rawValue;
                statManager.StatCore.registerStat(StatType.HEALTH, Mathf.Max(0f, current - damage));
                SpawnVfx(hit.transform.position);
            }
        }
        else
        {
            // 플레이어 레이저 — 적 타격
            Enemy enemy = hit.GetComponentInParent<Enemy>();
            if (enemy == null || !damagedTargets.Add(enemy)) return;

            enemy.TakeDamage(damage);
            SpawnVfx(enemy.transform.position);
        }
    }

    private void SpawnVfx(Vector3 pos)
    {
        if (vfxPrefab == null) return;
        var vfx = Instantiate(vfxPrefab, pos, Quaternion.identity);
        Destroy(vfx, vfxDuration);
    }
}
