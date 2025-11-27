using UnityEngine;
using GameTools;
using UnityEngine.UI;
using Events;

public class MainMenuCanvas : MonoBehaviour, ICanvasable
{
    [SerializeField] private EmptyPayloadEvent newGamePressedEvent;
    [SerializeField] private EmptyPayloadEvent continuePressedEvent;

    [SerializeField] private BoolPayloadEvent saveExistsChangedEvent;
    //[SerializeField] private EmptyPayloadEvent aboutPressedEvent;
    //[SerializeField] private EmptyPayloadEvent quitPressedEvent;

    [SerializeField] private Button continueButton;


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
    private void Awake()
    {
        isVisible = gameObject.activeInHierarchy;
    }

    private void Start()
    {
        GameManagerSingleton gm = GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>();
        continueButton.gameObject.SetActive(gm.SaveExists());
    }

    private void OnEnable()
    {
        GameManagerSingleton gm = GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>();
        saveExistsChangedEvent.OnEventTriggered += HandleSaveExistsChanged;
        continueButton.gameObject.SetActive(gm.SaveExists());
    }
    private void OnDisable()
    {
        saveExistsChangedEvent.OnEventTriggered -= HandleSaveExistsChanged;
    }

    private void HandleSaveExistsChanged(bool saveExists)
    {
        continueButton.gameObject.SetActive(saveExists);
    }
    public void NewGamePressed()
    {
        newGamePressedEvent.TriggerEvent();
    }
    public void ContinuePressed()
    {
        continuePressedEvent.TriggerEvent();
    }
    public void AboutPressed()
    {

    }
    public void QuitPressed()
    {

    }


    public EnumCanvasName CanvasName()
    {
        return EnumCanvasName.MAIN;
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
