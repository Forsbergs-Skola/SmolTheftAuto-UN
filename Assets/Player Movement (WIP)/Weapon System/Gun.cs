using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour
{
    public UnityEvent OnFire;
    public float gunCooldown = 0.2f;
    public bool isAutomatic;

    private float cooldown;
    private PlayerControls controls;

    private void Awake() => controls = new PlayerControls();
    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();
    private void Start() => cooldown = gunCooldown;

    private void Update()
    {
        bool shouldFire = isAutomatic ? controls.Player.Fire.IsPressed() : controls.Player.Fire.triggered;
        if (shouldFire && cooldown <= 0f)
            Fire();
        
        cooldown -= Time.deltaTime;
    }

    private void Fire()
    {
        OnFire.Invoke();
        cooldown = gunCooldown;
    }
}