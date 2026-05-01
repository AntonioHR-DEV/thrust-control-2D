using Unity.Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private float shakeDuration = 0.6f;
    [SerializeField] private float shakeAmplitude = 5f;
    [SerializeField] private float shakeFrequency = 4f;
    [SerializeField] private float fadeDuration = 0.6f;

    private CinemachineBasicMultiChannelPerlin noise;

    private void Start()
    {
        noise = cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        noise.AmplitudeGain = 0f; // silent at start

        Lander.Instance.OnCrashed += (s, e) => StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        noise.AmplitudeGain = shakeAmplitude;
        noise.FrequencyGain = shakeFrequency;

        yield return new WaitForSeconds(shakeDuration);

        // Smoothly fade out
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            noise.AmplitudeGain = Mathf.Lerp(shakeAmplitude, 0f, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        noise.AmplitudeGain = 0f;
    }
}