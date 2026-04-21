using Unity.VisualScripting;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int coinValue = 100;

    public int CoinValue
    {
        get => coinValue;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
