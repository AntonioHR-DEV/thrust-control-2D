using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int score = 0;

    public int Score => score;

    private void Awake()
    {
        Instance = this;
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
}
