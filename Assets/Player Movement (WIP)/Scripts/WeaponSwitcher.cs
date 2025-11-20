using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] private List<GameObject> weapons;
    private int currentWeapon = 0;
    
    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
    
    void Start() => SelectWeapon(currentWeapon);
    
    void Update()
    {
        float weaponNumber = controls.Player.WeaponSelect.ReadValue<float>();

        if (weaponNumber > 0)
        {
            int index = (int)weaponNumber - 1;   // convert 1 → 0, 2 → 1, etc.
            SelectWeapon(index);
        }
    }

    private void SelectWeapon(int index)
    {
        if (index < 0 || index >= weapons.Count)
            return; // safety check

        for (int i = 0; i < weapons.Count; i++)
            weapons[i].SetActive(i == index);
    }
}