using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class SucceedItemSaveData
{
    public List<string> equipmentIds = new List<string>();
}

public static class SucceedItemSaveStorage
{
    private const int MaxSaveCount = 3;
    private const string SaveFileName = "succeed_items.json";

    private static string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    public static void save(IReadOnlyList<EquipmentData> equipments)
    {
        SucceedItemSaveData saveData = new SucceedItemSaveData();

        if (equipments != null)
        {
            for (int i = 0; i < equipments.Count && saveData.equipmentIds.Count < MaxSaveCount; ++i)
            {
                if (equipments[i] != null && string.IsNullOrEmpty(equipments[i].equipmentId) == false)
                {
                    saveData.equipmentIds.Add(equipments[i].equipmentId);
                }
            }
        }

        File.WriteAllText(SavePath, JsonUtility.ToJson(saveData, true));
    }

    public static List<string> loadIds()
    {
        if (File.Exists(SavePath) == false)
        {
            return new List<string>();
        }

        SucceedItemSaveData saveData = JsonUtility.FromJson<SucceedItemSaveData>(File.ReadAllText(SavePath));
        return saveData != null && saveData.equipmentIds != null ? saveData.equipmentIds : new List<string>();
    }

    public static void clearIds()
    {
        File.WriteAllText(SavePath, JsonUtility.ToJson(new SucceedItemSaveData(), true));
    }
}
