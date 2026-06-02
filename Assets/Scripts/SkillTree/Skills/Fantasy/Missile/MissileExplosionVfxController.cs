using UnityEngine;

public class MissileExplosionVfxController : MonoBehaviour
{
    public void Init(float radius)
    {
        var ps = GetComponent<ParticleSystem>();
        if (ps == null) return;

        var shape = ps.shape;
        shape.radius = radius;

        var main = ps.main;
        main.startSize = new ParticleSystem.MinMaxCurve(radius * 0.1f, radius * 0.25f);

        ps.Play();
        Destroy(gameObject, main.duration + main.startLifetime.constantMax);
    }
}
