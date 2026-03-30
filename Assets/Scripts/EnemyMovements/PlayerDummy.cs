using UnityEngine;

public class PlayerDummy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxHp = 100f;

    private float currentHp;
    private Vector2 moveInput;

    private Rigidbody2D rb;
    private PlayerInputActions inputActions;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHp = maxHp;

        inputActions = new PlayerInputActions();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }

    
    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        Debug.Log($"플레이어 체력: {currentHp}");

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("플레이어 사망");
        Destroy(gameObject);

    }
}