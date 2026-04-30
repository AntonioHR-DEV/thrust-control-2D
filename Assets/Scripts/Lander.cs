using System;
using System.Collections;
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
    public event EventHandler<OnCrashedEventArgs> OnCrashed;
    public class OnCrashedEventArgs : EventArgs
    {
        public CrashReason crashReason;
        public float crashSpeed;
        public float crashAngle;
    }

    public event EventHandler OnCoinPicked;
    public event EventHandler OnFuelPicked;
    public event EventHandler OnForceFieldEntered;
    public event EventHandler OnFuelLow;
    public event EventHandler OnThrust;

    public enum LanderState
    {
        WaitingToStart,
        Flying,
        Landing,
        Landed,
        Crashed
    }

    public enum CrashReason
    {
        None,
        BadLandingConditions,
        TerrainCollision,
        HitByLaser
    }

    public static Lander Instance { get; private set; }

    [SerializeField] private float thrustPower = 500f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float fuelAmountMax = 10f;
    [SerializeField] float fuelConsumptionSpeed = .5f;
    [SerializeField] float lowFuelThreshold = 2f;
    [SerializeField] private float landingSpeedThreshold = 4f;
    [SerializeField] private float landingAngleThreshold = 15f;
    [SerializeField] private float gravityScale = .7f;
    private float fuelAmount;
    private Rigidbody2D rb;
    private LanderState state;
    private bool isHandlingLanding = false;
    private float collisionSpeed;
    private float collisionAngle;

    public float FuelAmount => fuelAmount;
    public float FuelAmountMax => fuelAmountMax;
    public LanderState State => state;

    private void Awake()
    {
        Instance = this;

        rb = GetComponent<Rigidbody2D>();

        state = LanderState.WaitingToStart;
        fuelAmount = fuelAmountMax;
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
                if (fuelAmount < lowFuelThreshold && fuelAmount > 0f)
                {
                    OnFuelLow?.Invoke(this, EventArgs.Empty);
                }
                break;
            case LanderState.Landing:
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

            OnCoinPicked?.Invoke(this, EventArgs.Empty);
        }

        if (other.TryGetComponent(out Fuel fuel))
        {
            // Collect the fuel and destroy it
            AddFuel(fuel.FuelAmount);
            fuel.DestroySelf();

            OnFuelPicked?.Invoke(this, EventArgs.Empty);
        }

        if (other.TryGetComponent(out LaserBeam _))
        {
            // Trigger crash on the lander
            HandleCrash(CrashReason.HitByLaser);
        }

        if (other.TryGetComponent(out ForceField _))
        {
            OnForceFieldEntered?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isHandlingLanding) return;

        collisionSpeed = collision.relativeVelocity.magnitude;
        collisionAngle = Vector2.Angle(transform.up, Vector2.up);

        if (collision.collider.TryGetComponent(out LandingPad landingPad))
        {
            if (collisionSpeed <= landingSpeedThreshold && collisionAngle <= landingAngleThreshold)
            {
                isHandlingLanding = true;
                StartCoroutine(HandleLandingPadCollision(landingPad));
            }
            else
            {
                // Crashed because of bad landing conditions
                HandleCrash(CrashReason.BadLandingConditions);
            }
        }
        else
        {
            // crashed on Terrain
            HandleCrash(CrashReason.TerrainCollision);
        }
    }

    private IEnumerator HandleLandingPadCollision(LandingPad landingPad)
    {
        state = LanderState.Landing;

        // wait for the lander to be stable on the landing pad for 1 second before confirming the landing
        float stableTime = 1f;
        float timer = 0f;
        float angleThreshold = 45f; // If the lander tilts more than this angle during landing, consider it a crash
        while (timer < stableTime && state == LanderState.Landing)
        {
            if (Vector2.Angle(transform.up, Vector2.up) > angleThreshold)
            {
                // If the lander is tilted too much, consider it a crash
                HandleCrash(CrashReason.BadLandingConditions);
                isHandlingLanding = false;
                yield break;
            }

            if (rb.linearVelocity.magnitude > 0.05f || Mathf.Abs(rb.angularVelocity) > 0.05f)
            {
                // If the lander is not stable, reset the timer
                timer = 0f;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        // Successful landing
        HandleSuccessfulLanding(landingPad);
    }

    public bool IsThrusting()
    {
        if (fuelAmount <= 0) return false;
        return GameManager.Instance.IsPlaying() && GameInput.Instance.IsMovingUp() && state == LanderState.Flying;
    }

    public bool HasLanded()
    {
        return state == LanderState.Landed;
    }

    public void HandleCrash(CrashReason crashReason)
    {
        state = LanderState.Crashed;
        OnCrashed?.Invoke(this, new OnCrashedEventArgs
        {
            crashReason = crashReason,
            crashSpeed = collisionSpeed,
            crashAngle = collisionAngle
        });
    }

    private void HandleInput()
    {
        if (!GameManager.Instance.IsPlaying() || fuelAmount <= 0) return;

        if (GameInput.Instance.IsRotatingLeft() ||
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
            Vector2 thrustDirection = transform.up; // Thrust in the direction the lander is facing
            rb.AddForce(thrustDirection * thrustPower * Time.deltaTime);
            OnThrust?.Invoke(this, EventArgs.Empty);
        }
    }

    private void AddFuel(float fuelAmount)
    {
        this.fuelAmount += fuelAmount;
        if (this.fuelAmount > fuelAmountMax)
        {
            this.fuelAmount = fuelAmountMax;
        }
    }

    private void HandleSuccessfulLanding(LandingPad landingPad)
    {
        int landingSpeedScoreMax = 100;
        int landingAngleScoreMax = 100;

        int landingSpeedScore = Mathf.RoundToInt(Mathf.Lerp(landingSpeedScoreMax, 0f, collisionSpeed / landingSpeedThreshold));
        int landingAngleScore = Mathf.RoundToInt(Mathf.Lerp(landingAngleScoreMax, 0f, collisionAngle / landingAngleThreshold));

        int totalLandingScore = (landingSpeedScore + landingAngleScore) * landingPad.ScoreMultiplier;

        // Add landing score to total score
        GameManager.Instance.AddScore(totalLandingScore);
        // If there is still a time left, add the time to the score
        if (GameManager.Instance.LevelTimer > 0)
        {
            int timeBonus = Mathf.FloorToInt(GameManager.Instance.LevelTimer);
            GameManager.Instance.AddScore(timeBonus);
        }

        state = LanderState.Landed;
        OnLanded?.Invoke(this, new OnLandedEventArgs
        {
            landingSpeed = collisionSpeed,
            landingAngle = collisionAngle,
            landingScore = totalLandingScore
        });
    }
}
