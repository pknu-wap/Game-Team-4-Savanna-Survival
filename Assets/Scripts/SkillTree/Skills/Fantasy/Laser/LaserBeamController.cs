using System.Collections.Generic;
using UnityEngine;

public class LaserBeamController : MonoBehaviour
{
    private readonly HashSet<Enemy> damagedEnemies = new();
    private float damage;
    private GameObject vfxPrefab;
    private float vfxDuration;

    public void Init(float laserDamage, float duration, GameObject hitVfxPrefab, float hitVfxDuration)
    {
        damage = laserDamage;
        vfxPrefab = hitVfxPrefab;
        vfxDuration = hitVfxDuration;
        DamageOverlappingEnemies();
        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageEnemy(other);
    }

    private void DamageOverlappingEnemies()
    {
        Physics2D.SyncTransforms();

        Collider2D area = GetComponent<Collider2D>();
        ContactFilter2D filter = new();
        filter.useTriggers = true;

        Collider2D[] hits = new Collider2D[64];
        int hitCount = area.Overlap(filter, hits);
        for (int i = 0; i < hitCount; i++)
            DamageEnemy(hits[i]);
    }

    private void DamageEnemy(Collider2D hit)
    {
        Enemy enemy = hit.GetComponentInParent<Enemy>();
        if (enemy == null || !damagedEnemies.Add(enemy)) return;

        enemy.TakeDamage(damage);
        if (vfxPrefab == null) return;

        var vfx = Instantiate(vfxPrefab, enemy.transform.position, Quaternion.identity);
        Destroy(vfx, vfxDuration);
    }
}
