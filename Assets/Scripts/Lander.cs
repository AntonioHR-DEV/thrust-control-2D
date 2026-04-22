using System;
using UnityEngine;


public class Lander : MonoBehaviour
{
    public event EventHandler OnLanded;
    public event EventHandler OnCrashed;

    public static Lander Instance { get; private set; }

    [SerializeField] private float thrustPower = 500f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float fuelAmountMax = 10f;
    [SerializeField] float fuelConsumptionSpeed = .5f;
    [SerializeField] private float landingSpeedThreshold = 4f;
    [SerializeField] private float landingAngleThreshold = 15f;
    [SerializeField] private float gravityScale = .7f;
    private float fuelAmount;
    private Rigidbody2D rb;

    public float FuelAmount => fuelAmount;
    public float FuelAmountMax => fuelAmountMax;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        fuelAmount = fuelAmountMax; // Start with full fuel
        rb.gravityScale = 0f; // Disable gravity at the start
    }

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying()) return;

        if(GameInput.Instance.IsRotatingLeft() ||
           GameInput.Instance.IsRotatingRight() ||
           GameInput.Instance.IsMovingUp())
        {
            if (rb.gravityScale == 0f)
            {
                rb.gravityScale = gravityScale;
            }

            // Consume fuel when thrusting or rotating
            fuelAmount -= fuelConsumptionSpeed * Time.deltaTime;
            if (fuelAmount < 0) fuelAmount = 0;
        }

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
                int speedScoreMax = 100;
                int angleScoreMax = 100;

                int landingSpeedScore = Mathf.RoundToInt(Mathf.Lerp(speedScoreMax, 0f, landingSpeed / landingSpeedThreshold));
                int landingAngleScore = Mathf.RoundToInt(Mathf.Lerp(angleScoreMax, 0f, landingAngle / landingAngleThreshold));

                int totalLandingScore = (landingSpeedScore + landingAngleScore) * landingPad.ScoreMultiplier;

                GameManager.Instance.AddScore(totalLandingScore);

                OnLanded?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                // Crash
                OnCrashed?.Invoke(this, EventArgs.Empty);
            }
        }
        else
        {
            // crashed on Terrain
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
