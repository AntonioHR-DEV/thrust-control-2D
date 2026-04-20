using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour
{
    [SerializeField] private float thrustPower = 500f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float fuelAmountMax = 10f;
    [SerializeField] float fuelConsumptionSpeed = .5f;
    private float fuelAmount;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        fuelAmount = fuelAmountMax; // Start with full fuel
    }

    private void Update()
    {
        // Rotate the lander
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            rb.AddTorque(rotationSpeed * Time.deltaTime);
        }
        else if (Keyboard.current.rightArrowKey.isPressed)
        {
            rb.AddTorque(-rotationSpeed * Time.deltaTime);
        }

        // Thrust the lander
        if (Keyboard.current.upArrowKey.isPressed)
        {
            if (fuelAmount <= 0) return;

            Vector2 thrustDirection = transform.up; // Thrust in the direction the lander is facing
            rb.AddForce(thrustDirection * thrustPower * Time.deltaTime);
            fuelAmount -= fuelConsumptionSpeed * Time.deltaTime;
        }

        Debug.Log(fuelAmount);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Coin coin))
        {
            // Collect the coin and destroy it
            GameManager.Instance.AddScore(coin.CoinValue);
            coin.DestroySelf();
        }

        if(other.TryGetComponent(out Fuel fuel))
        {
            // Collect the fuel and destroy it
            AddFuel(fuel.FuelAmount);
            fuel.DestroySelf();
        }
    }

    private void AddFuel(float fuelAmount)
    {
        this.fuelAmount += fuelAmount;
        if(this.fuelAmount > fuelAmountMax)
        {
            this.fuelAmount = fuelAmountMax;
        }
    }
}
