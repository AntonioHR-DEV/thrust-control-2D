using UnityEngine;

public class Fuel : MonoBehaviour
{
    [SerializeField] private float fuelAmount = 10f;
    [SerializeField] private ParticleSystem pickupParticleSystem;

    public float FuelAmount
    {
        get => fuelAmount;
    }

    public void Collect()
    {
        // Detach particles so they persist after GO is destroyed
        if (pickupParticleSystem != null)
        {
            pickupParticleSystem.transform.SetParent(null);
            pickupParticleSystem.Play();
        }
        
        Destroy(gameObject);
    }
}
