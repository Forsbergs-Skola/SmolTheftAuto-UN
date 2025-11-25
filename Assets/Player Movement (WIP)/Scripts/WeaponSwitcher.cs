using System.Collections.Generic;
using UnityEngine;
using GameTools;
using Events;


public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] private List<GameObject> weapons;
    [SerializeField] private EnumWeaponPayloadEvent swapWeaponEvent;
    
    private int currentWeapon = 0;
    
    private PlayerControls controls;
    private GameManagerSingleton gm;

    private void Awake() => controls = new PlayerControls();
    
    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>();
        SelectWeapon(currentWeapon);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable() => controls.Disable();
    void Update()
    {
        float weaponNumber = controls.Player.WeaponSelect.ReadValue<float>();

        if (weaponNumber > 0)
        {
            int weaponNumberFixed = (int)weaponNumber - 1;  //-1 as starts counting from 0
            SelectWeapon(weaponNumberFixed);
        }
    }

    private void SelectWeapon(int weaponNumber) 
    {
        //0-none,1-pistol,2-rifle,3-shotgun
        if (weaponNumber < 0 || weaponNumber >= weapons.Count)
            return;

        for (int i = 0; i < weapons.Count; i++)
            weapons[i].SetActive(i == weaponNumber);

        switch (weaponNumber)
        {
            case 0:
                swapWeaponEvent.TriggerEvent(EnumWeapon.NONE);
                break;
            case 1:
                swapWeaponEvent.TriggerEvent(EnumWeapon.PISTOL);
                break;
            case 2:
                swapWeaponEvent.TriggerEvent(EnumWeapon.RIFLE);
                break;
            case 3:
                swapWeaponEvent.TriggerEvent(EnumWeapon.SHOTGUN);
                break;
            
            default: return;
        }
        
    }
}