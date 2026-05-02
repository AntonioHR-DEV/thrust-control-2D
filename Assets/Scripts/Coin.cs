using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int coinValue = 100;
    [SerializeField] private ParticleSystem pickupParticleSystem;

    private bool isCollected = false;

    public int CoinValue
    {
        get => coinValue;
    }

    public void Collect()
    {
        if (isCollected) return;

        isCollected = true;
        GameManager.Instance.AddScore(coinValue);

        // Detach particles so they persist after GO is destroyed
        if (pickupParticleSystem != null)
        {
            pickupParticleSystem.transform.SetParent(null);
            pickupParticleSystem.Play();
        }

        Destroy(gameObject);
    }
}
