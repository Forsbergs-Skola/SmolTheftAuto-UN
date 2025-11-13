using UnityEngine;
using StateMachine;
using TMPro;
using Events;

public class ExamplePlayerMovement : MonoBehaviour
{
    public const string PARKED = "PARKED";
    public const string IDLE = "IDLE";
    public const string WALKING = "WALKING";
    public const string SPRINTING = "SPRINTING";

    public const string TO_PARKED = "TO_PARKED";
    public const string TO_IDLE = "TO_IDLE";
    public const string TO_WALKING = "TO_WALKING";
    public const string TO_SPRINTING = "TO_SPRINTING";

    private const float MINIMUM_MOVE_INPUT = 0.1f;

    [SerializeField] private Vector2PayloadEvent moveInputUpdate;
    [SerializeField] private BoolPayloadEvent sprintInputUpdate;
    [SerializeField] private BoolPayloadEvent gamePausedEvent;
    [SerializeField] private SimpleStateMachineSO stateMachine;

    [SerializeField] private TMP_Text animText;

    [SerializeField] float moveSpeed = 4.0f;
    [Range(1.0f, 5.0f)] [SerializeField] float sprintMultiplier;

    private Vector2 currentMoveInput = Vector2.zero;
    private bool isSprinting;

    private string _playerState;
    private string playerState
    {
        get => _playerState;
        set
        {
            if (value == _playerState) { return; }
            _playerState = value;
            Debug.Log($"Player entered state: {_playerState}");
        }
    }

    private void Start()
    {
        playerState = stateMachine.CurrentStateData.StateName;
        stateMachine.TriggerTransition(TO_IDLE);
    }
    private void OnEnable()
    {
        moveInputUpdate.OnEventTriggered += IngestMoveUpdate;
        sprintInputUpdate.OnEventTriggered += IngestSprintUpdate;
        gamePausedEvent.OnEventTriggered += IngestIsPaused;
    }
    private void OnDisable()
    {
        moveInputUpdate.OnEventTriggered -= IngestMoveUpdate;
        sprintInputUpdate.OnEventTriggered -= IngestSprintUpdate;
        gamePausedEvent.OnEventTriggered -= IngestIsPaused;
    }

    private void Update()
    {
        switch (stateMachine.CurrentStateData.StateName)
        {
            case PARKED:
                // There are no Update() behaviors for the PARKED state in this example
                // if there were, we would put them here.
                return;
            case IDLE:
                if (currentMoveInput.magnitude >= MINIMUM_MOVE_INPUT && !isSprinting)
                {
                    stateMachine.TriggerTransition(TO_WALKING);
                    return;
                }
                if (currentMoveInput.magnitude > MINIMUM_MOVE_INPUT && isSprinting)
                {
                    stateMachine.TriggerTransition(TO_SPRINTING);
                }
                return;
            case WALKING:
                if (currentMoveInput.magnitude < MINIMUM_MOVE_INPUT)
                {
                    stateMachine.TriggerTransition(TO_IDLE);
                    return;
                }
                if (currentMoveInput.magnitude >= MINIMUM_MOVE_INPUT && isSprinting)
                {
                    stateMachine.TriggerTransition(TO_SPRINTING);
                    return;
                }
                Vector3 walkMoveVector = new Vector3(currentMoveInput.x, 0.0f, currentMoveInput.y);
                transform.Translate(walkMoveVector * moveSpeed * Time.deltaTime);
                return;
            case SPRINTING:
                if (currentMoveInput.magnitude < MINIMUM_MOVE_INPUT)
                {
                    stateMachine.TriggerTransition(TO_IDLE);
                    return;
                }
                if (currentMoveInput.magnitude >= MINIMUM_MOVE_INPUT && !isSprinting)
                {
                    stateMachine.TriggerTransition(TO_WALKING);
                    return;
                }
                Vector3 sprintMoveVector = new Vector3(currentMoveInput.x, 0.0f, currentMoveInput.y);
                transform.Translate(sprintMoveVector * moveSpeed * sprintMultiplier * Time.deltaTime);
                return;
        }
    }

    


    public void HandleOnStateEntered(string stateName)
    {
        switch (stateName)
        {
            case PARKED:
                playerState = PARKED;
                animText.text = "PARKED\n(no animation)";
                // Do on enter Parked stuff
                // for example maybe make the player sptite invisible, or make the rb kinematic
                return;
            case IDLE:
                playerState = IDLE;
                animText.text = "IDLE\nANIMATION";
                // Do on enter idle stuff
                // for example play the idle animation, or start replinishing stamina
                return;
            case WALKING:
                playerState = WALKING;
                animText.text = "WALKING\nANIMATION";
                // Do on enter walking stuff
                // for example play the walking animation
                return;
            case SPRINTING:
                playerState = SPRINTING;
                animText.text = "RUNNING\nANIMATION";
                // Do on enter sprinting stuff
                // for example play the sprinting animation, or start depleting stamina
                return;
            default:
                return;
        }
    }

    private void IngestMoveUpdate(Vector2 _input)
    {
        currentMoveInput = _input;
    }

    private void IngestSprintUpdate(bool _input)
    {
        isSprinting = _input;
    }
    private void IngestIsPaused(bool _isPaused)
    {
        if (_isPaused)
        {
            stateMachine.TriggerTransition(TO_PARKED);
            return;
        }
        stateMachine.TriggerTransition(TO_IDLE);
    }
}
