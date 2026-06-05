using UnityEngine;

/// 적이 사망할 때 발행되는 이벤트.
public class EnemyDeathEvent
{
    /// 사망한 적
    public Entity   Victim      { get; }
    /// 처치한 주체 (알 수 없으면 null)
    public Entity   Killer      { get; }
    /// 사망 위치 (월드 좌표)
    public Vector2  Position    { get; }
    /// 사망한 적의 최대 체력
    public float    MaxHp       { get; }
    /// 처치 시 획득 점수
    public int      Score       { get; }

    public EnemyDeathEvent(Entity victim, Entity killer, Vector2 position, float maxHp, int score = 0)
    {
        Victim   = victim;
        Killer   = killer;
        Position = position;
        MaxHp    = maxHp;
        Score    = score;
    }

    public Entity  getVictim()   => Victim;
    public Entity  getKiller()   => Killer;
    public Vector2 getPosition() => Position;
    public float   getMaxHp()    => MaxHp;
    public int     getScore()    => Score;
}