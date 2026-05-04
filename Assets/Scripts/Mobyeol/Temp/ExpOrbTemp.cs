using UnityEngine;

public class ExpOrbTemp : MonoBehaviour
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
        PlayerLevel player = other.GetComponent<PlayerLevel>();

        if (player != null)
        {
            player.addExp(expAmount);
            Destroy(gameObject);
        }
    }
}