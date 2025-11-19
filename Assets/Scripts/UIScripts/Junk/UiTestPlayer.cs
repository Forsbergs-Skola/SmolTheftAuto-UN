using UnityEngine;
using Events;

public class UiTestPlayer : MonoBehaviour
{
    [SerializeField] private Vector2PayloadEvent moveInputEvent;
    [SerializeField] private StringPayloadEvent dialogueStartEvent;
    [SerializeField] private EmptyPayloadEvent dialogueFinishedEvent;
    private void OnEnable()
    {
        moveInputEvent.OnEventTriggered += HandleMoveInput;
        dialogueStartEvent.OnEventTriggered += HandleOnDialogueStarted;
        dialogueFinishedEvent.OnEventTriggered += HandleOnDialogueFinished;
    }

    private void OnDisable()
    {
        moveInputEvent.OnEventTriggered -= HandleMoveInput;
        dialogueStartEvent.OnEventTriggered -= HandleOnDialogueStarted;
        dialogueFinishedEvent.OnEventTriggered -= HandleOnDialogueFinished;
    }

    private void HandleMoveInput(Vector2 moveInput)
    {
        
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb.isKinematic == true) return;
        Vector3 velocity = new Vector3(moveInput.x, 0f, moveInput.y);
        rb.linearVelocity = velocity * 4;
    }

    private void HandleOnDialogueStarted(string _str)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void HandleOnDialogueFinished()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }

}
