using System;
using UnityEngine;

public class LanderVisual : MonoBehaviour
{
    [SerializeField] private ParticleSystem thrustParticleSystem;
    [SerializeField] private ParticleSystem explosionParticleSystem;
    private Lander lander;

    private void Awake()
    {
        lander = GetComponent<Lander>();
    }

    private void Start()
    {
        lander.OnCrashed += Lander_OnCrashed;
    }

    private void Lander_OnCrashed(object sender, EventArgs e)
    {
        gameObject.SetActive(false); // Hide the lander
        Instantiate(explosionParticleSystem, transform.position, Quaternion.identity); // Play explosion effect
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
