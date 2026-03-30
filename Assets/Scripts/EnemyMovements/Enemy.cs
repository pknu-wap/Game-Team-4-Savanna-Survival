using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected float currentHp;
    protected Transform player;

    protected virtual void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    protected virtual void Update()
    {
        if (player == null) return;

        if (IsPlayerInDetection())
        {
            Move();
        }
    }

    protected abstract void Move();
    protected abstract bool IsPlayerInDetection();

    public virtual void TakeDamage(float damage)
    {
        currentHp -= damage;

        if (currentHp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}