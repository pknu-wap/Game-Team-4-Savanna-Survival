using UnityEngine;
using UnityEngine.InputSystem;

public class ItemInfoUIFollowMouse : MonoBehaviour
{
    [SerializeField] private Vector2 leftTopOffset = new Vector2(10f, -10f);
    [SerializeField] private Vector2 rightTopOffset = new Vector2(-10f, -10f);
    [SerializeField] private float rightEdgeThreshold = 230f;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        followMouse();
    }

    private void followMouse()
    {
        if (Mouse.current == null) return;
        Vector2 mousePos = Mouse.current.position.ReadValue();

        if (mousePos.x > Screen.width - rightEdgeThreshold)
        {
            rectTransform.pivot = new Vector2(1f, 1f);
            rectTransform.position = mousePos + rightTopOffset;
        }
        else
        {
            rectTransform.pivot = new Vector2(0f, 1f);
            rectTransform.position = mousePos + leftTopOffset;
        }
    }
}
