using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;   // <-- needed for InputAction & CallbackContext
using StateMachine;
using Events;
using Vehicles;

public class PlayerVehicleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private PlayerController playerController;    // Euan's main player controller script  
    [SerializeField] private Animator animator;                    // same one PlayerController uses to drive animations

    private PlayerControls controls;

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
    private VehicleMover currentVehicle; // the vehicle the player is currently driving
    private Transform currentSeat; // where the player sits in the car

    // Cached input
    private float currentThrottle;
    private float currentSteer;
    private bool currentBrake;

    private void Awake() // initialize references
    {
        if (animator == null)           animator = GetComponent<Animator>();
        if (characterController == null) characterController = GetComponent<CharacterController>();
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
=======
        if (playerController == null)    playerController    = GetComponent<PlayerController>();

        controls = new PlayerControls();
    }

    private void OnEnable() // subscribe to events

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

        if (controls == null)
            controls = new PlayerControls();

        controls.Enable();
        controls.Player.EnterVehicle.performed += OnEnterVehiclePressed;
        
    }

    private void OnDisable() // unsubscribe from events
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

        if (controls != null)
        {
            controls.Player.EnterVehicle.performed -= OnEnterVehiclePressed;
            controls.Disable();
        }
    }

    private void Update()// push input to vehicle each frame
    {
        if (!isDriving || currentVehicle == null) return;

        // push cached input into the car each frame
        currentVehicle.SetInput(currentThrottle, currentSteer, currentBrake);
    }

    

    private void TryInteractWithNearbyVehicle()// look for nearby vehicle to enter
    {
        // Search sphere around the player for a VehicleSeatInteraction
        Collider[] hits = Physics.OverlapSphere(transform.position, 2f);

        foreach (var hit in hits)
        {
            VehicleSeatInteraction seat = hit.GetComponent<VehicleSeatInteraction>();
            if (seat != null)
            {
                RequestEnterVehicle(seat.Vehicle, seat.SeatTransform);
                return;
            }
        }

        Debug.Log("No vehicle nearby to enter.");
    }

    public void RequestEnterVehicle(VehicleMover vehicle, Transform seatTransform) // request to enter a specific vehicle
    {
        if (vehicle == null || playerStateMachine == null) return;

        currentVehicle = vehicle;
        currentSeat    = seatTransform;

        // Ask the state machine to switch from ON_FOOT -> DRIVING
        playerStateMachine.TriggerTransition(enterVehicleEventString);
    }

    private void OnEnterVehiclePressed(InputAction.CallbackContext ctx)// handle enter/exit vehicle input
    {
        // Single E key behaviour:
        // If not driving: try to enter nearest vehicle
        //  If driving: exit current vehicle
        if (isDriving)
        {
            RequestExitVehicle();
        }
        else
        {
            TryInteractWithNearbyVehicle();
        }
    }

    public void RequestExitVehicle() // request to exit current vehicle
    {
        if (playerStateMachine == null) return;

        playerStateMachine.TriggerTransition(exitVehicleEventString);
    }

    

    private void OnDrivingEntered(StateData data) // called when player enters driving state
    {
        isDriving = true;

        // snap player to seat & disable player controller
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

    private void OnDrivingExited(StateData data) // called when player exits driving state
    {
        isDriving = false;

        // exit positioning to the left of the car
        if (currentSeat != null)
        {
            Vector3 exitPos = currentSeat.position + currentSeat.right * -1f;
            transform.position = exitPos;
            transform.rotation = currentSeat.rotation;
        }

        if (characterController != null) characterController.enabled = true;
        if (playerController != null)    playerController.enabled    = true;

        // Clear car input
        currentVehicle?.SetInput(0f, 0f, false);
        currentVehicle = null;
        currentSeat    = null;
        currentThrottle = currentSteer = 0f;
        currentBrake = false;

        // switching between camera
        if (onFootCamera != null)  onFootCamera.Priority = 10;
        if (drivingCamera != null) drivingCamera.Priority = 0;
    }

    

    private void OnMoveInput(Vector2 move) // handle move input for driving
    {
        if (!isDriving) return;

        // W/S = throttle , A/D = steering 
        currentThrottle = move.y;
        currentSteer    = move.x;
    }

    private void OnSprintInput(bool sprintHeld)
    {
        if (!isDriving) return;

        // treating Sprint as brake  for now later move this to spacebar maybe
        currentBrake = sprintHeld;
    }

    private void OnFireInput()
    {
        if (!isDriving) return;

        // horn 
        Debug.Log("Vehicle fire/horn pressed");
    }
}
