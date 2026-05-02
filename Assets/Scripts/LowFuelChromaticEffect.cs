using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LowFuelChromaticEffect : MonoBehaviour
{
    [SerializeField] private Volume globalVolume;

    [Header("Pulse Settings")]
    [SerializeField] private float minIntensity = 0f;
    [SerializeField] private float maxIntensity = 0.4f;
    [SerializeField] private float pulseSpeed = 1.5f;

    [Header("Fade Settings")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 1f;

    private ChromaticAberration chromaticAberration;
    private bool isWarningActive = false;
    private Coroutine activeCoroutine;

    private void Start()
    {
        // Grab the ChromaticAberration component from the volume profile
        if (!globalVolume.profile.TryGet(out chromaticAberration))
        {
            Debug.LogError("No Chromatic Aberration override found on Global Volume!");
            return;
        }

        chromaticAberration.intensity.value = 0f;

        Lander.Instance.OnFuelLow += OnFuelLow;
        Lander.Instance.OnFuelPicked += OnFuelPicked;
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void OnFuelLow(object sender, EventArgs e)
    {
        if (isWarningActive) return;
        isWarningActive = true;

        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(FadeInThenPulse());
    }

    private void OnFuelPicked(object sender, EventArgs e)
    {
        if (!isWarningActive) return;
        isWarningActive = false;

        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(FadeOutRoutine());
    }

    private void GameManager_OnGameStateChanged(object sender, EventArgs e)
    {
        if (!isWarningActive && GameManager.Instance.State != GameManager.GameState.GameOver) return;
        isWarningActive = false;

        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeInThenPulse()
    {
        // Phase 1 — fade CA in from 0 to maxIntensity
        float elapsed = 0f;
        float startValue = chromaticAberration.intensity.value;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            chromaticAberration.intensity.value = Mathf.Lerp(
                startValue, maxIntensity, elapsed / fadeInDuration
            );
            yield return null;
        }

        // Phase 2 — pulse continuously while warning is active
        while (isWarningActive)
        {
            float t = (Mathf.Sin(Time.time * pulseSpeed * Mathf.PI * 2f) + 1f) / 2f;
            chromaticAberration.intensity.value = Mathf.Lerp(minIntensity, maxIntensity, t);
            yield return null;
        }
    }

    private IEnumerator FadeOutRoutine()
    {
        float startValue = chromaticAberration.intensity.value;
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            chromaticAberration.intensity.value = Mathf.Lerp(
                startValue, 0f, elapsed / fadeOutDuration
            );
            yield return null;
        }

        chromaticAberration.intensity.value = 0f;
    }

    private void OnDestroy()
    {
        // Always reset the volume on scene exit — otherwise it stays aberrated in the editor
        if (chromaticAberration != null)
            chromaticAberration.intensity.value = 0f;

        if (Lander.Instance != null)
        {
            Lander.Instance.OnFuelLow -= OnFuelLow;
            Lander.Instance.OnFuelPicked -= OnFuelPicked;
        }
    }
}