using Unity.Cinemachine;
using UnityEngine;

public class CinemachineCameraZoom2D : MonoBehaviour
{
    private const float NORMAL_ORTHOGRAPHIC_SIZE = 8.0f;
    public static CinemachineCameraZoom2D Instance { get; private set; }
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private float zoomSpeed = 2f;
    private float targetOrthographicSize = 8.0f;

    public float TargetOrthographicSize
    {
        get => targetOrthographicSize;
        set => targetOrthographicSize = value;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        cinemachineCamera.Lens.OrthographicSize = Mathf.Lerp(cinemachineCamera.Lens.OrthographicSize, targetOrthographicSize, Time.deltaTime * zoomSpeed);   
    }
    
    public void SetNormalOrthographicSize()
    {
        targetOrthographicSize = NORMAL_ORTHOGRAPHIC_SIZE;
    }
}