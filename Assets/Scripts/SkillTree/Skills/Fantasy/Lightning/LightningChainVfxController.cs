using UnityEngine;

// 연쇄 번개 연결선. Init 호출 시 두 적 사이 하늘색 선을 그리고 duration 후 자동 소멸.
public class LightningChainVfxController : MonoBehaviour
{
    public void Init(Vector3 from, Vector3 to, float duration)
    {
        var lr = gameObject.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.useWorldSpace = true;
        lr.SetPosition(0, from);
        lr.SetPosition(1, to);
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        var chainColor = new Color(0.5f, 0.9f, 1f);
        lr.startColor = chainColor;
        lr.endColor = chainColor;
        lr.sortingLayerName = "Effect";
        lr.sortingOrder = 8;
        Destroy(gameObject, duration);
    }
}
