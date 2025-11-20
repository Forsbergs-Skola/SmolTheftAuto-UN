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

    private bool isDriving = false;
    private VehicleMover currentVehicle;
    private Transform currentSeat;

    private void Awake()
    {
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
            ExitVehicle();
        }
        else
        {
            TryEnterNearestVehicle();
        }
    }

    private void TryEnterNearestVehicle()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRadius);

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

    // Disable on-foot movement first
    if (characterController != null) characterController.enabled = false;
    if (playerController != null)    playerController.enabled    = false;

    // Snap player to seat and parent to it so he moves with the car
    if (currentSeat != null)
    {
        transform.SetParent(currentSeat, worldPositionStays: false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    // Reset anim
    if (animator != null)
    {
        animator.SetFloat("Speed", 0f);
        animator.SetBool("Sprinting", false);
    }

    // Camera: do nothing same Cinemachine vcam follows the player, but may be need to adjust later
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

    currentVehicle?.SetInput(0f, 0f, false);
    currentVehicle = null;
    currentSeat = null;
}

}
