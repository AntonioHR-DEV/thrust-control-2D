using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class AbsorbToTarget : MonoBehaviour
{
    [SerializeField] private float attractStrength = 8f;
    [SerializeField][Range(0f, 1f)] private float scatterPhaseRatio = 0.35f; // first 35% of lifetime = scatter
    private Transform target; // assign Lander
    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;

    private void Awake()
    {
        target = Lander.Instance.transform;
        ps = GetComponent<ParticleSystem>();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        int count = ps.particleCount;
        if (count == 0) return;

        if (particles == null || particles.Length < count)
            particles = new ParticleSystem.Particle[count];

        ps.GetParticles(particles, count);

        for (int i = 0; i < count; i++)
        {
            float lifetimeFraction = 1f - (particles[i].remainingLifetime / particles[i].startLifetime);

            if (lifetimeFraction < scatterPhaseRatio)
            {
                // Phase 1 — let the particle system's own velocity do the scattering
                // we just don't touch velocity here, particles fly outward naturally
                continue;
            }

            // Phase 2 — smoothly redirect toward lander
            Vector3 dir = (target.position - particles[i].position).normalized;

            // Remap lifetimeFraction from [scatterPhaseRatio → 1] to [0 → 1]
            float absorbT = Mathf.InverseLerp(scatterPhaseRatio, 1f, lifetimeFraction);

            particles[i].velocity = Vector3.Lerp(
                particles[i].velocity,
                dir * attractStrength,
                absorbT * Time.deltaTime * 8f
            );
        }

        ps.SetParticles(particles, count);
    }
}