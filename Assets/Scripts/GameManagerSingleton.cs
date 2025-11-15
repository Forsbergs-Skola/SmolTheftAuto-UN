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

public struct PlayerData
{
    public int money;
    public int ammo;
    public int health;

    public bool hasGasCan;
    public bool hasMatches;
    public bool hasSunglasses;
    public PlayerData
        (
            int _money,
            int _ammo,
            int _health,
            bool _hasGasCan,
            bool _hasMatches,
            bool _hasSunglasses
        )
    {
        money = _money;
        ammo = _ammo;
        health = _health;
        hasGasCan = _hasGasCan;
        hasMatches = _hasMatches;
        hasSunglasses = _hasSunglasses;
    }
}
public struct QuestStartedData
{
    public bool gasCan;
    public bool sunglasses;
    public bool matches;
    public bool final;

    public QuestStartedData
        (
            bool _gasCan,
            bool _sunglasses,
            bool _matches,
            bool _final
        )
    {
        gasCan = _gasCan;
        sunglasses = _sunglasses;
        matches = _matches;
        final = _final;
    }
}

public class GameManagerSingleton : MonoBehaviour
{

    public const int MAX_HEALTH = 100;

    [Header("Event Channels")]
    [SerializeField] private EnumQuestPayloadEvent questCompletedEvent;
    [SerializeField] private EnumQuestPayloadEvent questStartedEvent;
    [SerializeField] private IntPayloadEvent moneyChangedEvent;
    [SerializeField] private IntPayloadEvent ammoChangedEvent;
    [SerializeField] private IntPayloadEvent healthChangedEvent;


    [HideInInspector] public PlayerData currentPlayerData;
    [HideInInspector] public QuestStartedData currentQuestStartedData;
    private bool savedDataFound = false;

    private void Awake()
    {
        if (GameObject.FindGameObjectsWithTag(Constants.Tags.GAME_MANAGER).Length > 0) { Destroy(gameObject); }
        tag = Constants.Tags.GAME_MANAGER;
        DontDestroyOnLoad(gameObject);

        if (savedDataFound)
        {
            // initialize currentPlayerData from saved 
            // initialize currentQuestStartedData from saved
        }
        else
        {
            currentPlayerData = new PlayerData(0, 0, MAX_HEALTH, false, false, false);
            currentQuestStartedData = new QuestStartedData(false, false, false, false);
        }
    }
    private void Start()
    {
        CanvasManager cm = GameObject.FindGameObjectWithTag(Constants.Tags.CANVAS_MANAGER).GetComponent<CanvasManager>();
        cm.questPanel.InitializeQuestUI(currentPlayerData, currentQuestStartedData);

        questStartedEvent.TriggerEvent(EnumQuest.SUNGLASSES);
        questStartedEvent.TriggerEvent(EnumQuest.MATCHES);
        questStartedEvent.TriggerEvent(EnumQuest.GAS_CAN);

        questCompletedEvent.TriggerEvent(EnumQuest.MATCHES);

    }

    private void OnEnable()
    {
        questCompletedEvent.OnEventTriggered += HandleOnQuestCompleted;
        questStartedEvent.OnEventTriggered += HandleOnQuestStarted;
        moneyChangedEvent.OnEventTriggered += HandleOnMoneyChanged;
        ammoChangedEvent.OnEventTriggered += HandleOnAmmoChanged;
        healthChangedEvent.OnEventTriggered += HandleOnHealthChanged;
    }
    private void OnDisable()
    {
        questCompletedEvent.OnEventTriggered -= HandleOnQuestCompleted;
        questStartedEvent.OnEventTriggered -= HandleOnQuestStarted;
        moneyChangedEvent.OnEventTriggered -= HandleOnMoneyChanged;
        ammoChangedEvent.OnEventTriggered -= HandleOnAmmoChanged;
        healthChangedEvent.OnEventTriggered -= HandleOnHealthChanged;
    }

    ////////////////////
    // Event Handlers //
    ////////////////////

    private void HandleOnAmmoChanged(int ammo)
    {

    }
    private void HandleOnHealthChanged(int health)
    {

    }
    private void HandleOnMoneyChanged(int money)
    {

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
                // update the inventory UI
                break;
            case EnumQuest.MATCHES:
                // update player data
                // update the inventory UI
                break;
            case EnumQuest.SUNGLASSES:
                // update player data
                // update the inventory UI
                break;
            default:
                return;
        }
        // Save the new player data to the persistent data store
    }
}
