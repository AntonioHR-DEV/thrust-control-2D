using UnityEngine;
using UnityEngine.UI;

public class TimeUpUI : MonoBehaviour
{
    public static TimeUpUI Instance { get; private set; }

    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button retryButton;

    private void Awake()
    {
        Instance = this;

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
        GameManager.Instance.OnTimeUp += GameManager_OnTimeUp;
    }

    public bool IsVisible()
    {
        return gameObject.activeSelf;
    }

    private void GameManager_OnTimeUp(object sender, System.EventArgs e)
    {
        Show();
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
