using Unity.VisualScripting;
using UnityEngine;

public class GameLevel : MonoBehaviour
{
    [SerializeField] private int levelIndex;
    [SerializeField] private Transform landerSpawnPoint;
    [SerializeField] private float timeLimit = 60f;

    public int LevelIndex => levelIndex;
    public Transform LanderSpawnPoint => landerSpawnPoint;
    public float TimeLimit => timeLimit;
}
