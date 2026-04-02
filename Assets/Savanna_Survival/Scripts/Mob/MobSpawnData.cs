using System.Collections.Generic;
using UnityEngine;

/// 낮/밤별 몹 소환 설정 (ScriptableObject)
[CreateAssetMenu(fileName = "MobSpawnData", menuName = "Savanna/Mob/SpawnData")]
public class MobSpawnData : ScriptableObject
{
    [SerializeField] public List<MobSpawnEntry> mobEntries = new();
}
