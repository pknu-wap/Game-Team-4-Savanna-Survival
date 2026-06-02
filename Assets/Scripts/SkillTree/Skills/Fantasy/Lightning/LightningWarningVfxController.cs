using UnityEngine;

// 번개 낙하 경고 원형 표시.
// LightningWarningVFX.prefab에 LineRenderer, SpriteRenderer가 미리 부착되어 있어야 함.
// Init(radius, duration) 호출 시 strikeRadius 크기로 맞춰지고 duration 후 자동 소멸.
public class LightningWarningVfxController : MonoBehaviour
{
    static Sprite cachedCircleSprite;
    static Texture2D cachedCircleTex;

    LineRenderer lr;
    SpriteRenderer sr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        sr = GetComponent<SpriteRenderer>();

        // 테두리 설정
        if (lr != null)
        {
            lr.loop = true;
            lr.useWorldSpace = true;
            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = Color.red;
            lr.endColor = Color.red;
            lr.sortingLayerName = "Effect";
            lr.sortingOrder = 5;
        }

        // 채우기 설정 (원형 텍스처 런타임 생성)
        if (sr != null)
        {
            sr.sprite = GetCircleSprite();
            sr.color = new Color(1f, 0.1f, 0.1f, 0.4f);
            sr.sortingLayerName = "Effect";
            sr.sortingOrder = 4;
        }

        // ParticleSystem 비활성
        var ps = GetComponent<ParticleSystem>();
        if (ps != null) ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var psr = GetComponent<ParticleSystemRenderer>();
        if (psr != null) psr.enabled = false;
    }

    public void Init(float radius, float duration)
    {
        // SpriteRenderer 크기: 1x1 스프라이트를 diameter(=radius*2)로 스케일
        transform.localScale = Vector3.one * radius * 2f;

        // LineRenderer: world space 원형
        if (lr != null)
        {
            Vector3 center = transform.position;
            int segments = 32;
            lr.positionCount = segments + 1;
            for (int i = 0; i <= segments; i++)
            {
                float a = (360f / segments) * i * Mathf.Deg2Rad;
                lr.SetPosition(i, center + new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0f) * radius);
            }
        }

        Destroy(gameObject, duration);
    }

    static Sprite GetCircleSprite()
    {
        if (cachedCircleSprite != null) return cachedCircleSprite;

        int res = 128;
        cachedCircleTex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        cachedCircleTex.filterMode = FilterMode.Bilinear;

        float c = res / 2f;
        var pixels = new Color32[res * res];
        for (int y = 0; y < res; y++)
        {
            for (int x = 0; x < res; x++)
            {
                float dx = x - c + 0.5f;
                float dy = y - c + 0.5f;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                byte alpha = (byte)(Mathf.Clamp01(c - dist + 1f) * 255);
                pixels[y * res + x] = new Color32(255, 255, 255, alpha);
            }
        }
        cachedCircleTex.SetPixels32(pixels);
        cachedCircleTex.Apply();

        cachedCircleSprite = Sprite.Create(
            cachedCircleTex,
            new Rect(0, 0, res, res),
            Vector2.one * 0.5f,
            res
        );
        return cachedCircleSprite;
    }
}
