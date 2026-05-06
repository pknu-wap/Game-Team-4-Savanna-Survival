using UnityEngine;

public class WaterZone : MonoBehaviour
{
    private float damagePerSecond;
    private float duration;
    private float length;
    private float width;

    private PlayerStatCore playerStatCore;

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

        if (playerStatCore != null)
        {
            tickTimer += Time.deltaTime;

            if (tickTimer >= tickInterval)
            {
                float currentHp = playerStatCore.getStat(StatType.HEALTH).rawValue;
                float next      = Mathf.Max(0f, currentHp - damagePerSecond * tickInterval);
                playerStatCore.registerStat(StatType.HEALTH, next);
                tickTimer = 0f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerStatCore != null) return;

        if (!other.CompareTag("Player")) return;

        playerStatCore = other.GetComponent<PlayerStatManager>()?.StatCore;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerStatCore exiting = other.GetComponent<PlayerStatManager>()?.StatCore;
        if (exiting == playerStatCore)
            playerStatCore = null;
    }
}