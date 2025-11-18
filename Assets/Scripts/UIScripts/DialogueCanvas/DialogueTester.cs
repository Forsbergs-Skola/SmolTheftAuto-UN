using UnityEngine;
using GameTools;
using Events;

public class DialogueTester : MonoBehaviour
{
    [SerializeField] private StringPayloadEvent dialogueStartedEvent;


    public void HandleOnTestPressed(string convoName)
    {
        dialogueStartedEvent.TriggerEvent(convoName);
    }
}
