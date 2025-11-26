using Unity.Cinemachine;
using UnityEngine;
using Events;
using UnityEngine.InputSystem;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] private CinemachineCamera mainCam;
    [SerializeField] private CinemachineCamera aimCam;
    [SerializeField] private CinemachineInputAxisController axisController;
    [SerializeField] private Camera cam;
    public PlayerController player;
    [SerializeField] private GameObject crosshairUI;
    [SerializeField] private PlayerControls input;

    [SerializeField] private StringPayloadEvent dialogueStartedEvent;
    [SerializeField] private StringPayloadEvent dialogueFinishedEvent;

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

    private void OnEnable()
    {
        dialogueStartedEvent.OnEventTriggered += HandleDialogueStarted;
        dialogueFinishedEvent.OnEventTriggered += HandleDialogueFinished;
    }
    private void OnDisable()
    {
        dialogueStartedEvent.OnEventTriggered -= HandleDialogueStarted;
        dialogueFinishedEvent.OnEventTriggered -= HandleDialogueFinished;
    }

    private void HandleDialogueStarted(string _unusedStr)
    {
        //Debug.Log("FOO");
        axisController.enabled = false;
        Cursor.visible = true;
    }
    private void HandleDialogueFinished(string _unusedStr)
    {
        //Debug.Log("BAR");
        axisController.enabled = true;
        Cursor.visible = false;
    }


    void Update()
    {
        bool aimPressed = aim.IsPressed();
        player.isAiming = aimPressed;

        if (aimPressed && !isAiming)
            EnterAiming();
        else if (!aimPressed && isAiming)
            ExitAiming();
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
        Quaternion yawOnly = Quaternion.Euler(0f, aimCam.transform.eulerAngles.y, 0f);
        float targetYaw = yawOnly.eulerAngles.y;

        orbitalFollow.HorizontalAxis.Value = targetYaw;
    }

    private void SnapAimForward() => aimCameraController.SetYawPitchFromCameraFoward(cam.transform);

    private void EnterAiming()
    {
        isAiming = true;
        
        SnapAimForward();
        
        aimCam.Priority = 20;
        mainCam.Priority = 10;

        axisController.enabled = false;
    }
}
