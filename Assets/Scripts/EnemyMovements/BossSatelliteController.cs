using UnityEngine;

public class BossSatelliteController : SatelliteController
{
    [Header("Boss Satellite")]
    [Tooltip("위성 유지 시간(초) — 0이면 Deactivate() 호출 전까지 유지")]
    [SerializeField] float duration = 5f;

    [Tooltip("위성이 탐색할 타겟 레이어 — 보스는 Player 레이어 설정")]
    [SerializeField] private LayerMask targetLayer;

    // ✅ SatelliteOrbit에서 읽어갈 타겟 레이어
    public LayerMask TargetLayer => targetLayer.value == 0
        ? (LayerMask)LayerMask.GetMask("Player")
        : targetLayer;

    // ✅ 보스 위성임을 SatelliteOrbit에 알림
    public bool IsBossOwned => true;

    private float timer;
    private bool active;

    public void Activate()
    {
        SpawnMissing(transform);
        timer  = 0f;
        active = duration > 0f;
        Debug.Log($"[BossSatelliteController] 위성 활성화 — 개수: {maxSatelliteCount}");
    }

    public void Deactivate()
    {
        active = false;
        foreach (var sat in activeSatellites)
            if (sat != null) Destroy(sat);
        activeSatellites.Clear();
        Debug.Log("[BossSatelliteController] 위성 비활성화");
    }

    protected override void Update()
    {
        activeSatellites.RemoveAll(s => s == null);

        if (!active) return;

        timer += Time.deltaTime;
        if (timer >= duration)
            Deactivate();
    }
}