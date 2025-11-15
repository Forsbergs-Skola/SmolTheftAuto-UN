using UnityEngine;
using GameTools;
using Events;


public enum EnumQuest
{
    GAS_CAN,
    SUNGLASSES,
    MATCHES
}

public struct PlayerData
{
    int money;
    int ammo;
    int health;

    bool hasGasCan;
    bool hasMatches;
    bool hasSunglasses;
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

public struct QuestData
{

}

public class GameManagerSingleton : MonoBehaviour
{

    [Header("Event Channels")]
    [SerializeField] private EnumQuestPayloadEvent questCompletedEvent;
    [SerializeField] private EnumQuestPayloadEvent questStartedEvent;
    [SerializeField] private IntPayloadEvent moneyChangedEvent;
    [SerializeField] private IntPayloadEvent ammoChangedEvent;
    [SerializeField] private IntPayloadEvent healthChangedEvent;

    private void Awake()
    {
        if (GameObject.FindGameObjectsWithTag(Constants.Tags.GAME_MANAGER).Length > 0) { Destroy(gameObject); }
        tag = Constants.Tags.GAME_MANAGER;
        DontDestroyOnLoad(gameObject);
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
        switch (startedQuest)
        {
            case EnumQuest.GAS_CAN:
                // update pause UI
                break;
            case EnumQuest.MATCHES:
                // update pause UI
                break;
            case EnumQuest.SUNGLASSES:
                // update pause UI
                break;
            default:
                return;
        }
    }

    private void HandleOnQuestCompleted(EnumQuest completedQuest)
    {
        switch (completedQuest)
        {
            case EnumQuest.GAS_CAN:
                // update player data
                // update quest UI
                // update the inventory UI
                break;
            case EnumQuest.MATCHES:
                // update player data
                // update quest UI
                // update the inventory UI
                break;
            case EnumQuest.SUNGLASSES:
                // update player data
                // update quest UI
                // update the inventory UI
                break;
            default:
                return;
        }
        // Save the new player data to the persistent data store
    }
}
