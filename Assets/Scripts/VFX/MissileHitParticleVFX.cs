using UnityEngine;

public class MissileHitParticleVFX : MonoBehaviour
{
    [SerializeField] private int burstCount = 8;
    [SerializeField] private float lifetime = 0.3f;
    [SerializeField] private float minSpeed = 1f;
    [SerializeField] private float maxSpeed = 2.5f;
    [SerializeField] private float minSize = 0.05f;
    [SerializeField] private float maxSize = 0.12f;
    [SerializeField] private Color colorMin = new Color(1f, 0.2f, 0.05f);
    [SerializeField] private Color colorMax = new Color(1f, 0.55f, 0f);

    void Start()
    {
        var ps = gameObject.AddComponent<ParticleSystem>();

        var main = ps.main;
        main.loop = false;
        main.startLifetime = lifetime;
        main.startSpeed = new ParticleSystem.MinMaxCurve(minSpeed, maxSpeed);
        main.startSize = new ParticleSystem.MinMaxCurve(minSize, maxSize);
        main.startColor = new ParticleSystem.MinMaxGradient(colorMin, colorMax);
        main.stopAction = ParticleSystemStopAction.Destroy;
        main.maxParticles = burstCount + 2;

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)burstCount) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.08f;

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Sprites/Default"));
        renderer.sortingOrder = 5;

        ps.Play();
    }
}
