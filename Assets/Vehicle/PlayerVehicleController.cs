using UnityEngine;
using Unity.Cinemachine;
using StateMachine;
using Events;
using Vehicles;

public class PlayerVehicleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private PlayerController playerController;    // Euan's main player controller script  
    [SerializeField] private Animator animator;                    // same one PlayerController uses to move  drive animations

    [Header("State Machine")]
    [Tooltip("The main player state machine asset.")]
    [SerializeField] private SimpleStateMachineSO playerStateMachine;

    [Tooltip("State asset that represents 'driving a vehicle'.")]
    [SerializeField] private StateSO drivingState;

    [Header("Transition Event Strings")]
    [SerializeField] private string enterVehicleEventString = "EnterVehicle";
    [SerializeField] private string exitVehicleEventString  = "ExitVehicle";

    [Header("Input Channels (same assets used in InputHandler)")]
    [SerializeField] private Vector2PayloadEvent moveInputEvent;
    [SerializeField] private BoolPayloadEvent sprintInputEvent;
    [SerializeField] private EmptyPayloadEvent fireInputEvent;

    [Header("Cinemachine")]
[SerializeField] private CinemachineCamera onFootCamera;
[SerializeField] private CinemachineCamera drivingCamera;


    // Runtime state
    private bool isDriving = false;
    private VehicleMover currentVehicle;
    private Transform currentSeat; // where the player sits in the car

    // Cached input
    private float currentThrottle;
    private float currentSteer;
    private bool currentBrake;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (characterController == null) characterController = GetComponent<CharacterController>();
<<<<<<< Updated upstream
        if (playerController == null) playerController = GetComponent<PlayerController>();
    }

    private void OnEnable()
=======
        if (playerController == null)    playerController    = GetComponent<PlayerController>();

        controls = new PlayerControls();
        foreach (StateSO state in playerStateMachine.GetStates())
        {
            state.OnStateEntered += HandleOnStateEntered;   
        }
        
    }
 private void HandleOnStateEntered(StateData data)
    {
        Debug.Log("Entered State: " + data.StateName);
    }
    private void OnEnable() // subscribe to events
>>>>>>> Stashed changes
    {
        if (drivingState != null)
        {
            drivingState.OnStateEntered += OnDrivingEntered;
            drivingState.OnStateExited  += OnDrivingExited;
        }

        if (moveInputEvent != null)
            moveInputEvent.OnEventTriggered += OnMoveInput;

        if (sprintInputEvent != null)
            sprintInputEvent.OnEventTriggered += OnSprintInput;

        if (fireInputEvent != null)
            fireInputEvent.OnEventTriggered += OnFireInput;
    }

    private void OnDisable()
    {
        if (drivingState != null)
        {
            drivingState.OnStateEntered -= OnDrivingEntered;
            drivingState.OnStateExited  -= OnDrivingExited;
        }

        if (moveInputEvent != null)
            moveInputEvent.OnEventTriggered -= OnMoveInput;

        if (sprintInputEvent != null)
            sprintInputEvent.OnEventTriggered -= OnSprintInput;

        if (fireInputEvent != null)
            fireInputEvent.OnEventTriggered -= OnFireInput;
    }

    private void Update()
    {
        if (!isDriving || currentVehicle == null) return;

        // pushing cached input into the car each frame
        currentVehicle.SetInput(currentThrottle, currentSteer, currentBrake);
    }

    // ---------- Public API (called by VehicleSeatInteraction) ----------

    public void RequestEnterVehicle(VehicleMover vehicle, Transform seatTransform)
    {
        if (vehicle == null || playerStateMachine == null) return;

        currentVehicle = vehicle;
        currentSeat    = seatTransform;

        // Ask the state machine to switch from ON_FOOT -> DRIVING
        playerStateMachine.TriggerTransition(enterVehicleEventString);
    }

    public void RequestExitVehicle()
    {
        if (playerStateMachine == null) return;

        playerStateMachine.TriggerTransition(exitVehicleEventString);
    }

    // ---------- State Machine Callbacks ----------

    private void OnDrivingEntered(StateData data)
    {
        isDriving = true;

        // snap player to seat & hide / freeze controller
        if (currentSeat != null)
        {
            transform.position = currentSeat.position;
            transform.rotation = currentSeat.rotation;
        }

        if (characterController != null) characterController.enabled = false;
        if (playerController != null)    playerController.enabled    = false;

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetBool("Sprinting", false);
        }

        // switch cameras
        if (onFootCamera != null)  onFootCamera.Priority = 0;
        if (drivingCamera != null) drivingCamera.Priority = 10;
    }

    private void OnDrivingExited(StateData data)
    {
        isDriving = false;

        // basic exit logic: step a bit to the left of the seat
        if (currentSeat != null)
        {
            Vector3 exitPos = currentSeat.position + currentSeat.right * -1f;
            transform.position = exitPos;
            transform.rotation = currentSeat.rotation;
        }

        if (characterController != null) characterController.enabled = true;
        if (playerController != null)    playerController.enabled    = true;

        // Clear car + input
        currentVehicle?.SetInput(0f, 0f, false);
        currentVehicle = null;
        currentSeat    = null;
        currentThrottle = currentSteer = 0f;
        currentBrake = false;

        // switch cameras back
        if (onFootCamera != null)  onFootCamera.Priority = 10;
        if (drivingCamera != null) drivingCamera.Priority = 0;
    }

    // ---------- Input Callbacks ----------

    private void OnMoveInput(Vector2 move)
    {
        if (!isDriving) return;

        // W/S = throttle (vertical), A/D = steering (horizontal)
        currentThrottle = move.y;
        currentSteer    = move.x;
    }

    private void OnSprintInput(bool sprintHeld)
    {
        if (!isDriving) return;

        // treat Sprint as brake / handbrake for now
        currentBrake = sprintHeld;
    }

    private void OnFireInput()
    {
        if (!isDriving) return;

        // horn / shoot from car
        Debug.Log("Vehicle fire/horn pressed");
    }
}
