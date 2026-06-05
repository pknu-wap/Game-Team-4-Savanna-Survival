using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class VFXSpriteAnimator : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float fps = 24f;

    private SpriteRenderer _spriteRenderer;
    private int _frame;
    private float _timer;

    void Awake() => _spriteRenderer = GetComponent<SpriteRenderer>();

    void OnEnable()
    {
        _frame = 0;
        _timer = 0f;
        if (sprites != null && sprites.Length > 0)
            _spriteRenderer.sprite = sprites[0];
    }

    void Update()
    {
        if (sprites == null || sprites.Length == 0) return;

        _timer += Time.deltaTime;
        float frameTime = 1f / fps;
        if (_timer < frameTime) return;

        _timer -= frameTime;
        _frame++;

        if (_frame >= sprites.Length)
        {
            Destroy(gameObject);
            return;
        }

        _spriteRenderer.sprite = sprites[_frame];
    }
}
