using UnityEngine;
using TMPro;
using GameTools;
using Events;

public class HudTester : MonoBehaviour
{
    [SerializeField] private EnumWeaponPayloadEvent weaponEquippedEvent;
    [SerializeField] private EnumWeaponPayloadEvent ammoDischargedEvent;
    [SerializeField] private EnumWeaponPayloadEvent reloadEvent;
    [SerializeField] private IntPayloadEvent healthEvent;

    [SerializeField] private int damage = 10;
    [SerializeField] private int heal = 10;

    [SerializeField] private TMP_Text dammageButtonText;
    [SerializeField] private TMP_Text healButtonText;
    
    private GameManagerSingleton gm;
    private int swapIdx = 0;


    private void Awake()
    {
        dammageButtonText.text = $"Take Damage:\n{damage}";
        healButtonText.text = $"Heal:\n{heal}";
    }

    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>();
    }

    public void SwapWeapons()
    {
        swapIdx = (swapIdx + 1) % 4;
        switch (swapIdx)
        {
            case 0:
                weaponEquippedEvent.TriggerEvent(EnumWeapon.NONE);
                break;
            case 1:
                weaponEquippedEvent.TriggerEvent(EnumWeapon.PISTOL);
                break;
            case 2:
                weaponEquippedEvent.TriggerEvent(EnumWeapon.RIFLE);
                break;
            case 3:
                weaponEquippedEvent.TriggerEvent(EnumWeapon.SHOTGUN);
                break;
        }
    }

    public void Fire()
    {
        EnumWeapon weapon = gm.CurrentPlayerData.equippedWeapon;
        PlayerData data = gm.CurrentPlayerData;
        int inClip = -1;
        string weaponString = data.equippedWeapon.ToString();

        switch (weapon)
        {
            case EnumWeapon.NONE:
                return;
            case EnumWeapon.PISTOL:
                inClip = data.pistolInClipAmmo;
                break;
            case EnumWeapon.RIFLE:
                inClip = data.rifleInClipAmmo;
                break;
            case EnumWeapon.SHOTGUN:
                inClip = data.shotgunInClipAmmo;
                break;
            default: return;
        }
        if (inClip > 0) { ammoDischargedEvent.TriggerEvent(weapon); Debug.Log($"{weaponString} says: BANG!"); }
        else { Debug.Log($"{weaponString} says: CLICK :("); }
    }
    public void Reload()
    {
        EnumWeapon weapon = gm.CurrentPlayerData.equippedWeapon;
        reloadEvent.TriggerEvent(weapon);
    }

    public void TakeDamage()
    {
        healthEvent.TriggerEvent(-damage); // <-- negative value means damage
    }
    public void Heal()
    {
        healthEvent.TriggerEvent(heal); // <-- positive value means heal
    }

}
