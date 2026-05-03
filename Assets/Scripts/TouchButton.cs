using UnityEngine;
using UnityEngine.EventSystems;

public class TouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public enum ActionType { Thrust, RotateLeft, RotateRight }

    [SerializeField] private ActionType action;

    [Header("Visual Feedback")]
    [SerializeField] private float normalAlpha = 0.4f;
    [SerializeField] private float pressedAlpha = 1f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = normalAlpha;
    }

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying() || Application.isMobilePlatform) return;

        switch (action)
        {
            case ActionType.Thrust:
                canvasGroup.alpha = GameInput.Instance.IsThrusting() ? pressedAlpha : normalAlpha;
                break;
            case ActionType.RotateLeft:
                canvasGroup.alpha = GameInput.Instance.IsRotatingLeft() ? pressedAlpha : normalAlpha;
                break;
            case ActionType.RotateRight:
                canvasGroup.alpha = GameInput.Instance.IsRotatingRight() ? pressedAlpha : normalAlpha;
                break;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetHeld(true);
        canvasGroup.alpha = pressedAlpha;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SetHeld(false);
        canvasGroup.alpha = normalAlpha;
    }

    // Finger slides off the button — treat as release
    public void OnPointerExit(PointerEventData eventData)
    {
        SetHeld(false);
        canvasGroup.alpha = normalAlpha;
    }

    private void SetHeld(bool held)
    {
        switch (action)
        {
            case ActionType.Thrust:
                GameInput.Instance.SetTouchThrust(held);
                break;
            case ActionType.RotateLeft:
                GameInput.Instance.SetTouchRotateLeft(held);
                break;
            case ActionType.RotateRight:
                GameInput.Instance.SetTouchRotateRight(held);
                break;
        }
    }
}