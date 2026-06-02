using UnityEngine;

public class StackSlaughterController : MonoBehaviour
{
    public float bonusDamage = 0f;
    public float damageIncreasePerKill;

    public void OnKill()
    {
        bonusDamage += damageIncreasePerKill;
        Debug.Log($"[StackSlaughter] 처치 보너스 누적 — bonusDamage={bonusDamage:F1}");
    }
}
