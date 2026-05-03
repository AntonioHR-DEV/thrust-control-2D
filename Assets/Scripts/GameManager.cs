using System;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event EventHandler OnScoreChanged;
    public event EventHandler OnGameStateChanged;
    public event EventHandler OnTimeUp;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;

    public static GameManager Instance { get; private set; }
    private static int gameLevelIndex = 1;
    public static int GameLevelIndex
    {
        get => gameLevelIndex;
        set => gameLevelIndex = value;
    }

    public enum GameState
    {
        WaitingToStart,
        Playing,
        GameOver
    }

    [SerializeField] private LevelsListSO levelsListSO;
    [SerializeField] private CinemachineCamera cinemachineCamera;
    private GameState state;
    private int score = 0;
    private GameLevel currentGameLevel;
    private float levelTimer;

    public int Score => score;
    public float LevelTimer => levelTimer;
    public GameState State => state;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadLevel(gameLevelIndex);
        if (currentGameLevel != null)
        {
            levelTimer = currentGameLevel.TimeLimit;
        }

        state = GameState.WaitingToStart;

        Lander.Instance.OnLanded += Lander_OnLanded;
        Lander.Instance.OnCrashed += Lander_OnCrashed;
        Lander.Instance.OnStateChanged += Lander_OnStateChanged;
        GameInput.Instance.OnPauseToggled += GameInput_OnPauseToggled;
    }

    private void Update()
    {
        switch (state)
        {
            case GameState.WaitingToStart:
                if (GameInput.Instance.IsRotatingLeft() ||
                    GameInput.Instance.IsRotatingRight() ||
                    GameInput.Instance.IsThrusting())
                {
                    state = GameState.Playing;
                    OnGameStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case GameState.Playing:
                levelTimer -= Time.deltaTime;
                if (levelTimer <= 0f)
                {
                    levelTimer = 0f;
                    state = GameState.GameOver;
                    OnGameStateChanged?.Invoke(this, EventArgs.Empty);
                    OnTimeUp?.Invoke(this, EventArgs.Empty);
                }
                break;
            case GameState.GameOver:
                break;
        }
    }

    public void AddScore(int point)
    {
        score += point;
        OnScoreChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsWaitingToStart()
    {
        return state == GameState.WaitingToStart;
    }

    public bool IsPlaying()
    {
        return state == GameState.Playing;
    }

    public bool IsGameOver()
    {
        return state == GameState.GameOver;
    }

    public int CalculateGamePlayRating(int levelIndex, int score, int timeBonus, int landingScore)
    {
        int starCount = 0;
        GameLevel gameLevel = GetGameLevelPrefab(levelIndex).GetComponent<GameLevel>();

        // Check if has collected all coins
        if (score - timeBonus - landingScore >= gameLevel.TotalCoinValue())
        {
            starCount++;
        }
        // Check if has a perfect landing
        int perfectLandingScore = 140;
        if (landingScore >= perfectLandingScore)
        {
            starCount++;
        }
        // Check if has not used more than 80% of the time
        if (timeBonus >= gameLevel.TimeLimit * 0.2f)
        {
            starCount++;
        }

        return starCount;
    }

    public void RestartLevel()
    {
        SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
    }

    public void LoadNextLevel()
    {
        GameLevelIndex++;
        if (!CheckLevelExistance(GameLevelIndex)) return;

        SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
    }

    public void TogglePause()
    {
        if (state == GameState.GameOver) return;

        if (Time.timeScale > 0)
        {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool CheckLevelExistance(int levelIndex)
    {
        foreach (GameLevel gameLevel in levelsListSO.GameLevelsList)
        {
            if (gameLevel.LevelIndex == levelIndex)
            {
                return true;
            }
        }

        return false;
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        state = GameState.GameOver;

        OnScoreChanged?.Invoke(this, EventArgs.Empty);
        OnGameStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Lander_OnCrashed(object sender, Lander.OnCrashedEventArgs e)
    {
        state = GameState.GameOver;
        OnGameStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Lander_OnStateChanged(object sender, EventArgs e)
    {
        if (Lander.Instance.State == Lander.LanderState.Flying)
        {
            cinemachineCamera.Target.TrackingTarget = Lander.Instance.transform;
            CinemachineCameraZoom2D.Instance.SetNormalOrthographicSize();

            GamePlayUI.Instance.gameObject.SetActive(true);
        }
    }

    private void GameInput_OnPauseToggled(object sender, EventArgs e)
    {
        TogglePause();
    }

    private void LoadLevel(int levelIndex)
    {
        GameObject gameLevelGameObject = Instantiate(GetGameLevelPrefab(levelIndex), Vector3.zero, Quaternion.identity);
        GameLevel gameLevel = gameLevelGameObject.GetComponent<GameLevel>();
        Lander.Instance.transform.position = gameLevel.LanderSpawnPoint.position;

        currentGameLevel = gameLevel;

        cinemachineCamera.Target.TrackingTarget = gameLevel.CameraStartTargetTransform;
        CinemachineCameraZoom2D.Instance.TargetOrthographicSize = gameLevel.ZoomedOutOrthographicSize;
        GamePlayUI.Instance.gameObject.SetActive(false);
    }

    private GameObject GetGameLevelPrefab(int levelIndex)
    {
        foreach (GameLevel gameLevel in levelsListSO.GameLevelsList)
        {
            if (gameLevel.LevelIndex == levelIndex)
            {
                return gameLevel.gameObject;
            }
        }

        Debug.LogError("Invalid level index: " + levelIndex);
        return null;
    }
}
