using System;
using UnityEngine;


public class Lander : MonoBehaviour
{
    public event EventHandler OnCrashed;

    [SerializeField] private float thrustPower = 500f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float fuelAmountMax = 10f;
    [SerializeField] float fuelConsumptionSpeed = .5f;
    [SerializeField] private float landingSpeedThreshold = 4f;
    [SerializeField] private float landingAngleThreshold = 15f;
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
        if (GameInput.Instance.IsRotatingLeft())
        {
            rb.AddTorque(rotationSpeed * Time.deltaTime);
        }
        else if (GameInput.Instance.IsRotatingRight())
        {
            rb.AddTorque(-rotationSpeed * Time.deltaTime);
        }

        // Thrust the lander
        if (GameInput.Instance.IsMovingUp())
        {
            if (fuelAmount <= 0) return;

            Vector2 thrustDirection = transform.up; // Thrust in the direction the lander is facing
            rb.AddForce(thrustDirection * thrustPower * Time.deltaTime);
        }

        // Consume fuel when thrusting or rotating
        if(GameInput.Instance.IsRotatingLeft() ||
           GameInput.Instance.IsRotatingRight() ||
           GameInput.Instance.IsMovingUp())
        {
            fuelAmount -= fuelConsumptionSpeed * Time.deltaTime;
            if (fuelAmount < 0) fuelAmount = 0;
        }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.TryGetComponent(out LandingPad landingPad))
        {
            // Check landing conditions
            float landingSpeed = collision.relativeVelocity.magnitude;
            float landingAngle = Vector2.Angle(transform.up, Vector2.up);

            Debug.Log("Landing Speed: " + landingSpeed + ", Landing Angle: " + landingAngle);

            if (landingSpeed <= landingSpeedThreshold && landingAngle <= landingAngleThreshold)
            {
                // Successful landing
                GameManager.Instance.MultiplyScore(landingPad.ScoreMultiplier);
                Debug.Log("Successful Landing! Score Gained: " + GameManager.Instance.Score);
            }
            else
            {
                // Crash
                Debug.Log("Crashed!");
                OnCrashed?.Invoke(this, EventArgs.Empty);
            }
        }
        else
        {
            // crashed on Terrain
            Debug.Log("Crashed on Terrain!");
            OnCrashed?.Invoke(this, EventArgs.Empty);
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

    public bool IsThrusting()
    {
        if (fuelAmount <= 0) return false;
        return GameInput.Instance.IsMovingUp();
    }
}
