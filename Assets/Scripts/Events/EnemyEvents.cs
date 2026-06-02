using System;
using UnityEngine;

public static class EnemyEvents
{
    public static event Action<EnemyDamageEvent> OnDamage;
    public static event Action<EnemyDeathEvent>  OnDeath;

    public static void PublishDamage(EnemyDamageEvent e)
    {
        float before = e.getDamage();
        OnDamage?.Invoke(e);
        float after = e.getDamage();

        if (!Mathf.Approximately(before, after))
            Debug.Log($"[EnemyEventBus] 데미지 이벤트 — 피해자: {e.getVictim().name}" +
                      $" / 공격자: {e.getAttacker()?.name ?? "없음"}" +
                      $" / 데미지: {before:F1} → {after:F1} (패시브 수정됨)" +
                      $" / HP: {e.getCurrentHp():F1}/{e.getMaxHp():F1}" +
                      $" / 타입: {e.getDamageType()}");
        else
            Debug.Log($"[EnemyEventBus] 데미지 이벤트 — 피해자: {e.getVictim().name}" +
                      $" / 공격자: {e.getAttacker()?.name ?? "없음"}" +
                      $" / 데미지: {after:F1}" +
                      $" / HP: {e.getCurrentHp():F1}/{e.getMaxHp():F1}" +
                      $" / 타입: {e.getDamageType()}");
    }

    public static void PublishDeath(EnemyDeathEvent e)
    {
        // Debug.Log($"[EnemyEventBus] 사망 이벤트 — 피해자: {e.getVictim().name}" +
        //           $" / 처치자: {e.getKiller()?.name ?? "없음"}" +
        //           $" / 위치: {e.getPosition()}");
        OnDeath?.Invoke(e);
    }
}