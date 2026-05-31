using UnityEngine;

/// 적이 피해를 받을 때 발행되는 이벤트.
/// damage는 패시브 스킬에서 수정 가능 — 수정 후 Enemy.TakeDamage가 최종값을 적용합니다.
public class EnemyDamageEvent
{
    /// 피해를 받는 적
    public Entity   Victim      { get; }
    /// 공격한 주체 (플레이어 Entity 등, 알 수 없으면 null)
    public Entity   Attacker    { get; }
    /// 피해를 받는 적의 현재 체력 (피해 적용 전)
    public float    CurrentHp   { get; }
    /// 피해를 받는 적의 최대 체력
    public float    MaxHp       { get; }
    /// 피해 발생 위치 (월드 좌표)
    public Vector2  Position    { get; }
    /// 피해 유형 (근접, 원거리, 상태이상 등)
    public DamageType DamageType { get; }

    /// 실제 적용될 데미지 — 패시브 스킬에서 수정 가능
    public float Damage { get; private set; }

    public EnemyDamageEvent(Entity victim, Entity attacker, float damage, float currentHp, float maxHp,
                             Vector2 position, DamageType damageType = DamageType.Melee)
    {
        Victim     = victim;
        Attacker   = attacker;
        Damage     = damage;
        CurrentHp  = currentHp;
        MaxHp      = maxHp;
        Position   = position;
        DamageType = damageType;
    }

    public float  getDamage()              => Damage;
    public void   setDamage(float value)   => Damage = Mathf.Max(0f, value);
    public Entity getVictim()              => Victim;
    public Entity getAttacker()            => Attacker;
    public float  getCurrentHp()           => CurrentHp;
    public float  getMaxHp()               => MaxHp;
    public Vector2 getPosition()           => Position;
    public DamageType getDamageType()      => DamageType;

    /// 현재 체력 비율 (0~1)
    public float getHpRatio() => MaxHp > 0f ? CurrentHp / MaxHp : 0f;
}

public enum DamageType { Melee, Ranged, StatusEffect, Explosion }