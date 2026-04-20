using Unity.VisualScripting;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private float coinValue = 100f;

    public float CoinValue
    {
        get => coinValue;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
