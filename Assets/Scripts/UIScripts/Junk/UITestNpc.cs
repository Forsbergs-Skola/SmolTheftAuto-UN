using UnityEngine;
using GameTools;
using Events;

public class UITestNpc : MonoBehaviour
{

    [SerializeField] private StringPayloadEvent dialogueStartEvent;
    [SerializeField] private EmptyPayloadEvent dialogueFinishedEvent;

    [SerializeField] private string testDialogueName;

    private bool isEnabled = true;
    private GameManagerSingleton gm;
    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!isEnabled) return;
        if (other.gameObject.GetComponent<UiTestPlayer>())
        {
            isEnabled = false;
            other.gameObject.transform.position -= Vector3.right * 1.5f;

            dialogueStartEvent.TriggerEvent(testDialogueName);
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


    private void HandleDialogueFinished()
    {
        isEnabled = true;
    }

}
