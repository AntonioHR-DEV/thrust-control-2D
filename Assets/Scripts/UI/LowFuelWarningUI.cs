using System;
using System.Collections;
using UnityEngine;

public class LowFuelWarningUI : MonoBehaviour
{
    [Header("Pulse Settings")]
    [SerializeField] private float pulseSpeed = 2f;      // full cycles per second
    [SerializeField] private float minAlpha = 0f;
    [SerializeField] private float maxAlpha = 1f;

    private CanvasGroup canvasGroup;

    private bool isWarningActive = false;
    private Coroutine pulseCoroutine;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        canvasGroup.alpha = 0f;
        gameObject.SetActive(true); // keep active so coroutine works (CanvasGroup hides it visually)

        Lander.Instance.OnFuelLow += OnFuelLow;
        Lander.Instance.OnFuelPicked += OnFuelPicked; // hide when refueled
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void OnFuelLow(object sender, EventArgs e)
    {
        if (isWarningActive) return;
        isWarningActive = true;

        if (pulseCoroutine != null) StopCoroutine(pulseCoroutine);
        pulseCoroutine = StartCoroutine(PulseRoutine());
    }

    private void OnFuelPicked(object sender, EventArgs e)
    {
        if (!isWarningActive) return;
        isWarningActive = false;

        if (pulseCoroutine != null) StopCoroutine(pulseCoroutine);
        StartCoroutine(FadeOutRoutine());
    }

    private void GameManager_OnGameStateChanged(object sender, EventArgs e)
    {
        if (!isWarningActive && GameManager.Instance.State != GameManager.GameState.GameOver) return;
        isWarningActive = false;
        if (pulseCoroutine != null) StopCoroutine(pulseCoroutine);
        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator PulseRoutine()
    {
        while (isWarningActive)
        {
            // Use a sine wave for smooth pulsing
            float t = (Mathf.Sin(Time.time * pulseSpeed * Mathf.PI * 2f) + 1f) / 2f;
            canvasGroup.alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
            yield return null;
        }
    }

    private IEnumerator FadeOutRoutine()
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;
        float fadeDuration = 0.5f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }

    private void OnDestroy()
    {
        if (Lander.Instance != null)
        {
            Lander.Instance.OnFuelLow -= OnFuelLow;
            Lander.Instance.OnFuelPicked -= OnFuelPicked;
        }
    }
}