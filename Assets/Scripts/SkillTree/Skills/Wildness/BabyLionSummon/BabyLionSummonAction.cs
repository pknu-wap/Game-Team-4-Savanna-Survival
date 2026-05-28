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
                var statManager = player.GetComponent<PlayerStatManager>();
                float skillDamage = statManager != null
                    ? statManager.StatCore.getStat(StatType.SKILL_DAMAGE).calibratedValue
                    : 1f;
                ai.damage = skillDamage * petCtrl.petDamageMultiplier;
            }
            petCtrl.activePets.Add(pet);
            Debug.Log($"[BabyLionSummon] 소환 완료 — {petCtrl.activePets.Count}/{petCtrl.maxPetCount}, damage={ai?.damage:F1}");
        }
    }

    public override void Clear(GameObject player)
    {
        player.GetComponent<PetController>()?.ClearAllPets();
    }
}
