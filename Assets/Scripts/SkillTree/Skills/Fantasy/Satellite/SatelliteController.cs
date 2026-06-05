using System.Collections.Generic;
using UnityEngine;

public class SatelliteController : MonoBehaviour
{
    public int maxSatelliteCount = 3;
    public float damageMultiplier = 1f;
    public bool hasKnockback;

    [SerializeField] float hungerDrainPerSecond;
    [SerializeField] public GameObject satellitePrefab;

    public List<GameObject> activeSatellites = new();

    private PlayerStatCore statCore;

    private void Start()
    {
        statCore = GetComponent<PlayerStatManager>()?.StatCore;
    }

    protected virtual void Update()
    {
        activeSatellites.RemoveAll(s => s == null);
        if (activeSatellites.Count == 0 || statCore == null) return;

        float currentHunger = statCore.getStat(StatType.HUNGER).rawValue;
        if (currentHunger <= 0f)
        {
            foreach (var sat in activeSatellites)
            {
                if (sat != null) Destroy(sat);
            }
            activeSatellites.Clear();
            return;
        }

        statCore.addStat(StatType.HUNGER, -hungerDrainPerSecond * Time.deltaTime);
    }

    // 현재 위성 수가 maxSatelliteCount 미만이면 부족한 수만큼 즉시 소환 후 균등 배분
    public void SpawnMissing(Transform playerTransform)
    {
        if (satellitePrefab == null) return;
        activeSatellites.RemoveAll(s => s == null);

        while (activeSatellites.Count < maxSatelliteCount)
        {
            var sat = Instantiate(satellitePrefab, playerTransform.position, Quaternion.identity);
            var orbit = sat.GetComponent<SatelliteOrbit>();
            if (orbit != null) orbit.Init(playerTransform, this);
            activeSatellites.Add(sat);
        }

        RedistributeAngles();
    }

    // 활성 위성 전체 각도를 균등하게 재배분
    public void RedistributeAngles()
    {
        int count = activeSatellites.Count;
        if (count == 0) return;
        float step = 360f / count;
        for (int i = 0; i < count; i++)
        {
            var orbit = activeSatellites[i]?.GetComponent<SatelliteOrbit>();
            orbit?.SetAngle(i * step);
        }
    }

    // maxSatelliteCount 초과분 위성을 제거 후 균등 재배분
    public void TrimExcess()
    {
        activeSatellites.RemoveAll(s => s == null);
        while (activeSatellites.Count > maxSatelliteCount)
        {
            int last = activeSatellites.Count - 1;
            if (activeSatellites[last] != null) Destroy(activeSatellites[last]);
            activeSatellites.RemoveAt(last);
        }
        RedistributeAngles();
    }
}