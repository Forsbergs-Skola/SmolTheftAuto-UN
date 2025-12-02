using UnityEngine;
using Events;

public class EndGameTrigger : MonoBehaviour
{
    [SerializeField] private EnumQuestPayloadEvent questStartedEvent;
    [SerializeField] private EmptyPayloadEvent endgameEvent;

    private Collider myCollider;
    private bool triggerable = false; // <--Prevents redundant triggering

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
        myCollider.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!triggerable) return;
        if (!other.gameObject.CompareTag("Player")) return;

        // Disable the collider and kick off the end game sequence
        triggerable = false; 
        myCollider.enabled = false;
        endgameEvent.TriggerEvent();
    }

    private void OnEnable()
    {
        questStartedEvent.OnEventTriggered += HandleOnQuestStarted;
    }
    private void OnDisable()
    {
        questStartedEvent.OnEventTriggered -= HandleOnQuestStarted;
    }

    private void HandleOnQuestStarted(EnumQuest quest)
    {
        // enable player detection when the final quest begins
        if (quest != EnumQuest.FINAL) return;
        myCollider.enabled = true;
        triggerable = true;
    }
}
