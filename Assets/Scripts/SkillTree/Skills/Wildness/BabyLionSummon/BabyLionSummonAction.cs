using UnityEngine;

[CreateAssetMenu(menuName = "SkillTree/Actions/BabyLionSummonAction")]
public class BabyLionSummonAction : AutoAction
{
    [SerializeField] GameObject summonVfxPrefab;

    public override void Process(GameObject player, AutoSkillData data)
    {
        var petCtrl = player.GetComponent<PetController>();
        if (petCtrl == null)
        {
            Debug.LogWarning("[BabyLionSummon] PetController가 플레이어에 없습니다. 컴포넌트를 추가하세요.");
            return;
        }

        petCtrl.activePets.RemoveAll(p => p == null);
        if (petCtrl.activePets.Count >= petCtrl.maxPetCount) return;

        Vector2 offset = Random.insideUnitCircle.normalized * 1.5f;
        Vector3 spawnPos = player.transform.position + (Vector3)offset;

        if (summonVfxPrefab != null)
            Instantiate(summonVfxPrefab, spawnPos, Quaternion.identity);

        if (petCtrl.petPrefab != null)
        {
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
        player.GetComponent<PetController>()?.ClearAllPets();
    }
}
