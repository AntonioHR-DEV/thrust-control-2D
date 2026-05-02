using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayUI : MonoBehaviour
{
    public static GamePlayUI Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image fuelBarFill;
    [SerializeField] private Color fuelBarNormalColor;
    [SerializeField] private Color fuelBarLowColor;

    private void Awake()
    {
        Instance = this;
    } 

    private void Start()
    {
        GameManager.Instance.OnScoreChanged += GameManager_OnScoreChanged;
        Lander.Instance.OnFuelLow += Lander_OnFuelLow;
        Lander.Instance.OnFuelPicked += Lander_OnFuelPicked;

        fuelBarFill.color = fuelBarNormalColor;

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
        fuelBarFill.fillAmount = Lander.Instance.FuelAmount / Lander.Instance.FuelAmountMax;
    }

    private void Lander_OnFuelLow(object sender, EventArgs e)
    {
        if (fuelBarFill.color == fuelBarLowColor) return;
        fuelBarFill.color = fuelBarLowColor;
    }

    private void Lander_OnFuelPicked(object sender, EventArgs e)
    {
        if (fuelBarFill.color == fuelBarNormalColor) return;
        fuelBarFill.color = fuelBarNormalColor;
    }
}
