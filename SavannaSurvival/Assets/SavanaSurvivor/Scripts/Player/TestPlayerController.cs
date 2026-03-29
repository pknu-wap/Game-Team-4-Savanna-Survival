using UnityEngine;

/// <summary>
/// 맵 시스템 확인용 테스트 플레이어. 이동 로직만 포함.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class TestPlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector2 dir = new Vector2(h, v);
        if (dir.sqrMagnitude > 1f) dir.Normalize();

        _rb.linearVelocity = dir * moveSpeed;
    }
}
