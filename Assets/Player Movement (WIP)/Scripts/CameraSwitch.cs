using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] private CinemachineCamera mainCam;
    [SerializeField] private CinemachineCamera aimCam;
    [SerializeField] private CinemachineInputAxisController axisController;
    [SerializeField] private Camera cam;
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject crosshairUI;
    [SerializeField] private PlayerControls input;

    private InputAction aim;
    private bool isAiming;
    private Transform yawTarget;
    private Transform pitchTarget;
    private AimCameraController aimCameraController;
    
    
    void Start()
    {
        aimCameraController = aimCam.GetComponent<AimCameraController>();
        
        axisController = mainCam.GetComponent<CinemachineInputAxisController>();
        
        input = new PlayerControls();
        input.Enable();
        aim = input.Player.Aim;
    }

    // Update is called once per frame
    void Update()
    {
        bool aimPressed = aim.IsPressed();
        player.isAiming = aimPressed;

        if (aimPressed && !isAiming)
        {
            EnterAiming();
        }
        else if (!aimPressed && isAiming)
        {
            ExitAiming();
        }
    }

    private void ExitAiming()
    {
        isAiming = false;

        SnapMainCamBehind();
        
        aimCam.Priority = 10;
        mainCam.Priority = 20;
        
        axisController.enabled = true;
    }

    private void SnapMainCamBehind()
    {
        CinemachineOrbitalFollow orbitalFollow = mainCam.GetComponent<CinemachineOrbitalFollow>();
        Vector3 forward = aimCam.transform.forward;
        float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
        
        orbitalFollow.HorizontalAxis.Value = angle;
    }

    private void SnapAimForward()
    {
        aimCameraController.SetYawPitchFromCameraFoward(mainCam.transform);
    }

    private void EnterAiming()
    {
        isAiming = true;
        
        SnapAimForward();
        
        aimCam.Priority = 20;
        mainCam.Priority = 10;

        axisController.enabled = false;
    }
}
