using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private const string PLAYER_PREFS_MUSIC_VOLUME_KEY = "MusicVolume";

    public static MusicManager Instance { get; private set; }

    private static float musicVolume = 0.5f;

    [SerializeField] private AudioClip musicClip;
    private AudioSource audioSource;

    public static float MusicVolume
    {
        get => musicVolume;
        set
        {
            musicVolume = Mathf.Clamp01(value);
            if (Instance != null)
                Instance.audioSource.volume = musicVolume;
                
            PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME_KEY, musicVolume);
            PlayerPrefs.Save();
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        if (PlayerPrefs.HasKey(PLAYER_PREFS_MUSIC_VOLUME_KEY))
        {
            musicVolume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME_KEY);
        }

        audioSource.volume = musicVolume;
        audioSource.Play();
    }
}
