using UnityEngine;

[CreateAssetMenu(menuName = "SkillTree/Actions/SatelliteAction")]
public class SatelliteAction : AutoAction
{
    public override void Process(GameObject player, AutoSkillData data)
    {
        var ctrl = player.GetComponent<SatelliteController>();
        if (ctrl == null)
        {
            Debug.LogWarning("[SatelliteAction] SatelliteController가 플레이어에 없습니다.");
            return;
        }

        if (ctrl.activeSatellites.Count >= ctrl.maxSatelliteCount) return;

        ctrl.SpawnMissing(player.transform);
        Debug.Log($"[Satellite] 소환 완료 — {ctrl.activeSatellites.Count}/{ctrl.maxSatelliteCount}");
    }

    public override void Clear(GameObject player)
    {
        var ctrl = player.GetComponent<SatelliteController>();
        if (ctrl == null) return;

        foreach (var sat in ctrl.activeSatellites)
        {
            if (sat != null) Object.Destroy(sat);
        }
        ctrl.activeSatellites.Clear();
    }
}
