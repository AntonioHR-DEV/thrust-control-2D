using System;
using Unity.Cinemachine;
using UnityEngine;

public class LanderVisual : MonoBehaviour
{
    [SerializeField] private ParticleSystem thrustParticleSystem;
    [SerializeField] private ParticleSystem explosionParticleSystem;
    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Start()
    {
        Lander.Instance.OnCrashed += Lander_OnCrashed;
    }

    private void Lander_OnCrashed(object sender, Lander.OnCrashedEventArgs e)
    {
        impulseSource.GenerateImpulse();
        gameObject.SetActive(false); // Hide the lander
        Instantiate(explosionParticleSystem, transform.position, Quaternion.identity); // Play explosion effect
    }

    private void Update()
    {
        // Play the thrust particle system when the lander is thrusting, and stop it when not
        if (Lander.Instance.IsThrusting())
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
