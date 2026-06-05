using UnityEngine;

/// <summary>
/// 보스 공격 VFX 공통 컨트롤러.
/// 스폰 후 duration 초 뒤 자동 Destroy됩니다.
/// 
/// 사용법:
///   BossAttackVfxController.Spawn(prefab, position, rotation, duration);
/// </summary>
public class BossAttackVfxController : MonoBehaviour
{
    public static BossAttackVfxController Spawn(
        GameObject prefab, Vector3 position, Quaternion rotation, float duration, Transform parent = null)
    {
        if (prefab == null) return null;
        var go  = Instantiate(prefab, position, rotation, parent);
        var vfx = go.AddComponent<BossAttackVfxController>();
        Destroy(go, duration);
        return vfx;
    }
}