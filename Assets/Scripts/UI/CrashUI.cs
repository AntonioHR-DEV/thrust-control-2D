using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrashUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private TextMeshProUGUI crashReasonText;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button retryButton;

    private void Awake()
    {
        retryButton.onClick.AddListener(() =>
        {
            GameManager.Instance.RestartLevel();
        });

        mainMenuButton.onClick.AddListener(() =>
        {
            SceneLoader.LoadScene(SceneLoader.Scene.MainMenuScene);
        });
    }

    private void Start()
    {
        Hide();
        Lander.Instance.OnCrashed += Lander_OnCrashed;
    }

    private void Lander_OnCrashed(object sender, Lander.OnCrashedEventArgs e)
    {
        if (TimeUpUI.Instance.IsVisible()) return;

        Show();

        if (e.crashReason == Lander.CrashReason.BadLandingConditions)
        {
            crashReasonText.text = "Bad Landing\nConditions";
        }
        else if (e.crashReason == Lander.CrashReason.TerrainCollision)
        {
            crashReasonText.text = "Terrain\nCollision";
        }

        int crashSpeedInt = Mathf.RoundToInt(e.crashSpeed * 10);
        int crashAngleInt = Mathf.RoundToInt(e.crashAngle);
        statsText.text = $"{crashSpeedInt}\n{crashAngleInt}\n0";
    }

    private void Show()
    {
        gameObject.SetActive(true);
        retryButton.Select();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
