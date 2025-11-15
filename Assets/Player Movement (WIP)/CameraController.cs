using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    private PlayerControls controls;
    
    [Header("Mouse Settings")]
    public float xMouseSensitivity = 250f;
    public float yMouseSensitivity = 250f;
    public float minPitch = -20f; //Lowest camera angle
    public float maxPitch = 50f; //Maximum Angle
    
    public float followSpeed = 10f;
    public float cameraDistance = 3f;

    private float yaw; //yaw is horizontal rotation
    private float pitch; //and pitch is vertical

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
    
    void Update()
    {
        Vector2 lookInput = controls.Player.Look.ReadValue<Vector2>();
        float mouseX = lookInput.x * xMouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * yMouseSensitivity * Time.deltaTime;
        
        yaw += mouseX;
        pitch -= mouseY;
        
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch); //Clamp so it doesn't flip over
    }

    void LateUpdate()   
    {
        if (!player) return;
        
        Quaternion cameraRotation = Quaternion.Euler(pitch, yaw, 0f); //Takes angles & covert it to a rotation the cam can use AKA tell cam where to look based on the mouse movement
        Vector3 rotatedOffset = cameraRotation * offset;
        
        Vector3 cameraPosition = player.position + rotatedOffset - cameraRotation * Vector3.forward * cameraDistance;
        transform.position = Vector3.Lerp(transform.position, cameraPosition, followSpeed * Time.deltaTime); //Smooths the cams movement
        
        Vector3 viewPoint = player.position + offset;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(viewPoint - transform.position), followSpeed * Time.deltaTime); //Smooths the rotation
    }

}
