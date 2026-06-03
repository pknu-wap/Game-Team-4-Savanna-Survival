using UnityEngine;
using System.Collections.Generic;

public class PetController : MonoBehaviour
{
    public int maxPetCount = 1;
    public float petDamageMultiplier = 1f;
    public GameObject petPrefab;

    [HideInInspector]
    public List<GameObject> activePets = new();

    [HideInInspector]
    public List<Vector2> lastPetPositions = new();

    public void ClearAllPets()
    {
        foreach (var pet in activePets)
            if (pet != null) Destroy(pet);
        activePets.Clear();
    }
}
