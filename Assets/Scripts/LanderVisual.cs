using UnityEngine;

public class LanderVisual : MonoBehaviour
{
    [SerializeField] private ParticleSystem thrustParticleSystem;
    private Lander lander;

    private void Awake()
    {
        lander = GetComponent<Lander>();
    }

    private void Update()
    {
        // Play the thrust particle system when the lander is thrusting, and stop it when not
        if (lander.IsThrusting())
        {
            if (!thrustParticleSystem.isPlaying)
                thrustParticleSystem.Play();
        }
        else
        {
            if (thrustParticleSystem.isPlaying)
                thrustParticleSystem.Stop();
        }
    }
}
