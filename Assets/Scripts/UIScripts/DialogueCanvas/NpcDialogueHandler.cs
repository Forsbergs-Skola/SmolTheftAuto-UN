using UnityEngine;
using Events;
using GameTools;

public class NpcDialogueHandler : MonoBehaviour
{

    [SerializeField] private EnumQuest quest;
    [SerializeField] private StringPayloadEvent dialogueStartEvent;
    [SerializeField] private StringPayloadEvent dialogueFinishEvent;
    [SerializeField] private EnumQuestPayloadEvent startQuestEvent;

    private string questStartConvoName;
    private string questFlavorConvoName;
    private string questFinishConvoName;
    private bool myQuestStarted;
    private bool myQuestFinished;

    [SerializeField] private bool testMode = false;

    private GameManagerSingleton gm;
    private Collider myCollider;

    public EnumQuest Quest { get => quest; }
    public string QuestStartConvoName { get => questStartConvoName; }
    public string QuestFlavorConvoName { get => questFlavorConvoName; }
    public string QuestFinishConvoName { get => questFinishConvoName; }

    private void Awake()
    {
        if (testMode)
        {
            GetComponent<Collider>().enabled = false;
        }
        myCollider = GetComponent<Collider>();
    }

    
    private void OnEnable()
    {
        dialogueFinishEvent.OnEventTriggered += HandleDialogueFinished;
    }
    private void OnDisable()
    {
        dialogueFinishEvent.OnEventTriggered -= HandleDialogueFinished;
    }
    


    private void Start()
    {
        if (GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER) == null)
        {
            GetComponent<Collider>().enabled = false;
            return;
        }
        gm = GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>();

        switch (quest)
        {
            case EnumQuest.GAS_CAN:
                questStartConvoName = "GAS_CAN_START";
                questFlavorConvoName = "GAS_CAN_FLAVOR";
                questFinishConvoName = "GAS_CAN_FINISH";
                break;
            case EnumQuest.MATCHES:
                questStartConvoName = "MATCHES_START";
                questFlavorConvoName = "MATCHES_FLAVOR";
                questFinishConvoName = "MATCHES_FINISH";
                break;
            case EnumQuest.SUNGLASSES:
                questStartConvoName = "SUNGLASSES_START";
                questFlavorConvoName = "SUNGLASSES_FLAVOR";
                questFinishConvoName = "SUNGLASSES_FINISH";
                break;
            default:
                Destroy(this);
                return;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!GetIsPlayer(other)) return;
        myCollider.enabled = false;

        if (!GetMyQuestStarted())
        {
            dialogueStartEvent.TriggerEvent(questStartConvoName);
            startQuestEvent.TriggerEvent(quest);
        }
        else if (!gm.GetIsQuestCriteriaMet(quest))
        {
            dialogueStartEvent.TriggerEvent(questFlavorConvoName);
        }
        else
        {
            dialogueStartEvent.TriggerEvent(questFinishConvoName);
            Destroy(gameObject);
        }
    }

    private bool GetIsPlayer(Collider _coll)
    {
        if (_coll.gameObject.GetComponent<UiTestPlayer>() != null) return true;
        if (_coll.gameObject.GetComponent<PlayerController>() != null) return true;
        return false;
    }


    public bool GetMyQuestStarted()
    {
        switch (quest)
        {
            case EnumQuest.GAS_CAN:
                return gm.CurrentQuestStartedData.gasCan;
            case EnumQuest.MATCHES:
                return gm.CurrentQuestStartedData.matches;
            case EnumQuest.SUNGLASSES:
                return gm.CurrentQuestStartedData.sunglasses;
        }
        return false;
    }
    public bool GetMyQuestFinished()
    {
        switch (quest)
        {
            case EnumQuest.GAS_CAN:
                return gm.CurrentPlayerData.hasGasCan;
            case EnumQuest.MATCHES:
                return gm.CurrentPlayerData.hasMatches;
            case EnumQuest.SUNGLASSES:
                return gm.CurrentPlayerData.hasSunglasses;
        }
        return false;
    }

    
    private void HandleDialogueFinished(string _str)
    {
        StartCoroutine(WaitThenReenable());
    }
    private System.Collections.IEnumerator WaitThenReenable()
    {
        yield return new WaitForSeconds(5.0f);
        myCollider.enabled = true;
    }

}
