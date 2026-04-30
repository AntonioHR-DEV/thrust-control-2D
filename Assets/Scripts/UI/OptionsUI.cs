using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }

    [SerializeField] private Button closeButton;
    [SerializeField] private Slider soundEffectsSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Button thrustButton;
    [SerializeField] private Button rotateLeftButton;
    [SerializeField] private Button rotateRightButton;
    [SerializeField] private Button pauseButton;

    [SerializeField] private TextMeshProUGUI thrustButtonText;
    [SerializeField] private TextMeshProUGUI rotateLeftButtonText;
    [SerializeField] private TextMeshProUGUI rotateRightButtonText;
    [SerializeField] private TextMeshProUGUI pauseButtonText;

    [SerializeField] private GameObject pressKeyToRebindGameObject;
    [SerializeField] private TextMeshProUGUI actionToRebindText;

    private Action onCloseAction;

    private void Awake()
    {
        Instance = this;

        closeButton.onClick.AddListener(() =>
        {
            Hide();
            onCloseAction?.Invoke();
        });

        soundEffectsSlider.onValueChanged.AddListener(value =>
        {
            SoundManager.ChangeSoundEffectsVolume(value);
        });

        musicSlider.onValueChanged.AddListener(value =>
        {
            // TODO
        });

        thrustButton.onClick.AddListener(() => { PerformRebinding(GameInput.Binding.Thrust); });
        rotateLeftButton.onClick.AddListener(() => { PerformRebinding(GameInput.Binding.Rotate_Left); });
        rotateRightButton.onClick.AddListener(() => { PerformRebinding(GameInput.Binding.Rotate_Right); });
        pauseButton.onClick.AddListener(() => { PerformRebinding(GameInput.Binding.Pause); });
    }

    private void Start()
    {
        UpdateVisual();
        pressKeyToRebindGameObject.SetActive(false);
        Hide();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameUnpaused += GameManager_OnGameUnpaused;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameUnpaused -= GameManager_OnGameUnpaused;
        }
    }

    public void Show(Action onCloseAction = null)
    {
        this.onCloseAction = onCloseAction;

        gameObject.SetActive(true);
        soundEffectsSlider.value = SoundManager.SoundVolume;
        closeButton.Select();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void GameManager_OnGameUnpaused(object sender, EventArgs e)
    {
        Hide();
    }

    private void UpdateVisual()
    {
        if (GameInput.Instance == null) return;

        thrustButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Thrust);
        rotateLeftButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Rotate_Left);
        rotateRightButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Rotate_Right);
        pauseButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
    }

    private void PerformRebinding(GameInput.Binding binding)
    {
        pressKeyToRebindGameObject.SetActive(true);

        actionToRebindText.text = binding.ToString();

        GameInput.Instance.RebindBinding(binding, () =>
        {
            pressKeyToRebindGameObject.SetActive(false);
            UpdateVisual();
        });
    }
}
