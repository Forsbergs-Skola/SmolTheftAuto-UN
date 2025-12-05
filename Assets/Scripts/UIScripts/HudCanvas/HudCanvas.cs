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
    [SerializeField] private GameObject youDiedObject;
    [SerializeField] private GameObject youWinObject;
    [SerializeField] private TMP_Text missionPassedText;
    [SerializeField] private TMP_Text youDiedText;
    [SerializeField] private TMP_Text youWinText;

    [SerializeField] private TMP_Text weaponText;
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text killsText;
    [SerializeField] private TMP_Text checkpointsText;
    [SerializeField] private TMP_Text grenadesText;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Button mainMenuButton;

    public enum EnumOnActivateEffectBehavior
    {
        MISSION_PASSED,
        YOU_DIED,
        YOU_WIN
    }

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
        mainMenuButton.gameObject.SetActive(false);
    }

    private void Start()
    {
        missionPassedObject.SetActive(false);
        gm = GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>();
    }

    public void HandleOnPlayerDataUpdated(PlayerData _data)
    {

        healthSlider.value = _data.health;
        FixWeaponText(_data.equippedWeapon);
        FixAmmoText(_data.equippedWeapon);
        FixMoneyText(_data.money);

        if (gm == null)
        {
            gm = GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>();
        }

        if (gm.CurrentQuestStartedData.gasCan && !_data.hasGasCan)
        {
            string cpString = $"{CHECKPOINTS_PREFIX} {_data.checkpointsReached}";
            checkpointsText.text = cpString;
        } else { checkpointsText.text = string.Empty; }
        if (gm.CurrentQuestStartedData.matches && !_data.hasMatches)
        {
            string killsString = $"{KILLS_PREFIX} {_data.npcsKilled}";
            killsText.text = killsString;
        } else { killsText.text = string.Empty; }

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

        int grenades = gm.CurrentPlayerData.granades;
        grenadesText.text = $"Grenades: {grenades}";


    }

    private void FixMoneyText(int money)
    {
        string moneyString = $"{MONEY_PREFIX} {money.ToString()}";
        moneyText.text = moneyString;
    }


    public void ActivateMissionPassedEffect()
    {
        missionPassedText.color = new Color(1f, 1f, 0f, 1f);
        missionPassedObject.transform.localScale = Vector3.one;
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

    public void ActivateYouDiedEffect()
    {
        youDiedObject.transform.localScale = Vector3.one;
        Tween scaleTween = TweenService.GetFloatTween(gameObject, 10f, 1f, 0.2f, EnumTweenEase.QUART, EnumTweenDirection.IN);
        scaleTween.StartTween();
        youDiedObject.SetActive(true);
        scaleTween.OnValueUpdated += (value) =>
        {

            Debug.Log("FOO");

            Vector3 newScale = new Vector3(value.x, value.x, value.x);
            youDiedObject.transform.localScale = newScale;
        };
        scaleTween.OnFinished += () =>
        {
            StartCoroutine(WaitThenShowMainMenuButton(0.75f));
        };
    }

    public void ActivateYouWinEffect()
    {
        youWinObject.transform.localScale = Vector3.one;
        Tween scaleTween = TweenService.GetFloatTween(gameObject, 10f, 1f, 0.2f, EnumTweenEase.QUART, EnumTweenDirection.IN);
        scaleTween.StartTween();
        youWinObject.SetActive(true);
        scaleTween.OnValueUpdated += (value) =>
        {
            Vector3 newScale = new Vector3(value.x, value.x, value.x);
            youWinObject.transform.localScale = newScale;
        };
        scaleTween.OnFinished += () =>
        {
            StartCoroutine(WaitThenShowMainMenuButton(0.75f));
        };
    }

    public void ActivateUiEffect(EnumOnActivateEffectBehavior behavior, float waitTime)
    {
        GameObject uiObj = null;
        bool doFadeBehavior = false;
        switch (behavior)
        {
            case EnumOnActivateEffectBehavior.YOU_WIN:
                uiObj = youWinObject;
                break;
            case EnumOnActivateEffectBehavior.MISSION_PASSED:
                doFadeBehavior = true;
                uiObj = missionPassedObject;
                break;
            case EnumOnActivateEffectBehavior.YOU_DIED:
                uiObj = youDiedObject;
                break;
        }
        uiObj.transform.localScale = Vector3.one;
        Tween scaleTween = TweenService.GetFloatTween(gameObject, 10f, 1f, 0.2f, EnumTweenEase.QUART, EnumTweenDirection.IN);
        scaleTween.StartTween();
        uiObj.SetActive(true);
        scaleTween.OnValueUpdated += (value) =>
        {
            Vector3 newScale = new Vector3(value.x, value.x, value.x);
            uiObj.transform.localScale = newScale;
        };
        scaleTween.OnFinished += () =>
        {
            if (doFadeBehavior) { StartCoroutine(WaitThenFade(waitTime)); }
            else { StartCoroutine(WaitThenShowMainMenuButton(waitTime)); }
        };


    }

    public void MainMenuPressed()
    {
        mainMenuButton.gameObject.SetActive(false);
        youDiedObject.SetActive(false );
        missionPassedObject.SetActive(false);
        youWinObject.SetActive(false);
    }

    private System.Collections.IEnumerator WaitThenShowMainMenuButton(float wait)
    {
        yield return new WaitForSeconds(wait);
        mainMenuButton.gameObject.SetActive(true);
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
