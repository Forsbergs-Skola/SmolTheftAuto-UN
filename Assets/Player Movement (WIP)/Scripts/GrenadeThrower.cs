using UnityEngine;

public class GrenadeThrower : MonoBehaviour
{
    [SerializeField] private float forwardForce = 10f;
    [SerializeField] private float upwardForce = 5f;
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private Transform cam;
    
    private PlayerControls controls;

    private void Awake() => controls = new PlayerControls();
    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();
    
    void Update()
    {
        if (controls.Player.Grenade.triggered)
            ThrowGrenade();
    }

    void ThrowGrenade()
    {
        GameObject grenade = Instantiate(grenadePrefab, transform.position, cam.transform.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        Vector3 forwardDirection = cam.transform.forward;
        Vector3 upwardDirection  = cam.transform.up;
        
        rb.AddForce(forwardDirection * forwardForce, ForceMode.VelocityChange);
        rb.AddForce(upwardDirection * upwardForce,  ForceMode.VelocityChange);
    }
}
