using Unity.Cinemachine;
using GameTools;
using Events;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private const float HIT_COOLDOWN = 0.1f;


    [Range(-100, 0)] [SerializeField] private int meleeDamage = -5;

    [SerializeField] private CharacterController characterController;
    private Animator animator;
    private PlayerControls controls;
    [SerializeField] private Transform cam;

    [SerializeField] private Transform yawTarget;
    
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float sprintSpeed = 12f;
    
    [Header("Jump")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float gravity = -9.81f;
    
    private Vector3 velocity;
    [SerializeField] private float rotationDeadzone = 0.15f;
    
    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Event Channels")]
    [SerializeField] private StringPayloadEvent dialogueStartedEvent;
    [SerializeField] private StringPayloadEvent dialogueEndedEvent;
    [SerializeField] private IntPayloadEvent healthChangedEvent;
    [SerializeField] private EmptyPayloadEvent playerDataChangedEvent;
    [SerializeField] private EmptyPayloadEvent pauseToggledEvent;

    //private bool dialogueIsActive = false;

    public bool isAiming;

    private bool isHittable = true;

    private bool gameIsPaused = false;

    private GameManagerSingleton gm;

    private Vector3 moveDirection;

    void Awake() => controls = new PlayerControls();
    //void OnEnable() => controls.Enable();
    private void OnEnable()
    {
        controls.Enable();
        dialogueStartedEvent.OnEventTriggered += HandleDialogueStarted;
        dialogueEndedEvent.OnEventTriggered += HandleDialogueFinished;
        playerDataChangedEvent.OnEventTriggered += HandlePlayerDataChanged;
        pauseToggledEvent.OnEventTriggered += HandleOnPauseToggled;
    }
    //void OnDisable() => controls.Disable();
    private void OnDisable()
    {
        controls.Disable();
        dialogueStartedEvent.OnEventTriggered -= HandleDialogueStarted;
        dialogueEndedEvent.OnEventTriggered -= HandleDialogueFinished;
        playerDataChangedEvent.OnEventTriggered -= HandlePlayerDataChanged;
        pauseToggledEvent.OnEventTriggered -= HandleOnPauseToggled;
    }
    

    private void OnDestroy()
    {
        controls.Player.Disable();
        controls.Camera.Disable();
    }

    //void Start() => animator = GetComponent<Animator>();
    private void Start()
    {
        animator = GetComponent<Animator>();
        gm = GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>();
    }

    void Update()
    {
        bool grounded = IsGrounded();
        bool sprintHeld = controls.Player.Sprint.IsPressed();
        
        PlayerMovement(sprintHeld);
        JumpingAndGravityLogic(grounded);
        AnimationHandling(grounded, sprintHeld);
    }


    void PlayerMovement(bool sprintHeld) //here I handle all the horizontal ground movement
    {

       

        Vector2 moveInput = controls.Player.Move.ReadValue<Vector2>();

        if (isAiming)
        {
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;
        
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
           
            moveDirection = forward * moveInput.y + right * moveInput.x; //move where cam is looking
        }
        else
        {
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;
        
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();
            moveDirection = camForward * moveInput.y + camRight * moveInput.x; //move where cam is looking
        }

        if (isAiming)
        {
            Vector3 lookDirection = yawTarget.forward;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude > rotationDeadzone)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (moveDirection.magnitude > rotationDeadzone) //Smooths the playermodel rotation when you turn your moouse
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
            }
        }
        
        float speed = sprintHeld ? sprintSpeed : walkSpeed; //Sets the speed based on if the sprint button (shift currently) is down
        
        Vector3 finalMovement = moveDirection * speed + velocity;
        animator.SetFloat("Speed", moveDirection.magnitude); //Sets the speed value to the animator
        characterController.Move(finalMovement * Time.deltaTime);
    }
    
    bool IsGrounded() //True if player on ground, fires raycast
    {
        float bottom = (characterController.height - 0.8f) - characterController.radius;
        Vector3 origin = transform.position + Vector3.down * bottom;  //Ground check slightly under the player
        float rayLength = groundCheckDistance + 0.1f;
        return Physics.Raycast(origin, Vector3.down, rayLength, groundLayer); //Fire straight down to detect ground
    }

    private void HandleDialogueStarted(string _unusedStr)
    {
        controls.Disable();
    }
    private void HandleDialogueFinished(string _unusedStr)
    {
        controls.Enable();
    }


    private void HandlePlayerDataChanged()
    {
        if (gm == null) return;
        int health = gm.CurrentPlayerData.health;
        if (health <= 0)
        {
            Die();
        }
    }

    private void HandleOnPauseToggled()
    {
        gameIsPaused = !gameIsPaused;
        if (gameIsPaused) { controls.Disable(); }
        else { controls.Enable(); }

    }

    private void Die()
    {
        // TODO...(die stuff)
    }

    public void TakeDamage()
    {
        if (!isHittable) return;
        isHittable = false;
        StartCoroutine(HitCooldown());
        healthChangedEvent.TriggerEvent(meleeDamage);
    }
    System.Collections.IEnumerator HitCooldown()
    {
        yield return new WaitForSeconds(HIT_COOLDOWN);
        isHittable = true;
    }



    void JumpingAndGravityLogic(bool grounded)
    {
        bool jumpPressed = controls.Player.Jump.triggered;
        
        velocity.y += gravity * Time.deltaTime; //Gotta apply gravity constantly as we not using a rigidbody
        
        if (characterController.isGrounded && velocity.y < 0)//Reset Y velocity when grounded
            velocity.y = -2f; 
        
        if (grounded &&  jumpPressed)
            velocity.y = jumpForce;
    }

    void AnimationHandling(bool grounded, bool sprintHeld) //Sprinting, Falling, Jumping animations
    {
        animator.SetBool("Grounded", grounded);
        
        if (!grounded)
        {
            if (velocity.y > 0.1f)
            {
                animator.SetBool("Jumping", true);
                animator.SetBool("Falling", false);
            }
            else if (velocity.y < -0.1f)
            {
                animator.SetBool("Jumping", false);
                animator.SetBool("Falling", true);
            }
        }
        else
        {
            animator.SetBool("Jumping", false);
            animator.SetBool("Falling", false);
        }
        animator.SetBool("Sprinting", sprintHeld);
    }
    
}
