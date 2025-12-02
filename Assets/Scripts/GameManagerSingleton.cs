using UnityEngine;
using UnityEngine.SceneManagement;
using GameTools;
using Events;

public enum EnumQuest
{
    GAS_CAN,
    SUNGLASSES,
    MATCHES,
    FINAL
}

public enum EnumWeapon
{
    PISTOL,
    RIFLE,
    SHOTGUN,
    GRENADE,
    NONE
}

public class GameManagerSingleton : MonoBehaviour
{

    [SerializeField] bool labMode = false;

    [SerializeField] private string gameplaySceneName = "GameplayScene";

    //public const int MAX_HEALTH = 100;

    [SerializeField] private SaveManager sm;

    [Header("Max Resources")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int maxMoney = 1000000;
    [SerializeField] private int maxGrenades = 20;

    [Header("Clip Capacities")]
    [SerializeField] private int rifleClipCapacity = 20;
    [SerializeField] private int pistolClipCapacity = 10;
    [SerializeField] private int shotgunClipCapacity = 5;

    [Header("Store values")]
    [SerializeField] private int ammoRefilCost = 100;
    [SerializeField] private int grenadeRefill = 3;
    

    [Header("Ammo pickup values")]
    [SerializeField] private int riflePickup = 20;
    [SerializeField] private int pistolPickup = 10;
    [SerializeField] private int shotgunPickup = 5;
    [SerializeField] private int grenadePickup = 3;
    
    [Header("For Testing -- Remove later")]
    [SerializeField] private TestScene testScene;

    [Header("Quest Event Channels")]
    [SerializeField] private EnumQuestPayloadEvent questCompletedEvent;
    [SerializeField] private EnumQuestPayloadEvent questStartedEvent;
    [SerializeField] private EmptyPayloadEvent endGameEvent;

    [Header("Menu Events")]
    [SerializeField] private EmptyPayloadEvent newGamePressedEvent;
    [SerializeField] private EmptyPayloadEvent continuePressedEvent;

    [Header("Gameplay Events")]
    [SerializeField] private IntPayloadEvent moneyChangedEvent;
    [SerializeField] private EnumWeaponPayloadEvent ammoDiscargedEvent;
    [SerializeField] private EnumWeaponPayloadEvent ammoPickupEvent;
    [SerializeField] private IntPayloadEvent grenadesChangedEvent;
    [SerializeField] private EnumWeaponPayloadEvent weaponReloadEvent;
    [SerializeField] private IntPayloadEvent healthChangedEvent;
    [SerializeField] private EmptyPayloadEvent npcKilledEvent;
    [SerializeField] private EmptyPayloadEvent checkpointReachedEvent;
    [SerializeField] private EnumWeaponPayloadEvent weaponEquippedEvent;
    [SerializeField] private EmptyPayloadEvent playerDataUpdatedEvent;
    [SerializeField] private EmptyPayloadEvent pausedEvent;

    [Header("Dialogue Events")]
    [SerializeField] private StringPayloadEvent dialogueStartedEvent;
    [SerializeField] private StringPayloadEvent dialogueEndedEvent;
    [SerializeField] private EmptyPayloadEvent dialogueAdvancedEvent;

    [Header("Store Events")]
    [SerializeField] private EmptyPayloadEvent storeInteractionStartedEvent;
    [SerializeField] private EmptyPayloadEvent storeInteractionFinishedEvent;

    [Header("Save Game Events")]
    [SerializeField] private EmptyPayloadEvent saveGameRequestedEvent;
    [SerializeField] private EmptyPayloadEvent clearSaveRequestedEvent;
    [SerializeField] private BoolPayloadEvent saveExistsChangedEvent;

    private PlayerData currentPlayerData;
    private QuestStartedData currentQuestStartedData;

    private bool gameIsPaused = false;

    public PlayerData CurrentPlayerData { get => currentPlayerData; }
    public QuestStartedData CurrentQuestStartedData { get => currentQuestStartedData; }

    public int MaxHealth { get => maxHealth; }

    public int AmmoRefilCost { get => ammoRefilCost; }
    public int GrenadeRefil { get => grenadeRefill; }
    public bool LabMode { get => labMode; }



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
            Debug.Log("FOO");

            SaveData data = sm.Load();
            currentPlayerData = sm.ConvertPlayer(data);
            currentQuestStartedData = sm.ConvertQuestStarted(data);
            saveExistsChangedEvent.TriggerEvent(true);
        }
        else
        {
            currentPlayerData = ResetPlayerData();
            currentQuestStartedData = ResetQuestStaredData();
            saveExistsChangedEvent.TriggerEvent(false);
        }
        CanvasManager? cm = GetCanvasManager();
        if (cm != null)
        {
            cm.questPanel.InitializeQuestUI(currentPlayerData, currentQuestStartedData);

            if (labMode) { cm.DisplayCanvas(EnumCanvasName.HUD); }
            else { cm.DisplayCanvas(EnumCanvasName.MAIN); }

            if (testScene!= null) { testScene.FixButtons(currentPlayerData, currentQuestStartedData); }
        }
        playerDataUpdatedEvent.TriggerEvent();

    }

    private PlayerData ResetPlayerData()
    {
        return new PlayerData
            (
                0,                      // money
                rifleClipCapacity,      // total rifle ammo
                pistolClipCapacity,     // total pistol ammo
                shotgunClipCapacity,    // total shotgun ammo
                rifleClipCapacity -5,                      // rifle in clip
                pistolClipCapacity -3,                      // pistol in clip
                shotgunClipCapacity -2,                      // shotgun in clip
                3,            // grenades
                maxHealth,              // player health
                0,                      // checkpoints reached
                0,                      // NPCs killed
                false,                  // has gas can
                false,                  // has matches
                false,                  // has sunglasses
                EnumWeapon.NONE         // equipped weapon
            );
    }
    private QuestStartedData ResetQuestStaredData()
    {
        return new QuestStartedData(false, false, false, false);
    }

    private void OnEnable()
    {
        questCompletedEvent.OnEventTriggered += HandleOnQuestCompleted;
        questStartedEvent.OnEventTriggered += HandleOnQuestStarted;
        moneyChangedEvent.OnEventTriggered += HandleOnMoneyChanged;
        ammoPickupEvent.OnEventTriggered += HandleAmmoPickup;
        ammoDiscargedEvent.OnEventTriggered += HandleOnAmmoDischarged;
        grenadesChangedEvent.OnEventTriggered += HandleOnGrenadesChanged;
        healthChangedEvent.OnEventTriggered += HandleOnHealthChanged;
        weaponReloadEvent.OnEventTriggered += HandleOnWeaponReloaded;
        saveGameRequestedEvent.OnEventTriggered += HandleOnSaveRequested;
        clearSaveRequestedEvent.OnEventTriggered += HandleOnClearSaveRequested;
        dialogueStartedEvent.OnEventTriggered += HandleOnStartDialogue;
        dialogueEndedEvent.OnEventTriggered += HandleOnFinishDialogue;
        checkpointReachedEvent.OnEventTriggered += HandleOnCheckpointReached;
        npcKilledEvent.OnEventTriggered += HandleOnNpcKilled;
        weaponEquippedEvent.OnEventTriggered += HandleOnWeaponEquipped;
        newGamePressedEvent.OnEventTriggered += HandleOnNewGamePressed;
        continuePressedEvent.OnEventTriggered += HandleContinuePressed;
        pausedEvent.OnEventTriggered += HandleOnGamePausedToggled;
        storeInteractionStartedEvent.OnEventTriggered += HandleOnStoreInteractionStarted;
        storeInteractionFinishedEvent.OnEventTriggered += HandleOnStoreInteractionFinished;
        endGameEvent.OnEventTriggered += HandleOnEndGameEvent;
    }
    private void OnDisable()
    {
        questCompletedEvent.OnEventTriggered -= HandleOnQuestCompleted;
        questStartedEvent.OnEventTriggered -= HandleOnQuestStarted;
        moneyChangedEvent.OnEventTriggered -= HandleOnMoneyChanged;
        ammoPickupEvent.OnEventTriggered -= HandleAmmoPickup;
        ammoDiscargedEvent.OnEventTriggered -= HandleOnAmmoDischarged;
        grenadesChangedEvent.OnEventTriggered -= HandleOnGrenadesChanged;
        healthChangedEvent.OnEventTriggered -= HandleOnHealthChanged;
        weaponReloadEvent.OnEventTriggered -= HandleOnWeaponReloaded;
        saveGameRequestedEvent.OnEventTriggered -= HandleOnSaveRequested;
        clearSaveRequestedEvent.OnEventTriggered -= HandleOnClearSaveRequested;
        dialogueStartedEvent.OnEventTriggered -= HandleOnStartDialogue;
        dialogueEndedEvent.OnEventTriggered -= HandleOnFinishDialogue;
        checkpointReachedEvent.OnEventTriggered -= HandleOnCheckpointReached;
        npcKilledEvent.OnEventTriggered -= HandleOnNpcKilled;
        weaponEquippedEvent.OnEventTriggered -= HandleOnWeaponEquipped;
        newGamePressedEvent.OnEventTriggered -= HandleOnNewGamePressed;
        continuePressedEvent.OnEventTriggered -= HandleContinuePressed;
        pausedEvent.OnEventTriggered -= HandleOnGamePausedToggled;
        storeInteractionStartedEvent.OnEventTriggered -= HandleOnStoreInteractionStarted;
        storeInteractionFinishedEvent.OnEventTriggered -= HandleOnStoreInteractionFinished;
        endGameEvent.OnEventTriggered -= HandleOnEndGameEvent;
    }

    
    public bool GetIsQuestCriteriaMet(EnumQuest quest)
    {
        switch (quest)
        {
            case EnumQuest.GAS_CAN:
                return currentPlayerData.checkpointsReached >= 3;
            case EnumQuest.MATCHES:
                return currentPlayerData.npcsKilled >= 5;
            case EnumQuest.SUNGLASSES:
                return currentPlayerData.money >= 100;
            default:
                return false;
        }
    }
    

    ////////////////////
    // Event Handlers //
    ////////////////////

    private void HandleOnSaveRequested()
    {
        sm.Save(currentPlayerData, currentQuestStartedData);
        saveExistsChangedEvent.TriggerEvent(true);

        //CanvasManager cm = GameObject.FindGameObjectWithTag(Constants.Tags.CANVAS_MANAGER).GetComponent<CanvasManager>();
        //cm.GameSavedFeedback();

    }
    private void HandleOnClearSaveRequested()
    {
        sm.Clear();
        currentPlayerData = ResetPlayerData();
        currentQuestStartedData = ResetQuestStaredData();
        saveExistsChangedEvent.TriggerEvent(false);

        CanvasManager? cm = GetCanvasManager();
        if (cm != null) { cm.questPanel.ResetQuestUI(); }

        playerDataUpdatedEvent.TriggerEvent();


        ///////////////////////////////////
        // TESTING LOGIC -- REMOVE LATER //
        ///////////////////////////////////
        if (testScene != null) { testScene.FixButtons(currentPlayerData, currentQuestStartedData); }
    }
    private void HandleOnGrenadesChanged(int grenades)
    {
        if (currentPlayerData.granades + grenades < 0)
        {
            currentPlayerData.granades = 0;
        }
        else if (currentPlayerData.granades + grenades > maxGrenades)
        {
            currentPlayerData.granades = maxGrenades;
        }
        else
        {
            currentPlayerData.granades += grenades;
        }
        playerDataUpdatedEvent.TriggerEvent();
    }

    private void HandleOnMoneyChanged(int moneyAdded)
    {
        if (currentPlayerData.money + moneyAdded < 0)
        {
            Debug.Log("You don't have enough money!");
            return;
        }
        if (currentPlayerData.money + moneyAdded > maxMoney) { currentPlayerData.money = maxMoney; }
        else { currentPlayerData.money += moneyAdded; }
        playerDataUpdatedEvent.TriggerEvent();

        Debug.Log($"GAME MANAGER says: Player current money = {currentPlayerData.money}");

    }
    private void HandleOnHealthChanged(int health)
    {
        if (currentPlayerData.health <= 0) return;

        if (currentPlayerData.health + health > maxHealth)
        {
            currentPlayerData.health = maxHealth;
            Debug.Log("Already at max health");
            playerDataUpdatedEvent.TriggerEvent();
            return;
        }

        if (currentPlayerData.health + health <= 0)
        {
            currentPlayerData.health = 0;

            CanvasManager cm = GetCanvasManager();
            cm.ActivatePlayerDied();

            Time.timeScale = 0.5f; 

            playerDataUpdatedEvent.TriggerEvent(); // Hud update
            return;
        }
        currentPlayerData.health += health;
        playerDataUpdatedEvent.TriggerEvent();
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
        playerDataUpdatedEvent.TriggerEvent();
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
                currentPlayerData.hasGasCan = true;
                break;
            case EnumQuest.MATCHES:
                currentPlayerData.hasMatches = true;
                break;
            case EnumQuest.SUNGLASSES:
                currentPlayerData.hasSunglasses = true;
                currentPlayerData.money = Mathf.Max(0, currentPlayerData.money - 100); // pay for the glasses
                break;
            default:
                return;
        }
        playerDataUpdatedEvent.TriggerEvent();
    }

    private void HandleOnStartDialogue(string conversationName)
    {
        CanvasManager cm = GetCanvasManager();
        if (cm.CurrentActiveCanvas == EnumCanvasName.DIALOGUE)
        {
            Debug.LogWarning($"{conversationName} -- There is already an active dialogue");
            return;
        }

        Time.timeScale = 0f;

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

        Time.timeScale = 1f;

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
        // do screen shake stuff
        currentPlayerData.npcsKilled += 1;
        playerDataUpdatedEvent.TriggerEvent();
    }
    private void HandleOnCheckpointReached()
    {
        if (!currentQuestStartedData.gasCan)
        {
            Debug.Log("Gas can quest not started yet");
            return;
        }
        if (currentPlayerData.hasGasCan)
        {
            Debug.Log("Gas can quest finished. No longer tracking checkpoints");
            return;
        }

        currentPlayerData.checkpointsReached += 1;
        playerDataUpdatedEvent.TriggerEvent();
        Debug.Log($"Checkpoints reached: {currentPlayerData.checkpointsReached}");
    }

    private void HandleOnWeaponReloaded(EnumWeapon weaponType)
    {
        int toReplinish = -1;
        int reloadedAmount = -1;
        switch (weaponType)
        {
            case EnumWeapon.RIFLE:
                toReplinish = rifleClipCapacity - currentPlayerData.rifleInClipAmmo;
                reloadedAmount = Mathf.Min(currentPlayerData.rifleTotalAmmo, toReplinish);
                currentPlayerData.rifleTotalAmmo -= reloadedAmount;
                currentPlayerData.rifleInClipAmmo += reloadedAmount;
                break;
            case EnumWeapon.PISTOL:
                toReplinish = pistolClipCapacity - currentPlayerData.pistolInClipAmmo;
                reloadedAmount = Mathf.Min(currentPlayerData.pistolTotalAmmo, toReplinish);
                currentPlayerData.pistolTotalAmmo -= reloadedAmount;
                currentPlayerData.pistolInClipAmmo += reloadedAmount;
                break;
            case EnumWeapon.SHOTGUN:
                toReplinish = shotgunClipCapacity - currentPlayerData.shotgunInClipAmmo;
                reloadedAmount = Mathf.Min(currentPlayerData.shotgunTotalAmmo, toReplinish);
                currentPlayerData.shotgunTotalAmmo -= reloadedAmount;
                currentPlayerData.shotgunInClipAmmo += reloadedAmount;
                break;
        }
        playerDataUpdatedEvent.TriggerEvent();
    }
    private void HandleAmmoPickup(EnumWeapon weaponType) // <-- this can come from an actual pickup, or a dialogue option in the store
    {
        switch (weaponType)
        {
            case EnumWeapon.RIFLE:
                currentPlayerData.rifleTotalAmmo += riflePickup;
                break;
            case EnumWeapon.PISTOL:
                currentPlayerData.pistolTotalAmmo += pistolPickup;
                break;
            case EnumWeapon.SHOTGUN:
                currentPlayerData.shotgunTotalAmmo += shotgunPickup;
                break;
            case EnumWeapon.GRENADE:
                currentPlayerData.granades += grenadePickup;
                break;
        }
        playerDataUpdatedEvent.TriggerEvent();
    }
    private void HandleOnAmmoDischarged(EnumWeapon weaponType)
    {
        switch (weaponType)
        {
            case EnumWeapon.RIFLE:
                currentPlayerData.rifleInClipAmmo = Mathf.Max(0, currentPlayerData.rifleInClipAmmo - 1);
                break;
            case EnumWeapon.PISTOL:
                currentPlayerData.pistolInClipAmmo = Mathf.Max(0, currentPlayerData.pistolInClipAmmo - 1);
                break;
            case EnumWeapon.SHOTGUN:
                currentPlayerData.shotgunInClipAmmo = Mathf.Max(0, currentPlayerData.shotgunInClipAmmo - 1);
                break;
        }
        playerDataUpdatedEvent.TriggerEvent();
    }
    private void HandleOnWeaponEquipped(EnumWeapon weaponType)
    {
        if (currentPlayerData.equippedWeapon == weaponType) return; // no change
        currentPlayerData.equippedWeapon = weaponType;
        playerDataUpdatedEvent.TriggerEvent();
    }

    private void HandleOnNewGamePressed()
    {
        if (sm.SaveExists())
        {
            HandleOnClearSaveRequested();
            currentPlayerData = ResetPlayerData(); 
            currentQuestStartedData = ResetQuestStaredData();
            saveExistsChangedEvent.TriggerEvent(false);

            playerDataUpdatedEvent.TriggerEvent();

        }
        LoadGameplayScene();
    }
    private void HandleContinuePressed()
    {
        if (!sm.SaveExists()) { Debug.LogError("This should not happen"); return; }
        LoadGameplayScene();
    }

    private void HandleOnGamePausedToggled()
    {
        CanvasManager cm = GameObject.FindGameObjectWithTag(Constants.Tags.CANVAS_MANAGER).GetComponent<CanvasManager>();
        gameIsPaused = !gameIsPaused;

        if (gameIsPaused)
        {
            cm.DisplayCanvas(EnumCanvasName.PAUSE);
            Time.timeScale = 0.0f;
        }
        else
        {
            cm.DisplayCanvas(EnumCanvasName.HUD);
            Time.timeScale = 1.0f;
        }
    }

    private void HandleOnStoreInteractionStarted()
    {
        CanvasManager cm = GetCanvasManager();
        if (cm.CurrentActiveCanvas == EnumCanvasName.STORE)
        {
            Debug.LogWarning("Store interaction UI is already up");
            return;
        }
        cm.StartStoreInteraction();
        Time.timeScale = 0.0f;
        
    }
    private void HandleOnStoreInteractionFinished()
    {
        CanvasManager cm = GetCanvasManager();
        if (cm.CurrentActiveCanvas != EnumCanvasName.STORE) { Debug.LogError("Something weird happended"); return; }
        Time.timeScale = 1.0f;
        cm.FinishStoreInteraction();
        playerDataUpdatedEvent.TriggerEvent();
    }

    private void HandleOnEndGameEvent()
    {
        Debug.Log("Endgame sequence begins");
        CanvasManager cm = GetCanvasManager();
        cm.ActivateEndgame();
    }

    public void SaveButtonPressed()
    {
        saveGameRequestedEvent.TriggerEvent();
    }
    public void OnMainButtonPressed()
    {
        LoadMain();
    }

    /////////////
    // HELPERS //
    /////////////
    
    public bool SaveExists()
    {
        return sm.SaveExists();
    }

    private void LoadGameplayScene()
    {
        Debug.Log($"Loading scene: {gameplaySceneName}");
        CanvasManager cm = GameObject.FindGameObjectWithTag(Constants.Tags.CANVAS_MANAGER).GetComponent<CanvasManager>();
        cm.ShowAndFadeLoadingScreen();
        SceneManager.LoadScene(gameplaySceneName);
    }

    private void LoadMain()
    {
        Time.timeScale = 1.0f;
        Debug.Log("Loading scene: Bootstrap");
        CanvasManager cm = GameObject.FindGameObjectWithTag(Constants.Tags.CANVAS_MANAGER).GetComponent<CanvasManager>();
        cm.ShowMain();
        SceneManager.LoadScene("Bootstrap");
    }

    private CanvasManager? GetCanvasManager()
    {
        return GameObject.FindGameObjectWithTag(Constants.Tags.CANVAS_MANAGER).GetComponent<CanvasManager>();
    }

    public int GetInClipAmmo(EnumWeapon weapon)
    {
        switch (weapon)
        {
            case EnumWeapon.PISTOL:
                return currentPlayerData.pistolInClipAmmo;
            case EnumWeapon.RIFLE:
                return currentPlayerData.rifleInClipAmmo;
            case EnumWeapon.SHOTGUN:
                return currentPlayerData.shotgunInClipAmmo;
            default: return 0;
        }
    }
    public int GetTotalAmmo(EnumWeapon weapon)
    {
        switch (weapon)
        {
            case EnumWeapon.PISTOL:
                return currentPlayerData.pistolTotalAmmo;
            case EnumWeapon.RIFLE:
                return currentPlayerData.rifleTotalAmmo;
            case EnumWeapon.SHOTGUN:
                return currentPlayerData.shotgunTotalAmmo;
            default: return 0;
        }
    }
}
