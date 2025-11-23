using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameTools;
using Tweens;
using Events;

public class HudCanvas : MonoBehaviour, ICanvasable
{

    private GameManagerSingleton gm;

    private const string WEAPON_PREFIX = "Weapon: ";
    private const string AMMO_PREFIX = "Ammo: ";
    private const string MONEY_PREFIX = "Money: ";
    private const string KILLS_PREFIX = "NPCs Killed: ";
    private const string CHECKPOINTS_PREFIX = "Checkpoints Reached: ";

    [SerializeField] private GameObject missionPassedObject;
    [SerializeField] private TMP_Text missionPassedText;
    [SerializeField] private EmptyPayloadEvent playerDataUpdatedEvent;

    [SerializeField] private TMP_Text weaponText;
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text killsText;
    [SerializeField] private TMP_Text checkpointsText;
    [SerializeField] private Slider healthSlider;

    private bool _isVisible = false;
    private bool isVisible
    {
        get => _isVisible;
        set
        {
            if(value != _isVisible)
            {
                _isVisible = value;
                gameObject.SetActive(_isVisible);
            }
        }
    }
    private void Awake()
    {
        isVisible = gameObject.activeInHierarchy;
    }

    private void Start()
    {
        missionPassedObject.SetActive(false);
        gm = GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>();
    }

    private void OnEnable()
    {
        playerDataUpdatedEvent.OnEventTriggered += HandleOnPlayerDataUpdated;
    }
    private void OnDisable()
    {
        playerDataUpdatedEvent.OnEventTriggered -= HandleOnPlayerDataUpdated;
    }

    private void HandleOnPlayerDataUpdated()
    {
        PlayerData data = gm.CurrentPlayerData;

        //Debug.Log("HUD says: PlayerData updated -- TODO: update the HUD indicators");

        // TODO: update the hud from data
        healthSlider.value = data.health;
        FixWeaponText(data.equippedWeapon);
        FixAmmoText(data.equippedWeapon);
        FixMoneyText(data.money);

        if (gm.CurrentQuestStartedData.gasCan && !data.hasGasCan)
        {
            string killsString = $"{KILLS_PREFIX} {data.npcsKilled}";
            killsText.text = killsString;
        } else { killsText.text = string.Empty; }
        if (gm.CurrentQuestStartedData.matches && !data.hasMatches)
        {
            string checkpointsString = $"{CHECKPOINTS_PREFIX} {data.checkpointsReached}";
            checkpointsText.text = checkpointsString;
        } else { checkpointsText.text = string.Empty; }

    }

    private void FixWeaponText(EnumWeapon weapon)
    {
        string weaponString = weapon.ToString();
        weaponString = $"{WEAPON_PREFIX} {weaponString}";
        weaponText.text = weaponString;
    }
    private void FixAmmoText(EnumWeapon weapon)
    {
        if (weapon == EnumWeapon.NONE) { ammoText.text = string.Empty; return; }
        int inClip = gm.GetInClipAmmo(weapon);
        int total = gm.GetTotalAmmo(weapon);
        string ammoString = $"{AMMO_PREFIX} {inClip} / {total}";
        ammoText.text = ammoString;
    }

    private void FixMoneyText(int money)
    {
        string moneyString = $"{MONEY_PREFIX} {money.ToString()}";
        moneyText.text = moneyString;
    }


    public void ActivateMissionPassedEffect()
    {
        missionPassedText.color = new Color(1f, 1f, 0f, 1f);
        missionPassedObject.transform.localScale = new Vector3(1f, 1f, 1f);
        Tween scaleTween = TweenService.GetFloatTween(gameObject, 10f, 1f, 0.2f, EnumTweenEase.QUART, EnumTweenDirection.IN);
        scaleTween.StartTween();
        missionPassedObject.SetActive(true);
        scaleTween.OnValueUpdated += (value) =>
        {
            Vector3 newScale = new Vector3(value.x, value.x, value.x);
            missionPassedObject.transform.localScale = newScale;
        };
        scaleTween.OnFinished += () =>
        {
            StartCoroutine(WaitThenFade(0.75f));
        };
    }

    private System.Collections.IEnumerator WaitThenFade(float wait)
    {
        yield return new WaitForSeconds(wait);
        Tween alphaTween = TweenService.GetFloatTween(gameObject, 1f, 0f, 2.0f, EnumTweenEase.LINEAR);
        alphaTween.StartTween();
        alphaTween.OnValueUpdated += (newAlpha) =>
        {
            Color newColor = new Color(1f, 1f, 0f, newAlpha.x);
            missionPassedText.color = newColor;
        };
    }


    public EnumCanvasName CanvasName()
    {
        return EnumCanvasName.HUD;
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
