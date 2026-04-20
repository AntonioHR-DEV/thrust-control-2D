using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int score = 0;

    private void Awake()
    {
        Instance = this;
    }
    public void AddScore(float point)
    {
        score += (int)point;
        Debug.Log("Score: " + score);
    }
}
