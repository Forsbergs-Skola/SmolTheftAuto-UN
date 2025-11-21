using UnityEngine;
using UnityEngine.Events;
using GameTools;
using Events;

public class Gun : MonoBehaviour
{
    public UnityEvent OnFire;
    public float gunCooldown = 0.2f;
    public bool isAutomatic;

    private float cooldown;
    private PlayerControls controls;

    private GameManagerSingleton gm;
    [SerializeField] private IntPayloadEvent shootGunEvent;

    private void Awake() => controls = new PlayerControls();
    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Start()
    {
        cooldown = 0f;
        gm = GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>();
    }

    private void Update()
    {
        bool shouldFire = isAutomatic ? controls.Player.Fire.IsPressed() : controls.Player.Fire.triggered;
        if (shouldFire && cooldown <= 0f)
            Fire();
        
        cooldown -= Time.deltaTime;
    }

    private void Fire()
    {
        //bool hasAmmo = gm.CurrentPlayerData.ammo > 0;
        bool hasAmmo = true;

        if (hasAmmo)
        {
            shootGunEvent.TriggerEvent(-1);
            OnFire.Invoke();
            cooldown = gunCooldown;
        }
    }
}