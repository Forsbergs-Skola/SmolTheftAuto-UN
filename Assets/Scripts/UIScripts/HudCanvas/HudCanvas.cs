using UnityEngine;
using TMPro;
using Tweens;

public class HudCanvas : MonoBehaviour, ICanvasable
{

    [SerializeField] private GameObject missionPassedObject;
    [SerializeField] private TMP_Text missionPassedText;

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
