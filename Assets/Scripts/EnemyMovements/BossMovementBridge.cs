using UnityEngine;

/// PlayerMovement를 상속해 보스 본체에 붙이는 브리지.
/// LeapAction / PounceAction의 stopMove(), GetLastMoveDirection() 호출을 받아냅니다.
/// PlayerMovement의 해당 메서드는 virtual이어야 합니다.
[DisallowMultipleComponent]
public class BossMovementBridge : PlayerMovement
{
    private Rigidbody2D rb;
    private Vector2 lastMoveDir = Vector2.right;
    private bool stopped;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// BossSkillRegistry가 매 프레임 보스 이동 방향을 갱신합니다.
    public void UpdateDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude > 0.01f)
            lastMoveDir = dir.normalized;
    }

    public override void stopMove()
    {
        stopped = true;
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    public override void reStartMove()
    {
        stopped = false;
        // Animator 호출 없음 — 보스는 isMoving 파라미터 없음
    }

    public override Vector2 GetLastMoveDirection() => lastMoveDir;

    public bool IsStopped => stopped;
}