using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private const float Speed = 10f;
    private Vector3 moveDirection;

    private void Update()
    {
        transform.Translate(Time.deltaTime * Speed * moveDirection);
    }

    private void OnMove(InputValue value)
    {
        //Debug.Log("inputed");
        moveDirection = value.Get<Vector2>();
        moveDirection.Normalize();
    }
}