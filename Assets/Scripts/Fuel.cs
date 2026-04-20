using UnityEngine;

public class Fuel : MonoBehaviour
{
    [SerializeField] private float fuelAmount = 10f;

    public float FuelAmount
    {
        get => fuelAmount;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
