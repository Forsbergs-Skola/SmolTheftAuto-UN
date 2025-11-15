using Unity.Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController characterController;
    private Animator animator;
    private PlayerControls controls;
    public Transform cam;

    [SerializeField] private CinemachineCamera aimCamera;
    
    [Header("Movement")]
    public float walkSpeed = 6f;
    public float rotateSpeed = 10f;
    public float sprintSpeed = 12f;
    
    [Header("Jump")]
    public float jumpForce = 6f;
    public float gravity = -9.81f;
    
    private Vector3 velocity;
    public float rotationDeadzone = 0.15f;
    
    [Header("Ground Check")]
    public float groundCheckDistance = 0.3f;
    public LayerMask groundLayer;
    
    void Awake()
    {
        controls = new PlayerControls();
    }
    
    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        bool grounded = IsGrounded();
        bool sprintHeld = controls.Player.Sprint.IsPressed();
        bool aimHeld = controls.Player.Aim.IsPressed();
        
        PlayerMovement(sprintHeld, aimHeld);
        JumpingAndGravityLogic(grounded);
        AnimationHandling(grounded, sprintHeld);
        CameraBlending(aimHeld);
    }

    void PlayerMovement(bool sprintHeld, bool aimHeld) //here I handle all the horizontal ground movement
    {
        Vector2 moveInput = controls.Player.Move.ReadValue<Vector2>();
        
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;
        
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();
        
        Vector3 moveDirection = camForward * moveInput.y + camRight * moveInput.x; //move where cam is looking


        if (aimHeld)
        {
            Vector3 lookDirection = cam.forward;
            lookDirection.y = 0f;
            
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
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

    void JumpingAndGravityLogic(bool grounded)
    {
        bool jumpPressed = controls.Player.Jump.triggered;
        
        velocity.y += gravity * Time.deltaTime; //Gotta apply gravity constantly as we not using a rigidbody
        
        if (characterController.isGrounded && velocity.y < 0)//Reset Y velocity when grounded
            velocity.y = -2f; 
        
        if (grounded &&  jumpPressed)
            velocity.y = jumpForce;
    }

    void AnimationHandling(bool grounded, bool sprintHeld) //Handles Sprinting, Falling and Jumping animations
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

    void CameraBlending(bool aimHeld)
    {
        if (aimHeld)
        {
            aimCamera.Priority = 20;
        }
        else
        {
            aimCamera.Priority = 5;
        }
    }
}
