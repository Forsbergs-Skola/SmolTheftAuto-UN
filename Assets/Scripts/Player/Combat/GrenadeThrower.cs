using UnityEngine;
using GameTools;
using Events;

public class GrenadeThrower : MonoBehaviour
{
    [SerializeField] private float forwardForce = 10f;
    [SerializeField] private float upwardForce = 5f;
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private Transform cam;
    [SerializeField] private IntPayloadEvent grenadeChangedEvent;
    
    private PlayerControls controls;
    private GameManagerSingleton gm;

    private void Awake() => controls = new PlayerControls();
    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();
    
    private void Start() => gm = GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>();
    
    void Update()
    {
        if (controls.Player.Grenade.triggered && gm.CurrentPlayerData.granades > 0)
            ThrowGrenade();
    }

    private void OnDestroy()
    {
        controls.Player.Disable();
        controls.Camera.Disable();
    }

    void ThrowGrenade()
    {
        grenadeChangedEvent.TriggerEvent(-1);
        
        GameObject grenade = Instantiate(grenadePrefab, transform.position, cam.transform.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb.AddForce(cam.forward * forwardForce, ForceMode.VelocityChange);
        rb.AddForce(cam.up * upwardForce, ForceMode.VelocityChange);    
    }
}
