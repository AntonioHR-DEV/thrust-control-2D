using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LanderLights : MonoBehaviour
{
    [SerializeField] private Light2D[] greenLights;
    [SerializeField] private float interval = 0.3f;
    [SerializeField] private float activeIntensity = 1.5f;
    [SerializeField] private float inactiveIntensity = 0.1f;

    private int currentIndex = 0;

    private void Start()
    {
        StartCoroutine(AlternateLights());
    }

    private IEnumerator AlternateLights()
    {
        while (true)
        {
            // Dim all lights
            foreach (var light in greenLights)
                light.intensity = inactiveIntensity;

            // Activate current one
            greenLights[currentIndex].intensity = activeIntensity;

            currentIndex = (currentIndex + 1) % greenLights.Length;

            yield return new WaitForSeconds(interval);
        }
    }
}