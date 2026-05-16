using UnityEngine;

/// 모든 살아있는 오브젝트(Enemy, Player 등)의 최상위 클래스.

public abstract class Entity : MonoBehaviour
{
    public abstract void TakeDamage(float damage);

    // TODO: 상태이상 시스템 추가 예정
}