using UnityEngine;

public class WaterZone : MonoBehaviour
{
    private float damagePerSecond;
    private float duration;
    private float length;
    private float width;

    private PlayerDummy playerInZone;

    private float tickInterval = 0.5f;
    private float tickTimer;
    private float lifeTimer;

    public void Init(float dps, float dur, float len, float wid)
    {
        damagePerSecond = dps;
        duration        = dur;
        length          = len;
        width           = wid;

        transform.localScale = new Vector3(length, width, 1f);
    }

    private void Update()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer >= duration)
        {
            Destroy(gameObject);
            return;
        }

        if (playerInZone != null)
        {
            tickTimer += Time.deltaTime;

            if (tickTimer >= tickInterval)
            {
                playerInZone.TakeDamage(damagePerSecond * tickInterval);
                tickTimer = 0f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerInZone != null) return;
        PlayerDummy pd = other.GetComponent<PlayerDummy>();
        if (pd != null) playerInZone = pd;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerDummy pd = other.GetComponent<PlayerDummy>();
        if (pd != null && pd == playerInZone)
            playerInZone = null;
    }
}