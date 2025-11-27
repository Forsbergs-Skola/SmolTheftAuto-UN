using UnityEngine;
using UnityEngine.Events;
using GameTools;
using Events;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [SerializeField] private UnityEvent OnFire;
    [SerializeField] private float gunCooldown = 0.2f;
    [SerializeField] private bool isAutomatic;

    private float cooldown;
    private PlayerControls controls;

    private GameManagerSingleton gm;

    [SerializeField] private EnumWeaponPayloadEvent fireEvent;
    [SerializeField] private EnumWeaponPayloadEvent reloadEvent;

    private void Awake() => controls = new PlayerControls();
    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Start()
    {
        cooldown = 0f;
        gm = GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>();
    }

    private void OnDestroy()
    {
        controls.Player.Disable();
        controls.Camera.Disable();
    }

    private void Update()
    {
        bool shouldFire = isAutomatic ? controls.Player.Fire.IsPressed() : controls.Player.Fire.triggered;
        if (shouldFire && cooldown <= 0f)
            Fire();
        
        cooldown -= Time.deltaTime;
        
        if (controls.Player.Reload.triggered)
            HandleReload();
    }

    private void Fire()
    {
        int ammo = -1;
        EnumWeapon currentWeapon = gm.CurrentPlayerData.equippedWeapon;

        switch (currentWeapon)
        {
            case EnumWeapon.NONE:
                Debug.Log("No Weapon");
                return;
            
            case EnumWeapon.PISTOL:
                ammo = gm.CurrentPlayerData.pistolInClipAmmo;
                Debug.Log(ammo);
                if (ammo > 0)
                {
                    fireEvent.TriggerEvent(EnumWeapon.PISTOL);
                }
                else
                {
                    Debug.Log("Click");
                    return;
                }
                break;
            
            case EnumWeapon.RIFLE:
                ammo = gm.CurrentPlayerData.rifleInClipAmmo;
                Debug.Log(ammo);
                if (ammo > 0)
                {
                    fireEvent.TriggerEvent(EnumWeapon.RIFLE);
                }
                else
                {
                    Debug.Log("Click");
                    return;
                }
                break;
            
            case EnumWeapon.SHOTGUN:
                ammo = gm.CurrentPlayerData.shotgunInClipAmmo;
                Debug.Log(ammo);
                if (ammo > 0)
                {
                    fireEvent.TriggerEvent(EnumWeapon.SHOTGUN);
                }
                else
                {
                    Debug.Log("Click");
                    return;
                }
                break;
            
            default:
                return;
        }
        
        OnFire.Invoke();
        cooldown = gunCooldown;
    }


    void HandleReload()
    {
        EnumWeapon currentWeapon = gm.CurrentPlayerData.equippedWeapon;

        switch (currentWeapon)
        {
            case EnumWeapon.NONE:
                return;
            case EnumWeapon.PISTOL:
                reloadEvent.TriggerEvent(EnumWeapon.PISTOL);
                break;
            case EnumWeapon.RIFLE:
                reloadEvent.TriggerEvent(EnumWeapon.RIFLE);
                break;
            case EnumWeapon.SHOTGUN:
                reloadEvent.TriggerEvent(EnumWeapon.SHOTGUN);
                break;
            
            default: 
                return;
        }
    }
}