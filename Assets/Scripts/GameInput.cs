using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameInput : MonoBehaviour
{
    private const string PLAYER_PREFS_BINDINGS = "InputBindings";

    public event EventHandler OnPauseToggled;
    public event EventHandler OnBindingRebound;

    public static GameInput Instance { get; private set; }

    public enum Binding
    {
        Thrust,
        Rotate_Left,
        Rotate_Right,
        Pause
    }

    private bool isInGameScene = false;

    private InputActions inputActions;

    private void Awake()
    {
        Instance = this;
        inputActions = new InputActions();

        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
            inputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));

        if (SceneManager.GetActiveScene().name == SceneLoader.Scene.GameScene.ToString()) isInGameScene = true;

        if (isInGameScene)
            inputActions.Player.Enable();
    }

    private void Start()
    {
        inputActions.Player.TogglePause.performed += TogglePause_Performed;
    }

    private void TogglePause_Performed(InputAction.CallbackContext context)
    {
        OnPauseToggled?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        inputActions.Player.TogglePause.performed -= TogglePause_Performed;

        inputActions.Player.Disable();
        inputActions.Dispose();
    }

    public bool IsMovingUp()
    {
        return inputActions.Player.MoveUp.IsPressed();
    }

    public bool IsRotatingLeft()
    {
        return inputActions.Player.RotateLeft.IsPressed();
    }

    public bool IsRotatingRight()
    {
        return inputActions.Player.RotateRight.IsPressed();
    }

    public string GetBindingText(Binding binding)
    {
        switch (binding)
        {
            default:
            case Binding.Thrust:
                return inputActions.Player.MoveUp.bindings[0].ToDisplayString();
            case Binding.Rotate_Left:
                return inputActions.Player.RotateLeft.bindings[0].ToDisplayString();
            case Binding.Rotate_Right:
                return inputActions.Player.RotateRight.bindings[0].ToDisplayString();
            case Binding.Pause:
                return inputActions.Player.TogglePause.bindings[0].ToDisplayString();
        }
    }

    public void RebindBinding(Binding binding, Action onActionRebound)
    {

        if (isInGameScene) inputActions.Player.Disable();

        InputAction inputAction;
        int bindingIndex = 0;
        
        switch (binding)
        {
            default:
            case Binding.Thrust:
                inputAction = inputActions.Player.MoveUp;
                break;
            case Binding.Rotate_Left:
                inputAction = inputActions.Player.RotateLeft;
                break;
            case Binding.Rotate_Right:
                inputAction = inputActions.Player.RotateRight;
                break;
            case Binding.Pause:
                inputAction = inputActions.Player.TogglePause;
                break;
        }

        inputAction.PerformInteractiveRebinding(bindingIndex)
            .OnComplete(callback =>
            {
                callback.Dispose();
                if (isInGameScene) inputActions.Player.Enable();
                onActionRebound();

                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, inputActions.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();

                OnBindingRebound?.Invoke(this, EventArgs.Empty);
            })
            .Start();
    }
}
