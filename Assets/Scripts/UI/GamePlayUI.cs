using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image fuelBarFill;

    private void Start()
    {
        GameManager.Instance.OnScoreChanged += GameManager_OnScoreChanged;

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
        if (GameManager.Instance.IsGameOver() && GameManager.Instance.LevelTimer > 0)
        {
            UpdateTimerText();
        }
    }

    
    private void GameManager_OnScoreChanged(object sender, EventArgs e)
    {
        UpdateScoreText();
    }

    private void UpdateLevelText()
    {
        levelText.text = $"LEVEL {GameManager.Instance.GameLevelIndex}";
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
}
