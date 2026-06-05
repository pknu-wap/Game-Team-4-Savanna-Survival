using UnityEngine;

[CreateAssetMenu(menuName = "SkillTree/Actions/BabyLionSummonAction")]
public class BabyLionSummonAction : AutoAction
{
    public override void Process(GameObject player, AutoSkillData data)
    {
        var petCtrl = player.GetComponent<PetController>();
        if (petCtrl == null)
        {
            Debug.LogWarning("[BabyLionSummon] PetController가 플레이어에 없습니다.");
            return;
        }

        petCtrl.activePets.RemoveAll(p => p == null);

        while (petCtrl.activePets.Count < petCtrl.maxPetCount)
        {
            if (petCtrl.petPrefab == null) break;

            Vector2 offset = Random.insideUnitCircle.normalized * 1.5f;
            Vector3 spawnPos = player.transform.position + (Vector3)offset;

            var pet = Instantiate(petCtrl.petPrefab, spawnPos, Quaternion.identity);
            var ai = pet.GetComponent<BabyLionAI>();
            if (ai != null)
            {
                ai.player = player.transform;
                ai.statManager = player.GetComponent<PlayerStatManager>();
                ai.petController = petCtrl;
            }
            petCtrl.activePets.Add(pet);
            Debug.Log($"[BabyLionSummon] 소환 완료 — {petCtrl.activePets.Count}/{petCtrl.maxPetCount}");
        }
    }

    public override void Clear(GameObject player)
    {
        var petCtrl = player.GetComponent<PetController>();
        if (petCtrl == null) return;

        petCtrl.lastPetPositions.Clear();
        foreach (var pet in petCtrl.activePets)
            if (pet != null) petCtrl.lastPetPositions.Add(pet.transform.position);

        petCtrl.ClearAllPets();
    }
}
