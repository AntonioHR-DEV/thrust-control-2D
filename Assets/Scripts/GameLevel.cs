using UnityEngine;

public class GameLevel : MonoBehaviour
{
    [SerializeField] private int levelIndex;
    [SerializeField] private Transform landerSpawnPoint;

    public int LevelIndex => levelIndex;
    public Transform LanderSpawnPoint => landerSpawnPoint;
}
