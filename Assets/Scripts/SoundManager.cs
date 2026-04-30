using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private const string PLAYER_PREFS_SOUND_VOLUME_KEY = "SoundVolume";

    public event EventHandler OnVolumeChanged;

    public static SoundManager Instance { get; private set; }

    private static float soundVolume = .6f;
    private static bool soundVolumeLoaded = false;

    [SerializeField] private AudioClip coinPickupSound;
    [SerializeField] private AudioClip fuelPickupSound;
    [SerializeField] private AudioClip landingSuccessSound;
    [SerializeField] private AudioClip crashSound;
    [SerializeField] private AudioClip lockSwitchSound;
    [SerializeField] private AudioClip forceFieldSound;
    [SerializeField] private AudioClip warningSound;

    private AudioSource warningAudioSource;

    public static float SoundVolume
    {
        get
        {
            if (!soundVolumeLoaded)
            {
                if (PlayerPrefs.HasKey(PLAYER_PREFS_SOUND_VOLUME_KEY))
                {
                    soundVolume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_VOLUME_KEY);
                }

                soundVolumeLoaded = true;
            }

            return soundVolume;
        }
        set
        {
            soundVolume = Mathf.Clamp01(value);

            if (Instance != null)
            {
                Instance.warningAudioSource.volume = soundVolume;
                Instance.OnVolumeChanged?.Invoke(Instance, EventArgs.Empty);
            }

            PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_VOLUME_KEY, soundVolume);
            PlayerPrefs.Save();
        }
    }

    private void Awake()
    {
        Instance = this;

        if (PlayerPrefs.HasKey(PLAYER_PREFS_SOUND_VOLUME_KEY))
        {
            soundVolume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_VOLUME_KEY);
        }

        soundVolumeLoaded = true;

        AddWarningAudioSource();
    }

    private void Start()
    {
        Lander.Instance.OnCoinPicked += Lander_OnCoinPicked;
        Lander.Instance.OnFuelPicked += Lander_OnFuelPicked;
        Lander.Instance.OnCrashed += Lander_OnCrashed;
        Lander.Instance.OnLanded += Lander_OnLanded;
        Lander.Instance.OnForceFieldEntered += Lander_OnForceFieldEntered;
        Lander.Instance.OnFuelLow += Lander_OnFuelLow;
        Lock.OnAnyLockSwitched += Lock_OnAnyLockSwitched;
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
        GameManager.Instance.OnGamePaused += GameManager_OnGamePaused;
    }

    private void OnDestroy()
    {
        Lock.OnAnyLockSwitched -= Lock_OnAnyLockSwitched;
    }

    private void AddWarningAudioSource()
    {
        warningAudioSource = gameObject.AddComponent<AudioSource>();
        warningAudioSource.clip = warningSound;
        warningAudioSource.volume = soundVolume;
        warningAudioSource.spatialBlend = 0.0f;
        warningAudioSource.playOnAwake = false;
    }

    private void Lander_OnCoinPicked(object sender, EventArgs e)
    {
        AudioSource.PlayClipAtPoint(coinPickupSound, Camera.main.transform.position, soundVolume);
    }

    private void Lander_OnFuelPicked(object sender, EventArgs e)
    {
        AudioSource.PlayClipAtPoint(fuelPickupSound, Camera.main.transform.position, soundVolume);

        if (warningAudioSource.isPlaying)
        {
            warningAudioSource.Stop();
        }
    }

    private void Lander_OnCrashed(object sender, Lander.OnCrashedEventArgs e)
    {
        AudioSource.PlayClipAtPoint(crashSound, Camera.main.transform.position, soundVolume);
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        AudioSource.PlayClipAtPoint(landingSuccessSound, Camera.main.transform.position, soundVolume);
    }

    private void Lander_OnForceFieldEntered(object sender, EventArgs e)
    {
        AudioSource.PlayClipAtPoint(forceFieldSound, Camera.main.transform.position, soundVolume);
    }

    private void Lander_OnFuelLow(object sender, EventArgs e)
    {
        if (!warningAudioSource.isPlaying)
        {
            warningAudioSource.Play();
        }
    }

    private void Lock_OnAnyLockSwitched(object sender, EventArgs e)
    {
        AudioSource.PlayClipAtPoint(lockSwitchSound, Camera.main.transform.position, soundVolume);
    }

    private void GameManager_OnGameStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.State == GameManager.GameState.GameOver)
        {
            if (warningAudioSource.isPlaying)
            {
                warningAudioSource.Stop();
            }
        }
    }

    private void GameManager_OnGamePaused(object sender, EventArgs e)
    {
        if (warningAudioSource.isPlaying)
        {
            warningAudioSource.Stop();
        }
    }
}
