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

    private void Update()
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

    // 현재 위성 수가 maxSatelliteCount 미만이면 부족한 수만큼 즉시 소환
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
    }

    // maxSatelliteCount 초과분 위성을 제거
    public void TrimExcess()
    {
        activeSatellites.RemoveAll(s => s == null);
        while (activeSatellites.Count > maxSatelliteCount)
        {
            int last = activeSatellites.Count - 1;
            if (activeSatellites[last] != null) Destroy(activeSatellites[last]);
            activeSatellites.RemoveAt(last);
        }
    }
}
