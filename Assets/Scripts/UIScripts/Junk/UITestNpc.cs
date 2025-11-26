using UnityEngine;
using GameTools;
using Events;

public class UITestNpc : MonoBehaviour
{

    [SerializeField] private StringPayloadEvent dialogueStartEvent;
    [SerializeField] private StringPayloadEvent dialogueFinishedEvent;
    [SerializeField] private EnumQuestPayloadEvent startQuestEvent;

    [SerializeField] private bool scootLeft = true;

    //[SerializeField] private string testDialogueName;

    private bool isEnabled = true;
    private GameManagerSingleton gm;
    //private NpcDialogueHandler dialogueHandler;


    private void Awake()
    {
        //dialogueHandler = GetComponent<NpcDialogueHandler>();
    }

    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>();
    }


    private void OnTriggerEnter(Collider other)
    {

        // if I don't have a dialogue handler, there is no dialogue.
        NpcDialogueHandler dialogueHandler = GetComponent<NpcDialogueHandler>();
        if (dialogueHandler == null) { return; }

        if (!isEnabled) return;
        if (other.gameObject.GetComponent<UiTestPlayer>())
        {
            isEnabled = false;
            Vector3 scootVector = Vector3.right * 1.5f;
            if (scootLeft) { scootVector *= -1; }

            other.gameObject.transform.position -= scootVector;

            // if my quest is not started, serve up the quest started dialogue
            // else...
            //     if the quest completion criteria not met, serve up the flavor dialogue
            //     else serve up the quest finished dialogue, and destroy the dialogue handler.

            if (!dialogueHandler.GetMyQuestStarted())
            {
                dialogueStartEvent.TriggerEvent(dialogueHandler.QuestStartConvoName);
                startQuestEvent.TriggerEvent(dialogueHandler.Quest);
            }
            else if (!gm.GetIsQuestCriteriaMet(dialogueHandler.Quest))
            {
                dialogueStartEvent.TriggerEvent(dialogueHandler.QuestFlavorConvoName);
            }
            else
            {
                dialogueStartEvent.TriggerEvent(dialogueHandler.QuestFinishConvoName);
                Destroy(dialogueHandler);
            }



            //dialogueStartEvent.TriggerEvent(testDialogueName);
        }
    }

    private void OnEnable()
    {
        dialogueFinishedEvent.OnEventTriggered += HandleDialogueFinished;
    }
    private void OnDisable()
    {
        dialogueFinishedEvent.OnEventTriggered -= HandleDialogueFinished;
    }


    private void HandleDialogueFinished(string _str)
    {
        isEnabled = true;
    }

}
