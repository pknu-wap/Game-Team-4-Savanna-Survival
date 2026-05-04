using System.Collections.Generic;
using UnityEngine;

// Structure Data - ScriptableObject for structure configuration
[CreateAssetMenu(fileName = "StructureData", menuName = "Game/Structure Data")]
public class StructureData : ScriptableObject
{
    [SerializeField] public List<StructureEntry> structureEntries = new List<StructureEntry>();

    public StructureEntry GetEntry(StructureType type)
    {
        foreach (var entry in structureEntries)
        {
            if (entry.structureType == type)
                return entry;
        }
        return null;
    }
}
