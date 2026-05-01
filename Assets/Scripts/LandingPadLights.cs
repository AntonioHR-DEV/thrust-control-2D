using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LandingPadLights : MonoBehaviour
{
    [SerializeField] private Light2D redLight1;
    [SerializeField] private Light2D redLight2;

    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float minIntensity = 0.2f;
    [SerializeField] private float maxIntensity = 2.5f;

    private void Update()
    {
        float time = Time.time * pulseSpeed;

        // Light2 is offset by PI so they're always opposite phases
        redLight1.intensity = Mathf.Lerp(minIntensity, maxIntensity, (Mathf.Sin(time) + 1f) / 2f);
        redLight2.intensity = Mathf.Lerp(minIntensity, maxIntensity, (Mathf.Sin(time + Mathf.PI) + 1f) / 2f);
    }
}