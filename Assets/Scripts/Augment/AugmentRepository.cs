using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class AugmentPowerDTO
{
    public int Level;
    public float Amount;
}

[Serializable]
public class AugmentDTO
{
    public int ID;
    public string Name;
    public string Description;
    public string Stat;
    public List<AugmentPowerDTO> Power;
}

public class AugmentRepository : MonoBehaviour
{

    public void Awake()
    {
        LoadAll();
        
    }
    private static readonly Dictionary<int, AugmentDTO> augments = new();

    public static void LoadAll()
    {
        
        string directoryPath = Application.dataPath + "/Data/Augments/";
        augments.Clear();

        if (!Directory.Exists(directoryPath))
        {
            Debug.LogError("디렉토리 없음: " + directoryPath);
            return;
        }

        foreach (string filePath in Directory.GetFiles(directoryPath, "*.json"))
        {
            string json = File.ReadAllText(filePath);
            AugmentDTO dto = JsonUtility.FromJson<AugmentDTO>(json);
            if (dto != null)
            {
                augments[dto.ID] = dto;
            }
        }

        Debug.Log($"Augment {augments.Count}개 로드됨.");
    }

    public static AugmentDTO GetById(int id)
    {
        // AI의 도움을 받음. TryGetValue를 통해 안전하게 로드할 수 있으며, out 키워드는 변수를 참조로 전달할 수 있음. 이를 이용해 탐색 횟수 2회 -> 1회로 감소
        augments.TryGetValue(id, out AugmentDTO dto);
        return dto;
    }

    public static IReadOnlyDictionary<int, AugmentDTO> GetAll()
    {
        return augments;
    }
}
