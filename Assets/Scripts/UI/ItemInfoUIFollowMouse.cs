using UnityEngine;
using UnityEngine.InputSystem;

public class ItemInfoUIFollowMouse : MonoBehaviour
{
    [SerializeField] private Vector2 leftTopOffset = new Vector2(10f, -10f);
    [SerializeField] private Vector2 rightTopOffset = new Vector2(-10f, -10f);
    [SerializeField] private Vector2 worldTargetOffset = new Vector2(0f, 30f);
    [SerializeField] private float rightEdgeThreshold = 230f;
    [SerializeField] private GameObject forceMouseModePanel;

    private RectTransform rectTransform;
    private Transform worldTarget;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (shouldFollowWorldTarget())
        {
            followWorldTarget();
            return;
        }

        followMouse();
    }

    public void followMouseMode()
    {
        worldTarget = null;
    }

    public void followWorldTargetMode(Transform target)
    {
        worldTarget = target;
    }

    private bool shouldFollowWorldTarget()
    {
        return worldTarget != null
            && (forceMouseModePanel == null || forceMouseModePanel.activeInHierarchy == false);
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

    private void followWorldTarget()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            return;
        }

        rectTransform.pivot = new Vector2(0.5f, 0f);
        rectTransform.position = (Vector2)mainCamera.WorldToScreenPoint(worldTarget.position) + worldTargetOffset;
    }
}
