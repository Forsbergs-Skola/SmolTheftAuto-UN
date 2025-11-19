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
    public const int MAX_HEALTH = 100;

    [SerializeField] private SaveManager sm;
    
    [Header("For Testing -- Remove later")]
    [SerializeField] private TestScene testScene;

    [Header("Quest Event Channels")]
    [SerializeField] private EnumQuestPayloadEvent questCompletedEvent;
    [SerializeField] private EnumQuestPayloadEvent questStartedEvent;

    [Header("Gameplay Events")]
    [SerializeField] private IntPayloadEvent moneyChangedEvent;
    [SerializeField] private IntPayloadEvent ammoChangedEvent;
    [SerializeField] private IntPayloadEvent healthChangedEvent;
    [SerializeField] private EmptyPayloadEvent npcKilledEvent;
    [SerializeField] private EmptyPayloadEvent checkpointReachedEvent;

    [Header("Dialogue Events")]
    [SerializeField] private StringPayloadEvent dialogueStartedEvent;
    [SerializeField] private EmptyPayloadEvent dialogueEndedEvent;
    [SerializeField] private EmptyPayloadEvent dialogueAdvancedEvent;

    [Header("Save Game Events")]
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
        CanvasManager? cm = GetCanvasManager();
        if (cm != null)
        {
            cm.questPanel.InitializeQuestUI(currentPlayerData, currentQuestStartedData);

            // testing logic -- remove later

            cm.DisplayCanvas(EnumCanvasName.HUD);







            testScene = null;
            if (testScene!= null) { testScene.FixButtons(currentPlayerData, currentQuestStartedData); }
        }
    }

    private PlayerData ResetPlayerData()
    {
        return new PlayerData(0, 0, MAX_HEALTH, 0, 0, false, false, false);
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
        dialogueStartedEvent.OnEventTriggered += HandleOnStartDialogue;
        dialogueEndedEvent.OnEventTriggered += HandleOnFinishDialogue;

        checkpointReachedEvent.OnEventTriggered += HandleOnCheckpointReached;
        npcKilledEvent.OnEventTriggered += HandleOnNpcKilled;

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
        dialogueStartedEvent.OnEventTriggered -= HandleOnStartDialogue;
        dialogueEndedEvent.OnEventTriggered -= HandleOnFinishDialogue;

        checkpointReachedEvent.OnEventTriggered -= HandleOnCheckpointReached;
        npcKilledEvent.OnEventTriggered -= HandleOnNpcKilled;
    }

    ////////////////////
    // Event Handlers //
    ////////////////////
    
    private void HandleOnSaveRequested()
    {
        sm.Save(currentPlayerData, currentQuestStartedData);
        saveExistsChangedEvent.TriggerEvent(true);
    }
    private void HandleOnClearSaveRequested()
    {
        sm.Clear();
        currentPlayerData = ResetPlayerData();
        currentQuestStartedData = ResetQuestsData();
        saveExistsChangedEvent.TriggerEvent(false);

        CanvasManager? cm = GetCanvasManager();
        if (cm != null) { cm.questPanel.ResetQuestUI(); }

        // TEMP
        if (testScene != null) { testScene.FixButtons(currentPlayerData, currentQuestStartedData); }
        
    }

    private void HandleOnAmmoChanged(int ammo)
    {
        currentPlayerData.ammo = ammo;
    }
    private void HandleOnHealthChanged(int health)
    {
        currentPlayerData.health = health;
    }
    private void HandleOnMoneyChanged(int moneyAdded)
    {
        currentPlayerData.money += moneyAdded;
    }
    
    private void HandleOnQuestStarted(EnumQuest startedQuest)
    {
        CanvasManager? cm = GetCanvasManager();
        if (cm == null) { Debug.LogError("No CM"); return; }

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
        CanvasManager? cm = GetCanvasManager();
        if (cm == null) { Debug.LogError("No CM"); return; }
        cm.questPanel.FinishQuest(completedQuest);

        cm.ActivateMissionPassed();

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
    }

    private void HandleOnStartDialogue(string conversationName)
    {
        CanvasManager cm = GetCanvasManager();
        if (cm.CurrentActiveCanvas == EnumCanvasName.DIALOGUE)
        {
            Debug.LogWarning("There is already an active dialogue");
            return;
        }

        // check if conversationName triggers any "quest started" or "quest finished" events
        // if so, trigger them
        // Keep ^^that data in GameTools.Constants maybe

        ////////////////////
        // PAUSE gameplay //
        ////////////////////
        // ...TODO

        cm.StartDialogue(conversationName);
    }
    private void HandleOnFinishDialogue()
    {
        CanvasManager cm = GetCanvasManager();
        if (cm.CurrentActiveCanvas != EnumCanvasName.DIALOGUE)
        {
            Debug.LogWarning("Something weird happened");
            return;
        }

        ///////////////////////
        // Un-PAUSE gameplay //
        ///////////////////////
        // ...TODO

        cm.FinishDialogue();

    }

    private void HandleOnNpcKilled()
    {
        currentPlayerData.npcsKilled += 1;
    }
    private void HandleOnCheckpointReached()
    {
        currentPlayerData.checkpointsReached += 1;
    }




    private CanvasManager? GetCanvasManager()
    {
        return GameObject.FindGameObjectWithTag(Constants.Tags.CANVAS_MANAGER).GetComponent<CanvasManager>();
    }
}
