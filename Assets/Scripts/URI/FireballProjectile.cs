using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    private float direction = 1f;
    private float speed;
    private float damage;

    public void Initialize(float direction, float speed, float damage)
    {
        this.direction = direction;
        this.speed = speed;
        this.damage = damage;
    }

    private void Update()
    {
        transform.Translate(
            Vector2.right * direction * speed * Time.deltaTime
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            Debug.Log($"파이어볼 적중! 피해: {damage}");

            Destroy(gameObject);
        }
    }
}