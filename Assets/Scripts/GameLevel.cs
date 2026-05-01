using Unity.VisualScripting;
using UnityEngine;

public class GameLevel : MonoBehaviour
{
    [SerializeField] private int levelIndex;
    [SerializeField] private Transform landerSpawnPoint;
    [SerializeField] private float timeLimit = 60f;
    [SerializeField] private Sprite thumbnail;
    [SerializeField] private Transform cameraStartTargetTransform;
    [SerializeField] private float zoomedOutOrthographicSize;

    public int LevelIndex => levelIndex;
    public Transform LanderSpawnPoint => landerSpawnPoint;
    public float TimeLimit => timeLimit;
    public Sprite Thumbnail => thumbnail;
    public Transform CameraStartTargetTransform => cameraStartTargetTransform;
    public float ZoomedOutOrthographicSize => zoomedOutOrthographicSize;

    public int TotalCoinValue()
    {
        int totalValue = 0;
        Coin[] coins = gameObject.GetComponentsInChildren<Coin>(false);
        foreach (Coin coin in coins)
        {
            totalValue += coin.CoinValue;
        }

        return totalValue;
    }
}
