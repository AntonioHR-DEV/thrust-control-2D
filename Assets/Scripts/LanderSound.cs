using System;
using UnityEngine;

public class LanderSound : MonoBehaviour
{
    [SerializeField] private AudioClip thrustSound;
    private AudioSource thrustAudioSource;

    private void Start()
    {
        thrustAudioSource = gameObject.AddComponent<AudioSource>();
        thrustAudioSource.clip = thrustSound;
        thrustAudioSource.volume = SoundManager.SoundVolume;
        thrustAudioSource.loop = true;
        thrustAudioSource.spatialBlend = 0.0f;
        thrustAudioSource.playOnAwake = false;

        SoundManager.Instance.OnVolumeChanged += SoundManager_OnVolumeChanged;
    }

    private void Update()
    {
        // Play the thrust sound when the lander is thrusting and game is not paused, and stop it when not
        if (Lander.Instance.IsThrusting() && Time.timeScale > 0f)
        {
            if (!thrustAudioSource.isPlaying)
                thrustAudioSource.Play();
        }
        else
        {
            if (thrustAudioSource.isPlaying)
                thrustAudioSource.Stop();
        }
    }

    private void SoundManager_OnVolumeChanged(object sender, EventArgs e)
    {
        thrustAudioSource.volume = SoundManager.SoundVolume;
    }
}
