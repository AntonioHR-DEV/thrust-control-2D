using System;
using UnityEngine;


public class Lander : MonoBehaviour
{
    public event EventHandler<OnLandedEventArgs> OnLanded;
    public class OnLandedEventArgs : EventArgs
    {
        public float landingSpeed;
        public float landingAngle;
        public int landingScore;
    }
    public event EventHandler OnCrashed;

    public enum LanderState
    {
        WaitingToStart,
        Flying,
        Landed,
        Crashed
    }

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
    private LanderState state;

    public float FuelAmount => fuelAmount;
    public float FuelAmountMax => fuelAmountMax;
    public LanderState State => state;

    private void Awake()
    {
        Instance = this;
        state = LanderState.WaitingToStart;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        fuelAmount = fuelAmountMax; // Start with full fuel
        rb.gravityScale = 0f; // Disable gravity at the start
    }

    private void Update()
    {
        switch (state)
        {
            case LanderState.WaitingToStart:
                if (GameInput.Instance.IsRotatingLeft() ||
                    GameInput.Instance.IsRotatingRight() ||
                    GameInput.Instance.IsMovingUp())
                {
                    state = LanderState.Flying;
                    rb.gravityScale = gravityScale;
                }
                break;
            case LanderState.Flying:
                HandleInput();
                break;
            case LanderState.Landed:
                break;
            case LanderState.Crashed:
                break;
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
                int landingSpeedScoreMax = 100;
                int landingAngleScoreMax = 100;

                int landingSpeedScore = Mathf.RoundToInt(Mathf.Lerp(landingSpeedScoreMax, 0f, landingSpeed / landingSpeedThreshold));
                int landingAngleScore = Mathf.RoundToInt(Mathf.Lerp(landingAngleScoreMax, 0f, landingAngle / landingAngleThreshold));

                int totalLandingScore = (landingSpeedScore + landingAngleScore) * landingPad.ScoreMultiplier;

                state = LanderState.Landed;
                OnLanded?.Invoke(this, new OnLandedEventArgs
                {
                    landingSpeed = landingSpeed,
                    landingAngle = landingAngle,
                    landingScore = totalLandingScore
                });
            }
            else
            {
                // Crashed because of bad landing conditions
                state = LanderState.Crashed;
                OnCrashed?.Invoke(this, EventArgs.Empty);
            }
        }
        else
        {
            // crashed on Terrain
            state = LanderState.Crashed;
            OnCrashed?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsThrusting()
    {
        if (fuelAmount <= 0) return false;
        return GameInput.Instance.IsMovingUp();
    }

    public bool HasLanded()
    {
        return state == LanderState.Landed;
    }

    private void HandleInput(){
        if(GameInput.Instance.IsRotatingLeft() ||
           GameInput.Instance.IsRotatingRight() ||
           GameInput.Instance.IsMovingUp())
        {
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

    private void AddFuel(float fuelAmount)
    {
        this.fuelAmount += fuelAmount;
        if(this.fuelAmount > fuelAmountMax)
        {
            this.fuelAmount = fuelAmountMax;
        }
    }
}
