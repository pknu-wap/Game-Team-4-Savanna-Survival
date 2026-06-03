using UnityEngine;

[CreateAssetMenu(menuName = "SkillTree/Actions/AdultLionSummonAction")]
public class AdultLionSummonAction : AutoAction
{
    [SerializeField] GameObject adultLionPrefab;
    [SerializeField] GameObject summonVfxPrefab;
    [SerializeField] float adultDamageMultiplier = 2f;
    [SerializeField] float moveSpeed = 7f;
    [SerializeField] float chaseRange = 10f;
    [SerializeField] float leashRange = 6f;
    [SerializeField] float attackRange = 1.0f;
    [SerializeField] float attackCooldown = 1.2f;

    public override void Process(GameObject player, AutoSkillData data)
    {
        var petCtrl = player.GetComponent<PetController>();
        if (petCtrl == null)
        {
            Debug.LogWarning("[AdultLionSummon] PetController가 플레이어에 없습니다.");
            return;
        }

        if (adultLionPrefab == null)
        {
            Debug.LogWarning("[AdultLionSummon] adultLionPrefab이 연결되지 않았습니다.");
            return;
        }

        petCtrl.activePets.RemoveAll(p => p == null);

        while (petCtrl.activePets.Count < petCtrl.maxPetCount)
        {
            int idx = petCtrl.activePets.Count;
            Vector3 spawnPos = idx < petCtrl.lastPetPositions.Count
                ? (Vector3)petCtrl.lastPetPositions[idx]
                : player.transform.position + (Vector3)(Random.insideUnitCircle.normalized * 1.5f);

            var pet = Instantiate(adultLionPrefab, spawnPos, Quaternion.identity);

            if (summonVfxPrefab != null)
            {
                var vfx = Instantiate(summonVfxPrefab, pet.transform.position, Quaternion.identity, pet.transform);
                vfx.transform.localPosition = Vector3.zero;
            }
            var ai = pet.GetComponent<AdultLionAI>();
            if (ai != null)
            {
                ai.player = player.transform;
                ai.statManager = player.GetComponent<PlayerStatManager>();
                ai.petController = petCtrl;
                ai.damageMultiplier = adultDamageMultiplier;
                ai.moveSpeed = moveSpeed;
                ai.chaseRange = chaseRange;
                ai.leashRange = leashRange;
                ai.attackRange = attackRange;
                ai.attackCooldown = attackCooldown;
            }
            petCtrl.activePets.Add(pet);
            Debug.Log($"[AdultLionSummon] 소환 완료 — {petCtrl.activePets.Count}/{petCtrl.maxPetCount}");
        }

        petCtrl.lastPetPositions.Clear();
    }

    public override void Clear(GameObject player)
    {
        player.GetComponent<PetController>()?.ClearAllPets();
    }
}
