using UnityEngine;

/// 개별 몹 소환 정보
[System.Serializable]
public class MobSpawnEntry
{
    public GameObject mobPrefab;
    public int weight = 1;
    public int minGroupSize = 1;
    public int maxGroupSize = 1;
    [Tooltip("선호 구조물 타입. None이면 구조물 미연동 스폰")]
    public StructureType preferredStructure = StructureType.None;
}
