using UnityEngine;
using GameTools;
using Events;


public enum EnumQuest
{
    GAS_CAN,
    SUNGLASSES,
    MATCHES,
    FINAL
}

public class GameManagerSingleton : MonoBehaviour
{

    // for testing --  remove later
    [SerializeField] private TestScene testScene;

    public const int MAX_HEALTH = 100;

    [Header("Event Channels")]
    [SerializeField] private EnumQuestPayloadEvent questCompletedEvent;
    [SerializeField] private EnumQuestPayloadEvent questStartedEvent;
    [SerializeField] private IntPayloadEvent moneyChangedEvent;
    [SerializeField] private IntPayloadEvent ammoChangedEvent;
    [SerializeField] private IntPayloadEvent healthChangedEvent;
    [SerializeField] private EmptyPayloadEvent saveGameRequestedEvent;
    [SerializeField] private EmptyPayloadEvent clearSaveRequestedEvent;
    [SerializeField] private BoolPayloadEvent saveExistsChangedEvent;



    private PlayerData currentPlayerData;
    private QuestStartedData currentQuestStartedData;

    public PlayerData CurrentPlayerData { get => currentPlayerData; }
    public QuestStartedData CurrentQuestStartedData { get => currentQuestStartedData; }
    

    private void Awake()
    {
        if (GameObject.FindGameObjectsWithTag(Constants.Tags.GAME_MANAGER).Length > 0) { Destroy(gameObject); }
        tag = Constants.Tags.GAME_MANAGER;
        DontDestroyOnLoad(gameObject);
        
    }
    private void Start()
    {
        SaveManager sm = GameObject.FindGameObjectWithTag(Constants.Tags.SAVE_MANAGER).GetComponent<SaveManager>();
        if (sm.SaveExists())
        {
            SaveData data = sm.Load();
            currentPlayerData = sm.ConvertPlayer(data);
            currentQuestStartedData = sm.ConvertQuest(data);
            saveExistsChangedEvent.TriggerEvent(true);
        }
        else
        {
            currentPlayerData = ResetPlayerData();
            currentQuestStartedData = ResetQuestsData();
            saveExistsChangedEvent.TriggerEvent(false);
        }

        CanvasManager cm = GameObject.FindGameObjectWithTag(Constants.Tags.CANVAS_MANAGER).GetComponent<CanvasManager>();
        cm.questPanel.InitializeQuestUI(currentPlayerData, currentQuestStartedData);
        testScene.FixButtons(currentPlayerData, currentQuestStartedData);
    }

    private PlayerData ResetPlayerData()
    {
        return new PlayerData(0, 0, MAX_HEALTH, false, false, false);
    }
    private QuestStartedData ResetQuestsData()
    {
        return new QuestStartedData(false, false, false, false);
    }

    private void OnEnable()
    {
        questCompletedEvent.OnEventTriggered += HandleOnQuestCompleted;
        questStartedEvent.OnEventTriggered += HandleOnQuestStarted;
        moneyChangedEvent.OnEventTriggered += HandleOnMoneyChanged;
        ammoChangedEvent.OnEventTriggered += HandleOnAmmoChanged;
        healthChangedEvent.OnEventTriggered += HandleOnHealthChanged;
        saveGameRequestedEvent.OnEventTriggered += HandleOnSaveRequested;
        clearSaveRequestedEvent.OnEventTriggered += HandleOnClearSaveRequested;
    }
    private void OnDisable()
    {
        questCompletedEvent.OnEventTriggered -= HandleOnQuestCompleted;
        questStartedEvent.OnEventTriggered -= HandleOnQuestStarted;
        moneyChangedEvent.OnEventTriggered -= HandleOnMoneyChanged;
        ammoChangedEvent.OnEventTriggered -= HandleOnAmmoChanged;
        healthChangedEvent.OnEventTriggered -= HandleOnHealthChanged;
        saveGameRequestedEvent.OnEventTriggered -= HandleOnSaveRequested;
        clearSaveRequestedEvent.OnEventTriggered -= HandleOnClearSaveRequested;
    }

    ////////////////////
    // Event Handlers //
    ////////////////////
    
    private void HandleOnSaveRequested()
    {
        SaveManager sm = GameObject.FindGameObjectWithTag(Constants.Tags.SAVE_MANAGER).GetComponent<SaveManager>();
        sm.Save(currentPlayerData, currentQuestStartedData);
        saveExistsChangedEvent.TriggerEvent(true);
        Debug.Log("SAVE");
    }
    private void HandleOnClearSaveRequested()
    {
        SaveManager sm = GameObject.FindGameObjectWithTag(Constants.Tags.SAVE_MANAGER).GetComponent<SaveManager>();
        sm.Clear();
        currentPlayerData = ResetPlayerData();
        currentQuestStartedData = ResetQuestsData();
        saveExistsChangedEvent.TriggerEvent(false);

        CanvasManager cm = GameObject.FindGameObjectWithTag(Constants.Tags.CANVAS_MANAGER).GetComponent<CanvasManager>();
        cm.questPanel.ResetQuestUI();

        // TEMP
        Debug.Log("CLEAR SAVE");
        testScene.FixButtons(currentPlayerData, currentQuestStartedData);
    }

    private void HandleOnAmmoChanged(int ammo)
    {
        currentPlayerData.ammo = ammo;
    }
    private void HandleOnHealthChanged(int health)
    {
        currentPlayerData.health = health;
    }
    private void HandleOnMoneyChanged(int money)
    {
        currentPlayerData.money = money;
    }
    
    private void HandleOnQuestStarted(EnumQuest startedQuest)
    {
        CanvasManager cm = GameObject.FindGameObjectWithTag(Constants.Tags.CANVAS_MANAGER).GetComponent<CanvasManager>();
        if (cm == null)
        {
            Debug.LogError("No reference to CanvasManager");
            return;
        }

        switch (startedQuest)
        {
            case EnumQuest.SUNGLASSES:
                if (currentQuestStartedData.sunglasses) { Debug.LogWarning("Quest already started"); return; }
                else { currentQuestStartedData.sunglasses = true; }
                break;
            case EnumQuest.GAS_CAN:
                if (currentQuestStartedData.gasCan) { Debug.LogWarning("Quest already started"); return; }
                else { currentQuestStartedData.gasCan = true; }
                break;
            case EnumQuest.MATCHES:
                if (currentQuestStartedData.matches) { Debug.LogWarning("Quest already started"); return; }
                else { currentQuestStartedData.matches = true; }
                break;
            case EnumQuest.FINAL:
                if (currentQuestStartedData.final) { Debug.LogWarning("Quest already started"); return; }
                else { currentQuestStartedData.final = true; }
                break;
            default:
                return;
        }
        cm.questPanel.StartQuest(startedQuest);
    }

    private void HandleOnQuestCompleted(EnumQuest completedQuest)
    {
        CanvasManager cm = GameObject.FindGameObjectWithTag(Constants.Tags.CANVAS_MANAGER).GetComponent<CanvasManager>();
        if (cm == null)
        {
            Debug.LogError("No reference to CanvasManager");
            return;
        }
        cm.questPanel.FinishQuest(completedQuest);


        switch (completedQuest)
        {
            case EnumQuest.GAS_CAN:
                // update player data
                currentPlayerData.hasGasCan = true;
                // update the inventory UI
                break;
            case EnumQuest.MATCHES:
                // update player data
                currentPlayerData.hasMatches = true;
                // update the inventory UI
                break;
            case EnumQuest.SUNGLASSES:
                // update player data
                currentPlayerData.hasSunglasses = true;
                // update the inventory UI
                break;
            default:
                return;
        }
        // Save the new player data to the persistent data store
    }
}
