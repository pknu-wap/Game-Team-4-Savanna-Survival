using System.Collections;
using UnityEngine;

public class WaterZone : MonoBehaviour
{
    private static readonly int ParamEnd = Animator.StringToHash("End");

    private float damagePerSecond;
    private float duration;
    private float length;
    private float width;

    private PlayerStatCore playerStatCore;

    private float tickInterval = 0.5f;
    private float tickTimer;
    private float lifeTimer;

    private Animator anim;
    private bool     isEnding;

    public void Init(float dps, float dur, float len, float wid)
    {
        damagePerSecond = dps;
        duration        = dur;
        length          = len;
        width           = wid;

        transform.localScale = new Vector3(length, width, 1f);
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isEnding) return;

        lifeTimer += Time.deltaTime;

        if (lifeTimer >= duration)
        {
            isEnding = true;
            StartCoroutine(EndRoutine());
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

    private IEnumerator EndRoutine()
    {
        if (anim != null)
        {
            anim.SetTrigger(ParamEnd);
            float length = 0f;
            foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
                if (clip.name == "End") { length = clip.length; break; }
            yield return new WaitForSeconds(length);
        }
        Destroy(gameObject);
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
        if (exiting == playerStatCore) playerStatCore = null;
    }
}