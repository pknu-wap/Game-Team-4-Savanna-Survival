using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    /*  이동을 적용해보는 과정에서, 스터디의 이동로직을 가져왔습니다. 
        이 로직은 단순하게 transform으로 위치를 옮기는 방식이라, 확장이 곤란한 방법일 수 있습니다.
            위치좌표만 프레임당 옮기는 방식이라, 속력이 0으로 나오는 등의 이슈가 발생합니다.
        추후에 확장성을 살펴보고, 타 방법으로 변경하겠습니다.
    */
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