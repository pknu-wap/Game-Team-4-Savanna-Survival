using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    [Header("EXP")]
    [SerializeField] private int expAmount = 10;

    [Header("Magnet")]
    [SerializeField] private float magnetRange = 3f;
    [SerializeField] private float moveSpeed = 6f;

    private Transform player;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= magnetRange)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                moveSpeed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerDummy player = other.GetComponent<PlayerDummy>();

        if (player != null)
        {
            player.AddExp(expAmount);
            Destroy(gameObject);
        }
    }
}