using UnityEngine;
using Events;
using GameTools;
using TMPro;
using UnityEngine.UI;

public class PauseCanvas : MonoBehaviour, ICanvasable
{

    private enum EnumPausePanel
    {
        INVENTORY,
        QUESTS,
    }

    [SerializeField] private Button switchViewButton;
    [SerializeField] private TMP_Text switchViewButtonText;
    [SerializeField] private InventoryPanel inventoryPanel;
    [SerializeField] private QuestPanel questPanel;

    [SerializeField] private GameObject gasCanImage;
    [SerializeField] private GameObject matchesImage;
    [SerializeField] private GameObject sunglassesImage;

    //[SerializeField] private EmptyPayloadEvent playerDataUpdatedEvent;

    private GameManagerSingleton gm;

    private EnumPausePanel currentPanel;
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
                HandleVisiblityChanged();
            }
        }
    }
    private void Awake()
    {
        isVisible = gameObject.activeInHierarchy;
    }
    private void Start()
    {
        DisplayPanel(EnumPausePanel.QUESTS);
        gm = GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>();
        FixInventoryImages(gm.CurrentPlayerData);
    }

    /*
    private void OnEnable()
    {
        playerDataUpdatedEvent.OnEventTriggered += HandleInventoryUpdate;
    }
    private void OnDisable()
    {
        playerDataUpdatedEvent.OnEventTriggered -= HandleInventoryUpdate;
    }
    */

    public EnumCanvasName CanvasName()
    {
        return EnumCanvasName.PAUSE;
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
    public void HandleInventoryUpdate(PlayerData _data)
    {
        FixInventoryImages(_data);
    }

    private void FixInventoryImages(PlayerData _pData)
    {
        matchesImage.SetActive(_pData.hasMatches);
        gasCanImage.SetActive(_pData.hasGasCan);
        sunglassesImage.SetActive(_pData.hasSunglasses);
    }

    private void DisplayPanel(EnumPausePanel panel)
    {
        switch (panel)
        {
            case EnumPausePanel.INVENTORY:
                questPanel.gameObject.SetActive(false);
                inventoryPanel.gameObject.SetActive(true);
                switchViewButtonText.text = "View Quest Log";
                currentPanel = EnumPausePanel.INVENTORY;
                break;
            case EnumPausePanel.QUESTS:
                questPanel.gameObject.SetActive(true);
                inventoryPanel.gameObject.SetActive(false);
                switchViewButtonText.text = "View Inventory";
                currentPanel = EnumPausePanel.QUESTS;

                break;
            default: return;
        }
    }

    public void HandleOnSwitchViewButtonPressed()
    {
        switch (currentPanel)
        {
            case EnumPausePanel.INVENTORY:
                DisplayPanel(EnumPausePanel.QUESTS);
                break;
            case EnumPausePanel.QUESTS:
                DisplayPanel(EnumPausePanel.INVENTORY);
                break;
            default: return;
        }
    }

    private void HandleVisiblityChanged()
    {

        //if (isVisible)
        //{
        //    RectTransform rt = GetComponent<RectTransform>();
        //    LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        //}


        DisplayPanel(EnumPausePanel.QUESTS);



    }

}
