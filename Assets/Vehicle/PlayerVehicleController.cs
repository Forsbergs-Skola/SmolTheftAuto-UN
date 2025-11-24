using UnityEngine;
using UnityEngine.InputSystem;   // for PlayerControls & InputAction
using Vehicles;                  // for VehicleMover

public class PlayerVehicleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private PlayerController playerController; //  Euans existing player movement script
    [SerializeField] private Animator animator;     

    [Header("Vehicle Interaction")]
    [SerializeField] private float interactionRadius = 2f;

    private PlayerControls controls;

<<<<<<< Updated upstream
    private bool isDriving = false;
    private VehicleMover currentVehicle;
    private Transform currentSeat;
=======
    [Header("State Machine")]
    [Tooltip("The main player state machine asset.")]
    [SerializeField] private SimpleStateMachineSO playerStateMachine;

    [Tooltip("State asset that represents 'driving a vehicle'.")]
    [SerializeField] private StateSO drivingState;

    [Header("Transition Event Strings")]
    [SerializeField] private string enterVehicleEventString = "EnterVehicle";
    [SerializeField] private string exitVehicleEventString = "ExitVehicle";

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
    private Transform currentSeat;       // where the player sits in the car
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes

    private void Awake()
    {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
        if (playerController == null)
            playerController = GetComponent<PlayerController>();
        if (animator == null)
            animator = GetComponent<Animator>();

        controls = new PlayerControls();
    }

    private void OnEnable()
    {
=======
        if (animator == null) animator = GetComponent<Animator>();
        if (characterController == null) characterController = GetComponent<CharacterController>();
        if (playerController == null) playerController = GetComponent<PlayerController>();

        controls = new PlayerControls();

        // Se o state machine estiver setado, assina o evento de debug
        if (playerStateMachine != null)
        {
            foreach (StateSO state in playerStateMachine.GetStates())
            {
                state.OnStateEntered += HandleOnStateEntered;
            }
        }
    }

=======
        if (animator == null) animator = GetComponent<Animator>();
        if (characterController == null) characterController = GetComponent<CharacterController>();
        if (playerController == null) playerController = GetComponent<PlayerController>();

        controls = new PlayerControls();

        // Se o state machine estiver setado, assina o evento de debug
        if (playerStateMachine != null)
        {
            foreach (StateSO state in playerStateMachine.GetStates())
            {
                state.OnStateEntered += HandleOnStateEntered;
            }
        }
    }

>>>>>>> Stashed changes
    private void HandleOnStateEntered(StateData data)
    {
        Debug.Log("Entered State: " + data.StateName);
    }

    private void OnEnable() // subscribe to events
    {
        if (drivingState != null)
        {
            drivingState.OnStateEntered += OnDrivingEntered;
            drivingState.OnStateExited += OnDrivingExited;
        }

        if (moveInputEvent != null)
            moveInputEvent.OnEventTriggered += OnMoveInput;

        if (sprintInputEvent != null)
            sprintInputEvent.OnEventTriggered += OnSprintInput;

        if (fireInputEvent != null)
            fireInputEvent.OnEventTriggered += OnFireInput;

        if (controls == null)
            controls = new PlayerControls();

>>>>>>> Stashed changes
        controls.Enable();
        controls.Player.EnterVehicle.performed += OnEnterVehiclePressed;
    }

    private void OnDisable()
    {
        controls.Player.EnterVehicle.performed -= OnEnterVehiclePressed;
        controls.Disable();
    }

    private void Update()
    {
        if (!isDriving || currentVehicle == null)
            return;

        // While driving: read WASD from our own PlayerControls
        Vector2 move = controls.Player.Move.ReadValue<Vector2>();
        bool brakeHeld = controls.Player.Sprint.IsPressed();

        float throttle = move.y; // W/S
        float steer    = move.x; // A/D

        currentVehicle.SetInput(throttle, steer, brakeHeld);
    }


    private void OnEnterVehiclePressed(InputAction.CallbackContext ctx)
    {
        if (isDriving)
        {
<<<<<<< Updated upstream
            ExitVehicle();
=======
            drivingState.OnStateEntered -= OnDrivingEntered;
            drivingState.OnStateExited -= OnDrivingExited;
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        }
        else
        {
            TryEnterNearestVehicle();
        }
    }

<<<<<<< Updated upstream
<<<<<<< Updated upstream
    private void TryEnterNearestVehicle()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRadius);
=======
=======
>>>>>>> Stashed changes
    private void Update() // push input to vehicle each frame
    {
        if (!isDriving || currentVehicle == null) return;

        // push cached input into the car each frame
        currentVehicle.SetInput(currentThrottle, currentSteer, currentBrake);
    }

    private void TryInteractWithNearbyVehicle() // look for nearby vehicle to enter
    {
        // Search sphere around the player for a VehicleSeatInteraction
        Collider[] hits = Physics.OverlapSphere(transform.position, 2f);
>>>>>>> Stashed changes

        foreach (var hit in hits)
        {
            VehicleSeatInteraction seat = hit.GetComponent<VehicleSeatInteraction>();
            if (seat != null && seat.Vehicle != null)
            {
                StartDriving(seat.Vehicle, seat.SeatTransform);
                return;
            }
        }

        Debug.Log("No vehicle nearby to enter.");
    }

  private void StartDriving(VehicleMover vehicle, Transform seat)
{
    currentVehicle = vehicle;
    currentSeat = seat;
    isDriving = true;

    // If this car has an AI controller, disable it
    var ai = vehicle.GetComponent<AIVehicleController>();
    if (ai != null)
    {
<<<<<<< Updated upstream
        ai.SetAIEnabled(false);
        ai.enabled = false;
    }

    // Disable on-foot movement first
    if (characterController != null) characterController.enabled = false;
    if (playerController != null)    playerController.enabled    = false;

    // Snap player to seat and parent to it so he moves with the car
    if (currentSeat != null)
    {
        transform.SetParent(currentSeat, worldPositionStays: false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
=======
        if (vehicle == null || playerStateMachine == null) return;

        currentVehicle = vehicle;
        currentSeat = seatTransform;

        // Ask the state machine to switch from ON_FOOT -> DRIVING
        playerStateMachine.TriggerTransition(enterVehicleEventString);
    }

    private void OnEnterVehiclePressed(InputAction.CallbackContext ctx) // handle enter/exit vehicle input
    {
        // Single E key behaviour:
        //  If not driving: try to enter nearest vehicle
        //  If driving: exit current vehicle
        if (isDriving)
        {
            RequestExitVehicle();
        }
        else
        {
            TryInteractWithNearbyVehicle();
        }
>>>>>>> Stashed changes
    }

    // Reset animetor parameters                    
    if (animator != null)
    {
        animator.SetFloat("Speed", 0f);
        animator.SetBool("Sprinting", false);
    }

<<<<<<< Updated upstream
<<<<<<< Updated upstream
    
}



  private void ExitVehicle()
{
    if (!isDriving)
        return;

    isDriving = false;

    // Detach from car
    transform.SetParent(null, worldPositionStays: true);

    // Put player just to the left of the seat
    if (currentSeat != null)
    {
        Vector3 exitPos = currentSeat.position + currentSeat.right * -1f;
        transform.position = exitPos;
        transform.rotation = currentSeat.rotation;
    }

    if (characterController != null) characterController.enabled = true;
    if (playerController != null)    playerController.enabled    = true;

    // Re-enable AI, if any
    if (currentVehicle != null)
    {
        var ai = currentVehicle.GetComponent<AIVehicleController>();
        if (ai != null)
        {
            ai.enabled = true;
            ai.SetAIEnabled(true);
        }

        currentVehicle.SetInput(0f, 0f, false);
    }

    currentVehicle = null;
    currentSeat = null;
}


=======
=======
>>>>>>> Stashed changes
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
        if (playerController != null) playerController.enabled = false;

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetBool("Sprinting", false);
        }

        // switch cameras
        if (onFootCamera != null) onFootCamera.Priority = 0;
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
        if (playerController != null) playerController.enabled = true;

        // Clear car input
        currentVehicle?.SetInput(0f, 0f, false);
        currentVehicle = null;
        currentSeat = null;
        currentThrottle = currentSteer = 0f;
        currentBrake = false;

        // switching between camera
        if (onFootCamera != null) onFootCamera.Priority = 10;
        if (drivingCamera != null) drivingCamera.Priority = 0;
    }

    private void OnMoveInput(Vector2 move) // handle move input for driving
    {
        if (!isDriving) return;

        // W/S = throttle , A/D = steering 
        currentThrottle = move.y;
        currentSteer = move.x;
    }

    private void OnSprintInput(bool sprintHeld)
    {
        if (!isDriving) return;

        // treating Sprint as brake for now (talvez passar pra Space depois)
        currentBrake = sprintHeld;
    }

    private void OnFireInput()
    {
        if (!isDriving) return;

        // horn 
        Debug.Log("Vehicle fire/horn pressed");
    }
>>>>>>> Stashed changes
}
