using UnityEngine;

public class GrenadeThrower : MonoBehaviour
{
    public float forwardForce = 10f;
    public float upwardForce = 5f;
    public GameObject grenadePrefab;
    public Transform cam;
    
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
