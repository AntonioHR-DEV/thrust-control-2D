using System;
using System.Collections;
using UnityEngine;

public class LanderVisual : MonoBehaviour
{
    public static LanderVisual Instance { get; private set; }

    [SerializeField] private ParticleSystem thrustParticleSystem;
    [SerializeField] private ParticleSystem explosionParticleSystem;
    [SerializeField] private SpriteRenderer landerSprite;
    [SerializeField] private float flashDuration = 0.15f;

    private Color goldTintColor = new Color(1f, 0.85f, 0.1f);
    private Color greenTintColor = new Color(0.2f, 1f, 0.2f);

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Lander.Instance.OnCrashed += Lander_OnCrashed;
        Lander.Instance.OnCoinPicked += Lander_OnCoindPicked;
        Lander.Instance.OnFuelPicked += Lander_OnFuelPicked;
    }

    private void Lander_OnFuelPicked(object sender, EventArgs e)
    {
        FlashColor(greenTintColor);
    }

    private void Lander_OnCoindPicked(object sender, EventArgs e)
    {
        FlashColor(goldTintColor);
    }

    private void Lander_OnCrashed(object sender, Lander.OnCrashedEventArgs e)
    {
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

    private void FlashColor(Color flashColor)
    {
        StopCoroutine(nameof(FlashRoutine));
        StartCoroutine(FlashRoutine(flashColor));
    }

    private IEnumerator FlashRoutine(Color flashColor)
    {
        landerSprite.color = flashColor;
        yield return new WaitForSeconds(flashDuration);

        // Lerp back to white (neutral sprite color)
        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            landerSprite.color = Color.Lerp(flashColor, Color.white, elapsed / flashDuration);
            yield return null;
        }

        landerSprite.color = Color.white;
    }
}
