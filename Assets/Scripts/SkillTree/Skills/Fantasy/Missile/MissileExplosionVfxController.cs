using UnityEngine;

public class MissileExplosionVfxController : MonoBehaviour
{
    [SerializeField] private float scaleMultiplier = 2f;

    public void Init(float radius)
    {
        var ps = GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var shape = ps.shape;
            shape.radius = radius;
            var main = ps.main;
            main.startSize = new ParticleSystem.MinMaxCurve(radius * 0.1f, radius * 0.25f);
            ps.Play();
            Destroy(gameObject, main.duration + main.startLifetime.constantMax);
            return;
        }

        // 스프라이트 기반 VFX: 폭발 반경에 비례하여 스케일 적용
        transform.localScale = Vector3.one * radius * scaleMultiplier;
    }
}
