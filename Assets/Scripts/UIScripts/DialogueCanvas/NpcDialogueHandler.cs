using UnityEngine;
using GameTools;

public class NpcDialogueHandler : MonoBehaviour
{

    [SerializeField] private EnumQuest quest;
    private string questStartConvoName;
    private string questFlavorConvoName;
    private string questFinishConvoName;
    private bool myQuestStarted;
    private bool myQuestFinished;


    public EnumQuest Quest { get => quest; }
    public string QuestStartConvoName { get => questStartConvoName; }
    public string QuestFlavorConvoName { get => questFlavorConvoName; }
    public string QuestFinishConvoName { get => questFinishConvoName; }

    private void Start()
    {
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

    public bool GetMyQuestStarted()
    {
        GameManagerSingleton gm = GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>();
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
        GameManagerSingleton gm = GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>();
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

    

}
