using UnityEngine;

public class ItemInfoUIFollowWorldTarget : MonoBehaviour
{
    [SerializeField] private Vector2 worldTargetOffset = new Vector2(0f, 30f);
    [SerializeField] private Camera targetCamera;

    private RectTransform rectTransform;
    private Transform worldTarget;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (worldTarget == null)
        {
            return;
        }

        followWorldTarget();
    }

    public void setTarget(Transform target)
    {
        worldTarget = target;
        if (worldTarget != null)
        {
            followWorldTarget();
        }
    }

    public void clearTarget()
    {
        worldTarget = null;
    }

    private void followWorldTarget()
    {
        Camera followCamera = getFollowCamera();
        if (followCamera == null)
        {
            return;
        }

        rectTransform.pivot = new Vector2(0.5f, 0f);
        rectTransform.position = (Vector2)followCamera.WorldToScreenPoint(worldTarget.position) + worldTargetOffset;
    }

    private Camera getFollowCamera()
    {
        if (targetCamera != null)
        {
            return targetCamera;
        }

        targetCamera = Camera.main;
        if (targetCamera != null)
        {
            return targetCamera;
        }

        targetCamera = FindObjectOfType<Camera>();
        return targetCamera;
    }
}
