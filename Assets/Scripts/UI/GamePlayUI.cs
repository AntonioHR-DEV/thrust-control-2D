using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayUI : MonoBehaviour
{
    public static GamePlayUI Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image fuelBarImage;

    [Header("Fuel Bar Pulse")]
    [SerializeField] private float pulseScale = 1.25f;
    [SerializeField] private float pulseDuration = 0.25f;
    [SerializeField] private int blinkCount = 3;
    [SerializeField] private float blinkInterval = 0.1f;

    [Header("Fuel Bar Colors")]
    [SerializeField] private Color normalColor = new Color(0.2f, 0.8f, 0.2f);
    [SerializeField] private Color fuelLowColor = new Color(1f, 0f, 0f);
    [SerializeField] private Color flashColor = new Color(1f, 1f, 0.2f);

    private RectTransform fuelBarRect;

    private void Awake()
    {
        Instance = this;
        fuelBarRect = fuelBarImage.rectTransform;
    }

    private void Start()
    {
        GameManager.Instance.OnScoreChanged += GameManager_OnScoreChanged;
        Lander.Instance.OnFuelLow += Lander_OnFuelLow;
        Lander.Instance.OnFuelPicked += Lander_OnFuelPicked;

        fuelBarImage.color = normalColor;

        UpdateLevelText();
        UpdateTimerText();
        UpdateScoreText();
        UpdateFuelBar();
    }

    private void Update()
    {
        if (GameManager.Instance.IsPlaying())
        {
            UpdateTimerText();
            UpdateFuelBar();
        }
    }

    public void PulseFuelBar()
    {
        StopCoroutine(nameof(PulseFuelBarRoutine)); // prevent stacking
        StartCoroutine(nameof(PulseFuelBarRoutine));
    }

    private IEnumerator PulseFuelBarRoutine()
    {
        // Run scale pulse and color blink in parallel
        StartCoroutine(ScalePulseRoutine());
        yield return StartCoroutine(ColorBlinkRoutine());
    }

    private IEnumerator ScalePulseRoutine()
    {
        Vector3 originalScale = fuelBarRect.localScale;
        Vector3 targetScale = originalScale * pulseScale;
        float half = pulseDuration / 2f;

        float elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            fuelBarRect.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / half);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            fuelBarRect.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / half);
            yield return null;
        }

        fuelBarRect.localScale = originalScale;
    }

    private IEnumerator ColorBlinkRoutine()
    {
        for (int i = 0; i < blinkCount; i++)
        {
            fuelBarImage.color = flashColor;
            yield return new WaitForSeconds(blinkInterval);
            fuelBarImage.color = normalColor;
            yield return new WaitForSeconds(blinkInterval);
        }

        // Ensure we always end on normal color
        fuelBarImage.color = normalColor;
    }

    private void GameManager_OnScoreChanged(object sender, EventArgs e)
    {
        UpdateScoreText();
    }

    private void UpdateLevelText()
    {
        levelText.text = $"LEVEL {GameManager.GameLevelIndex}";
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(GameManager.Instance.LevelTimer / 60);
        int seconds = Mathf.FloorToInt(GameManager.Instance.LevelTimer % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void UpdateScoreText()
    {
        scoreText.text = $"{GameManager.Instance.Score}";
    }

    private void UpdateFuelBar()
    {
        fuelBarImage.fillAmount = Lander.Instance.FuelAmount / Lander.Instance.FuelAmountMax;
    }

    private void Lander_OnFuelLow(object sender, EventArgs e)
    {
        if (fuelBarImage.color != fuelLowColor)
            fuelBarImage.color = fuelLowColor;
    }

    private void Lander_OnFuelPicked(object sender, EventArgs e)
    {
        if (fuelBarImage.color != normalColor)
            fuelBarImage.color = normalColor;
        PulseFuelBar();
    }
}
