using UnityEngine;
using Events;
using TMPro;
using GameTools;
using UnityEngine.UI;

public class StoreCanvas : MonoBehaviour, ICanvasable
{

    [SerializeField] private IntPayloadEvent moneyEvent;
    [SerializeField] private EnumWeaponPayloadEvent ammoPickupEvent;
    [SerializeField] private EmptyPayloadEvent storeInteractionFinishedEvent;

    [SerializeField] private Button buyAmmoButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text pistolText;
    [SerializeField] private TMP_Text rifleText;
    [SerializeField] private TMP_Text shotgunText;
    [SerializeField] private TMP_Text grenadesText;

    private bool _isVisible = false;
    private bool isVisible
    {
        get => _isVisible;
        set
        {
            if (value != _isVisible)
            {
                _isVisible = value;
                gameObject.SetActive(_isVisible);
            }
        }
    }

    private void Start()
    {
        gameObject.SetActive(isVisible);
    }


    public void StartStoreInteraction()
    {
        FixUI();
    }

    public void FixUI()
    {
        GameManagerSingleton gm = GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>();
        int playerMoney = gm.CurrentPlayerData.money;
        int ammoCost = gm.AmmoRefilCost;

        string moneyStr = $"Money: {playerMoney}";
        string pistolStr = $"Pistol Ammo: {gm.CurrentPlayerData.pistolInClipAmmo} / {gm.CurrentPlayerData.pistolTotalAmmo}";
        string rifleStr = $"Rifle Ammo: {gm.CurrentPlayerData.rifleInClipAmmo} / {gm.CurrentPlayerData.rifleTotalAmmo}";
        string shotgunStr = $"Shotgun Ammo: {gm.CurrentPlayerData.shotgunInClipAmmo} / {gm.CurrentPlayerData.shotgunTotalAmmo}";
        string grenadesStr = $"Grenades: {gm.CurrentPlayerData.granades}";

        moneyText.text = moneyStr;
        pistolText.text = pistolStr;
        rifleText.text = rifleStr;
        shotgunText.text = shotgunStr;
        grenadesText.text = grenadesStr;


        if (playerMoney >= ammoCost)
        {
            moneyText.color = new Color(0f, 1f, 0f);
            dialogueText.text = $"Wanna buy some more ammo? It costs {ammoCost}.";
            buyAmmoButton.gameObject.SetActive(true);
        }
        else
        {
            moneyText.color = new Color(1f, 0f, 0f);
            dialogueText.text = $"You need more money...broke ass elf. It costs {ammoCost}.";
            buyAmmoButton.gameObject.SetActive(false);
        }
    }

    public void HandleBuyAmmoButtonPressed()
    {
        GameManagerSingleton gm = GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>();
        int ammoCost = gm.AmmoRefilCost;
        ammoPickupEvent.TriggerEvent(EnumWeapon.PISTOL);
        ammoPickupEvent.TriggerEvent(EnumWeapon.RIFLE);
        ammoPickupEvent.TriggerEvent(EnumWeapon.SHOTGUN);
        ammoPickupEvent.TriggerEvent(EnumWeapon.GRENADE);
        moneyEvent.TriggerEvent(-ammoCost);
        FixUI();
    }
    public void CancelButtonPressed()
    {
        storeInteractionFinishedEvent.TriggerEvent();
    }


    public EnumCanvasName CanvasName()
    {
        return EnumCanvasName.STORE;
    }
    public GameObject GetCanvasObject()
    {
        return gameObject;
    }
    public bool GetIsVisible()
    {
        return isVisible;
    }
    public void SetIsVisible(bool val)
    {
        isVisible = val;
    }

}
