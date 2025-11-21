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
    NONE
}

public class GameManagerSingleton : MonoBehaviour
{
    public const int MAX_HEALTH = 100;

    [SerializeField] private SaveManager sm;

    [Header("Max Resources")]
    [SerializeField] private int maxHealth = 100;
    //[SerializeField] private int maxAmmo = 100;
    [SerializeField] private int maxMoney = 1000000;
    [SerializeField] private int maxGrenades = 20;

    [Header("Clip Capacities")]
    [SerializeField] private int rifleClipCapacity = 20;
    [SerializeField] private int pistolClipCapacity = 10;
    [SerializeField] private int shotgunClipCapacity = 5;

    [Header("Ammo pickup values")]
    [SerializeField] private int riflePickup = 20;
    [SerializeField] private int pistolPickup = 10;
    [SerializeField] private int shotgunPickup = 5;
    
    [Header("For Testing -- Remove later")]
    [SerializeField] private TestScene testScene;

    [Header("Quest Event Channels")]
    [SerializeField] private EnumQuestPayloadEvent questCompletedEvent;
    [SerializeField] private EnumQuestPayloadEvent questStartedEvent;

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

    [Header("Dialogue Events")]
    [SerializeField] private StringPayloadEvent dialogueStartedEvent;
    [SerializeField] private StringPayloadEvent dialogueEndedEvent;
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
            cm.DisplayCanvas(EnumCanvasName.HUD);



            testScene = null;
            if (testScene!= null) { testScene.FixButtons(currentPlayerData, currentQuestStartedData); }

            //currentPlayerData.ammo = maxAmmo;
        }
    }

    private PlayerData ResetPlayerData()
    {
        return new PlayerData
            (
                0,0,0,0,0,0,0,0,maxHealth,0,0,false,false,false,EnumWeapon.NONE
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

        //SceneAndPlayerPos sceneData = GetSceneAndPlayerPos();

        sm.Save(currentPlayerData, currentQuestStartedData);
        saveExistsChangedEvent.TriggerEvent(true);
    }
    private void HandleOnClearSaveRequested()
    {
        sm.Clear();
        currentPlayerData = ResetPlayerData();
        currentQuestStartedData = ResetQuestStaredData();
        saveExistsChangedEvent.TriggerEvent(false);

        CanvasManager? cm = GetCanvasManager();
        if (cm != null) { cm.questPanel.ResetQuestUI(); }


        ///////////////////////////////////
        // TESTING LOGIC -- REMOVE LATER //
        ///////////////////////////////////
        if (testScene != null) { testScene.FixButtons(currentPlayerData, currentQuestStartedData); }
    }
    private void HandleOnGrenadesChanged(int grenades)
    {
        if (currentPlayerData.granades + grenades < 0) { currentPlayerData.granades = 0; return; }
        if (currentPlayerData.granades + grenades > maxGrenades) { currentPlayerData.granades = maxGrenades; return; }
        currentPlayerData.granades += grenades;
    }

    /*
    private void HandleOnAmmoChanged(int ammo)
    {
        if (currentPlayerData.ammo + ammo < 0) { currentPlayerData.ammo = 0; Debug.Log("You got no ammo"); return; }
        if (currentPlayerData.ammo + ammo > maxAmmo) { currentPlayerData.ammo = maxAmmo; return; }
        currentPlayerData.ammo += ammo;
        Debug.Log("Ammo: " + currentPlayerData.ammo);
    }
    */
    private void HandleOnMoneyChanged(int moneyAdded)
    {
        if (currentPlayerData.money + moneyAdded < 0)
        {
            Debug.Log("You don't have enough money!");
            return;
        }
        if (currentPlayerData.money + moneyAdded > maxMoney) { currentPlayerData.money = maxMoney; return; }
        currentPlayerData.money += moneyAdded;

        Debug.Log($"GAME MANAGER says: Player current money = {currentPlayerData.money}");

    }
    private void HandleOnHealthChanged(int health)
    {
        if (currentPlayerData.health + health < 0)
        {
            currentPlayerData.health = 0;
            // TODO: HANDLE PLAYER DIES
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
                currentPlayerData.hasGasCan = true;
                // TODO: update the inventory UI
                break;
            case EnumQuest.MATCHES:
                currentPlayerData.hasMatches = true;
                // TODO: update the inventory UI
                break;
            case EnumQuest.SUNGLASSES:
                currentPlayerData.hasSunglasses = true;
                // TODO: update the inventory UI
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
        // do screen shake stuff
        currentPlayerData.npcsKilled += 1;
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
                reloadedAmount = Mathf.Max(currentPlayerData.pistolTotalAmmo, toReplinish);
                currentPlayerData.pistolTotalAmmo -= reloadedAmount;
                currentPlayerData.pistolInClipAmmo += reloadedAmount;
                break;
            case EnumWeapon.SHOTGUN:
                toReplinish = shotgunClipCapacity - currentPlayerData.shotgunInClipAmmo;
                reloadedAmount = Mathf.Max(currentPlayerData.shotgunTotalAmmo, toReplinish);
                currentPlayerData.pistolTotalAmmo -= reloadedAmount;
                currentPlayerData.pistolInClipAmmo += reloadedAmount;
                break;
        }
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
        }
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
    }
    private void HandleOnWeaponEquipped(EnumWeapon weaponType)
    {
        if (currentPlayerData.equippedWeapon == weaponType) return; // no change
        currentPlayerData.equippedWeapon = weaponType;
    }

    private CanvasManager? GetCanvasManager()
    {
        return GameObject.FindGameObjectWithTag(Constants.Tags.CANVAS_MANAGER).GetComponent<CanvasManager>();
    }
}
