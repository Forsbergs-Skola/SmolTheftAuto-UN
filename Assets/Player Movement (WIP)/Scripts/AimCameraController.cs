using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimCameraController : MonoBehaviour
{
    [SerializeField] private Transform yawTarget;
    [SerializeField] private Transform pitchTarget;
    
    [SerializeField] private InputActionReference lookInput;
    [SerializeField] private InputActionReference shoulderSwapInput;

    [SerializeField] private float sensitivity = 0.05f;

    [SerializeField] private float pitchMin = -40f;
    [SerializeField] private float pitchMax = 80f;

    [SerializeField] private CinemachineThirdPersonFollow aimCamera;
    [SerializeField] private float shoulderSwapSpeed;

    private float yaw;
    private float pitch;
    private float targetCameraSide;


    private void Awake()
    {
        aimCamera = GetComponent<CinemachineThirdPersonFollow>();
        targetCameraSide = aimCamera.CameraSide;
    }
    
    void Start()
    {
        Vector3 angles = yawTarget.rotation.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        lookInput.asset.Enable();
    }

    private void OnEnable()
    {
        shoulderSwapInput.action.Enable();
        shoulderSwapInput.action.performed += OnSwitchShoulder;
    }

    private void onDisable()
    {
        shoulderSwapInput.action.Disable();
        shoulderSwapInput.action.performed -= OnSwitchShoulder;
    }

    private void OnSwitchShoulder(InputAction.CallbackContext obj)
    {
        targetCameraSide = aimCamera.CameraSide < 0.5f ? 1f : 0f;
    }
    
    void Update()
    {
        Vector2 look = lookInput.action.ReadValue<Vector2>();
        if(Mouse.current != null && Mouse.current.delta.IsActuated())
            look *= sensitivity;
        
        yaw += look.x * sensitivity;
        pitch -= look.y * sensitivity;
        
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
        
        yawTarget.rotation = Quaternion.Euler(0f, yaw, 0f);
        pitchTarget.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        
        aimCamera.CameraSide = Mathf.Lerp(aimCamera.CameraSide, targetCameraSide, Time.deltaTime * shoulderSwapSpeed);
    }

    public void SetYawPitchFromCameraFoward(Transform camTransform) //Align the cameras so there isn't any weird snapping or misalignment
    {
        Vector3 forward = camTransform.forward;
        Vector3 forwardHorizontal = new Vector3(forward.x, 0f, forward.z); //remove vertical so its just hori for yaw
    
        if (forwardHorizontal.sqrMagnitude > 0.001f) //Only updating if camera isn't looking straight up or down
            yaw = Quaternion.LookRotation(forwardHorizontal).eulerAngles.y;
        
        pitch = camTransform.eulerAngles.x;
        
        if (pitch > 180f)
            pitch -= 360f;
        
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
        yawTarget.rotation = Quaternion.Euler(0f, yaw, 0f);
        pitchTarget.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}
