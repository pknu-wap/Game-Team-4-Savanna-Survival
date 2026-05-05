using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerStatCore statCore;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float speed;
    private Vector3 moveDirection;

    private bool canMove = true;

    private void Start()
    {
        PlayerStatManager playerStatManager = GetComponent<PlayerStatManager>();
        statCore = playerStatManager.StatCore;

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (!canMove) return;

        speed = statCore.getStat(StatType.MOVESPEED).calibratedValue;
        transform.Translate(Time.deltaTime * speed * moveDirection);
    }

    private void OnMove(InputValue value)
    {
        if (!canMove) return;

        //Debug.Log("inputed");
        moveDirection = value.Get<Vector2>();
        moveDirection.Normalize();

        bool isMoving = moveDirection != Vector3.zero;
        animator.SetBool("isMoving", isMoving);

        if (moveDirection.x > 0f)
        {
            spriteRenderer.flipX = true;
        }
        else if (moveDirection.x < 0f)
        {
            spriteRenderer.flipX = false;
        }
    }

    public void stopMove()
    {
        canMove = false;
        moveDirection = Vector3.zero;
    }
}