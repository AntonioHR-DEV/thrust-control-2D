using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event EventHandler OnScoreChanged;

    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        WaitingToStart,
        Playing,
        GameOver
    }

    [SerializeField] private int gameLevelIndex = 1;
    [SerializeField] private List<GameObject> gameLevelPrefabList;
    private GameState state;
    private int score = 0;
    private GameLevel currentGameLevel;
    private float levelTimer;

    public int Score => score;
    public int GameLevelIndex => gameLevelIndex;
    public GameLevel CurrentGameLevel => currentGameLevel;
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
    }

    private void Lander_OnLanded(object sender, EventArgs e)
    {
        state = GameState.GameOver;
        Debug.Log("Game Over! Lander landed successfully.");
    }

    private void Lander_OnCrashed(object sender, EventArgs e)
    {
        state = GameState.GameOver;
        Debug.Log("Game Over! Lander crashed.");
    }

    private void Update()
    {
        switch (state)
        {
            case GameState.WaitingToStart:
                if (GameInput.Instance.IsRotatingLeft() ||
                    GameInput.Instance.IsRotatingRight() ||
                    GameInput.Instance.IsMovingUp())
                {
                    state = GameState.Playing;
                }
                break;
            case GameState.Playing:
                levelTimer -= Time.deltaTime;
                if (levelTimer <= 0f)
                {
                    levelTimer = 0f;
                    state = GameState.GameOver;
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

    private void LoadLevel(int levelIndex)
    {
        GameObject gameLevelGameObject = Instantiate(GetGameLevelPrefab(levelIndex), Vector3.zero, Quaternion.identity);
        GameLevel gameLevel = gameLevelGameObject.GetComponent<GameLevel>();
        Lander.Instance.transform.position = gameLevel.LanderSpawnPoint.position;

        currentGameLevel = gameLevel;
    }

    private GameObject GetGameLevelPrefab(int levelIndex)
    {
        foreach (GameObject prefab in gameLevelPrefabList)
        {
            GameLevel gameLevel = prefab.GetComponent<GameLevel>();
            if (gameLevel != null && gameLevel.LevelIndex == levelIndex)
            {
                return prefab;
            }
        }

        Debug.LogError("Invalid level index: " + levelIndex);
        return null;
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
}
