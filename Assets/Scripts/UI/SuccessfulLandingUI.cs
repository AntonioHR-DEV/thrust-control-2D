using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SuccessfulLandingUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI landingStatsText;
    [SerializeField] private TextMeshProUGUI timeBonusText;
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private List<Image> starList;
    [SerializeField] private Sprite yellowStarSprite;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button continueButton;
    private float landingSpeed;
    private float landingAngle;
    private int landingScore;
    private int starCount;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            SceneLoader.LoadScene(SceneLoader.Scene.MainMenuScene);
        });

        continueButton.onClick.AddListener(() =>
        {
            GameManager.Instance.LoadNextLevel();
        });
    }

    private void Start()
    {
        // Hide the continue button if there is no following/next level
        continueButton.gameObject.SetActive(GameManager.Instance.CheckLevelExistance(GameManager.GameLevelIndex + 1));

        Hide();
        Lander.Instance.OnLanded += Lander_OnLanded;
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        landingSpeed = e.landingSpeed;
        landingAngle = e.landingAngle;
        landingScore = e.landingScore;

        Show();
        UpdateLandingStatsText();
        UpdateTotalScoreText();
        UpdateTimeBonusText();
        UpdateStarRating();
    }

    private void Show()
    {
        gameObject.SetActive(true);

        if (continueButton.gameObject.activeInHierarchy)
        {
            continueButton.Select();
        }
        else
        {
            mainMenuButton.Select();
        }
        
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void UpdateLandingStatsText()
    {
        landingStatsText.text =
            Mathf.RoundToInt(landingSpeed * 10) + "\n" +
            Mathf.RoundToInt(landingAngle) + "\n" +
            landingScore;
    }

    private void UpdateTotalScoreText()
    {
        totalScoreText.text = GameManager.Instance.Score.ToString();
    }

    private void UpdateTimeBonusText()
    {
        int timeBonus = Mathf.FloorToInt(GameManager.Instance.LevelTimer);
        timeBonusText.text = $"Time Bonus: <voffset=-5><size=150%>+{timeBonus}</size></voffset>";
    }

    private void UpdateStarRating()
    {
        starCount = GameManager.Instance.CalculateGamePlayRating(GameManager.GameLevelIndex, GameManager.Instance.Score, Mathf.FloorToInt(GameManager.Instance.LevelTimer), landingScore);

        for (int i = 0; i < starList.Count; i++)
        {
            if (i < starCount)
            {
                starList[i].sprite = yellowStarSprite;
                starList[i].color = Color.white;
            }
        }
    }
}
