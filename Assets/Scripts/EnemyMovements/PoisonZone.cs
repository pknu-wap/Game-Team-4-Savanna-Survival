using UnityEngine;

public class PoisonZone : MonoBehaviour
{
    private float poisonPercent;
    private float poisonTickInterval;
    private float poisonDuration;
    private float zoneDuration;
    private float elapsed;

    public void Init(float poisonPercent, float poisonTickInterval, float poisonDuration,
                     float zoneDuration, float radius)
    {
        this.poisonPercent      = poisonPercent;
        this.poisonTickInterval = poisonTickInterval;
        this.poisonDuration     = poisonDuration;
        this.zoneDuration       = zoneDuration;
        transform.localScale    = Vector3.one * radius * 2f;
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= zoneDuration) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        PlayerEffectTemp target = other.GetComponent<PlayerEffectTemp>();
        if (target == null) return;

        if (target.HasEffect<PoisonEffectTemp>(out PoisonEffectTemp existing))
            existing.Refresh(poisonDuration);
        else
            target.ApplyEffect(new PoisonEffectTemp(poisonPercent, poisonTickInterval, poisonDuration));
    }
}