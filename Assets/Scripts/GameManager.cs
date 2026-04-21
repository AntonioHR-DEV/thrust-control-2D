using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int gameLevelIndex = 1;
    [SerializeField] private List<GameObject> gameLevelPrefabList;
    private int score = 0;

    public int Score => score;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadLevel(gameLevelIndex);
    }

    public void AddScore(int point)
    {
        score += point;
        Debug.Log("Score: " + score);
    }

    public void MultiplyScore(int multiplier)
    {
        score *= multiplier;
    }

    private void LoadLevel(int levelIndex)
    {
        GameObject gameLevelGameObject = Instantiate(GetGameLevelPrefab(levelIndex), Vector3.zero, Quaternion.identity);
        GameLevel gameLevel = gameLevelGameObject.GetComponent<GameLevel>();
        Lander.Instance.transform.position = gameLevel.LanderSpawnPoint.position;
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
}
