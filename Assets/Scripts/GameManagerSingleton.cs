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

    [Header("Max Resources")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int maxAmmo = 100;
    [SerializeField] private int maxMoney = 1000000;
    [SerializeField] private int maxGrenades = 20;
    
    [Header("For Testing -- Remove later")]
    [SerializeField] private TestScene testScene;

    [Header("Quest Event Channels")]
    [SerializeField] private EnumQuestPayloadEvent questCompletedEvent;
    [SerializeField] private EnumQuestPayloadEvent questStartedEvent;

    [Header("Gameplay Events")]
    [SerializeField] private IntPayloadEvent moneyChangedEvent;
    [SerializeField] private IntPayloadEvent ammoChangedEvent;
    [SerializeField] private IntPayloadEvent grenadesChangedEvent;
    [SerializeField] private IntPayloadEvent healthChangedEvent;
    [SerializeField] private EmptyPayloadEvent npcKilledEvent;
    [SerializeField] private EmptyPayloadEvent checkpointReachedEvent;

    [Header("Dialogue Events")]
    [SerializeField] private StringPayloadEvent dialogueStartedEvent;
    //[SerializeField] private EmptyPayloadEvent dialogueEndedEvent;
    [SerializeField] private StringPayloadEvent dialogueEndedEvent;
    [SerializeField] private EmptyPayloadEvent dialogueAdvancedEvent;

    [Header("Save Game Events")]
    [SerializeField] private EmptyPayloadEvent saveGameRequestedEvent;
    [SerializeField] private EmptyPayloadEvent clearSaveRequestedEvent;
    [SerializeField] private BoolPayloadEvent saveExistsChangedEvent;



    private PlayerData currentPlayerData;
    private QuestStartedData currentQuestStartedData;
    //private QuestFinishedData currentQuestFinishedData;

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
            currentQuestStartedData = sm.ConvertQuestStarted(data);
            //currentQuestFinishedData = sm.ConvertQuestFinished(data);
            saveExistsChangedEvent.TriggerEvent(true);
        }
        else
        {
            currentPlayerData = ResetPlayerData();
            currentQuestStartedData = ResetQuestStaredData();
            //currentQuestFinishedData = ResetQuestFinishedData();
            saveExistsChangedEvent.TriggerEvent(false);
        }
        CanvasManager? cm = GetCanvasManager();
        if (cm != null)
        {
            cm.questPanel.InitializeQuestUI(currentPlayerData, currentQuestStartedData);

            // testing logic -- remove later

            cm.DisplayCanvas(EnumCanvasName.HUD);







            //testScene = null;
            if (testScene!= null) { testScene.FixButtons(currentPlayerData, currentQuestStartedData); }
        }
    }

    private PlayerData ResetPlayerData()
    {
        return new PlayerData(0, 0, 0, MAX_HEALTH, 0, 0, false, false, false);
    }
    private QuestStartedData ResetQuestStaredData()
    {
        return new QuestStartedData(false, false, false, false);
    }
    //private QuestFinishedData ResetQuestFinishedData()
    //{
    //   return new QuestFinishedData(false, false, false, false);
    //}

    private void OnEnable()
    {
        questCompletedEvent.OnEventTriggered += HandleOnQuestCompleted;
        questStartedEvent.OnEventTriggered += HandleOnQuestStarted;
        moneyChangedEvent.OnEventTriggered += HandleOnMoneyChanged;
        ammoChangedEvent.OnEventTriggered += HandleOnAmmoChanged;
        grenadesChangedEvent.OnEventTriggered += HandleOnGrenadesChanged;
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
        grenadesChangedEvent.OnEventTriggered -= HandleOnGrenadesChanged;
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
        currentQuestStartedData = ResetQuestStaredData();
        //currentQuestFinishedData = ResetQuestFinishedData();
        saveExistsChangedEvent.TriggerEvent(false);

        CanvasManager? cm = GetCanvasManager();
        if (cm != null) { cm.questPanel.ResetQuestUI(); }

        // TEMP
        if (testScene != null) { testScene.FixButtons(currentPlayerData, currentQuestStartedData); }
        
    }
    private void HandleOnGrenadesChanged(int grenades)
    {
        if (currentPlayerData.granades + grenades < 0) { currentPlayerData.granades = 0; return; }
        if (currentPlayerData.granades + grenades > maxGrenades) { currentPlayerData.granades = maxGrenades; return; }
        currentPlayerData.granades += grenades;
    }

    private void HandleOnAmmoChanged(int ammo)
    {
        if (currentPlayerData.ammo + ammo < 0) { currentPlayerData.ammo = 0; return; }
        if (currentPlayerData.ammo + ammo > maxAmmo) { currentPlayerData.ammo = maxAmmo; return; }
        currentPlayerData.ammo += ammo;
    }
    private void HandleOnMoneyChanged(int moneyAdded)
    {
        if (currentPlayerData.money + moneyAdded < 0) { currentPlayerData.money = 0; return; }
        if (currentPlayerData.money + moneyAdded > maxMoney) { currentPlayerData.money = maxMoney; return; }
        currentPlayerData.money += moneyAdded;
    }
    private void HandleOnHealthChanged(int health)
    {
        if (currentPlayerData.health + health < 0)
        {
            currentPlayerData.health = 0;
            // HANDLE PLAYER DIES
            return;
        }
        currentPlayerData.health += health;
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
                //currentQuestFinishedData.gasCan = true;

                // update the inventory UI
                break;
            case EnumQuest.MATCHES:
                // update player data
                currentPlayerData.hasMatches = true;
                //currentQuestFinishedData.matches = true;

                // update the inventory UI
                break;
            case EnumQuest.SUNGLASSES:
                // update player data
                currentPlayerData.hasSunglasses = true;
                //currentQuestFinishedData.sunglasses = true;

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
    private void HandleOnFinishDialogue(string convoName)
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
        
        switch (convoName)
        {
            case "SUNGLASSES_FINISH":
                questCompletedEvent.TriggerEvent(EnumQuest.SUNGLASSES);
                break;
            case "GAS_CAN_FINISH":
                questCompletedEvent.TriggerEvent(EnumQuest.GAS_CAN);
                break;
            case "MATCHES_FINISH":
                questCompletedEvent.TriggerEvent(EnumQuest.MATCHES);
                break;
            default:
                break;

        }
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
