using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private InputActions inputActions;

    private void Awake()
    {
        Instance = this;
        inputActions = new InputActions();
        inputActions.Player.Enable();
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
}
