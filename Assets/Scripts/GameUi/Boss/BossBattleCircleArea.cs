using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BossBattleCircleArea : MonoBehaviour
{
    [Header("플레이어")]
    [SerializeField] private Transform player;

    [Header("원 크기")]
    [SerializeField] private float startRadius = 15f;
    [SerializeField] private float minRadius = 5f;

    [Header("축소 설정")]
    [SerializeField] private bool shrinkOverTime = true;
    [SerializeField] private float shrinkDelay = 10f;
    [SerializeField] private float shrinkSpeed = 0.5f;

    [Header("원 표시")]
    [SerializeField] private int circleSegments = 180;
    [SerializeField] private float lineWidth = 0.3f;
    [SerializeField] private Color circleColor = new Color(1f, 0f, 0f, 0.8f);

    private LineRenderer lineRenderer;

    private Vector3 centerPosition;
    private float currentRadius;
    private float timer;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogError("Player 태그를 가진 오브젝트를 찾을 수 없습니다.");
                enabled = false;
                return;
            }
        }

        centerPosition = player.position;
        centerPosition.z = 0f;

        currentRadius = startRadius;

        lineRenderer = GetComponent<LineRenderer>();

        SetupLineRenderer();
        DrawCircle();
    }

    private void Update()
    {
        if (player == null)
            return;

        timer += Time.deltaTime;

        if (shrinkOverTime && timer >= shrinkDelay)
        {
            currentRadius -= shrinkSpeed * Time.deltaTime;
            currentRadius = Mathf.Max(currentRadius, minRadius);

            DrawCircle();
        }

        ClampPlayerInsideCircle();
    }

    private void ClampPlayerInsideCircle()
    {
        Vector3 playerPos = player.position;
        playerPos.z = 0f;

        Vector3 direction = playerPos - centerPosition;
        float distance = direction.magnitude;

        if (distance > currentRadius)
        {
            Vector3 clampedPos =
                centerPosition +
                direction.normalized * currentRadius;

            clampedPos.z = player.position.z;

            player.position = clampedPos;
        }
    }

    private void SetupLineRenderer()
    {
        lineRenderer.useWorldSpace = true;
        lineRenderer.loop = true;
        lineRenderer.positionCount = circleSegments;

        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        lineRenderer.material = new Material(
            Shader.Find("Sprites/Default")
        );

        Gradient gradient = new Gradient();

        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(circleColor, 0f),
                new GradientColorKey(circleColor, 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(circleColor.a, 0f),
                new GradientAlphaKey(circleColor.a, 1f)
            }
        );

        lineRenderer.colorGradient = gradient;

        lineRenderer.sortingOrder = 999;
    }

    private void DrawCircle()
    {
        for (int i = 0; i < circleSegments; i++)
        {
            float angle =
                (float)i / circleSegments *
                Mathf.PI * 2f;

            float x =
                Mathf.Cos(angle) *
                currentRadius;

            float y =
                Mathf.Sin(angle) *
                currentRadius;

            Vector3 point =
                centerPosition +
                new Vector3(x, y, 0f);

            lineRenderer.SetPosition(i, point);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if (Application.isPlaying)
        {
            Gizmos.DrawWireSphere(
                centerPosition,
                currentRadius
            );
        }
    }
}