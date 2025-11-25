using UnityEngine;
using Events;

public class VehicleCheckpoint : MonoBehaviour
{
    [SerializeField] private EmptyPayloadEvent checkpointReachedEvent;
    [SerializeField] private EnumQuestPayloadEvent questStartedEvent;
    [SerializeField] private EnumQuestPayloadEvent questFinishedEvent;

    private MeshRenderer myRenderer;
    private SphereCollider myCollider;
    private bool triggerable = false;

    private void Awake()
    {
        myRenderer = GetComponent<MeshRenderer>();
        myCollider = GetComponent<SphereCollider>();
        enableCheckpoint(false);
    }

    private void OnEnable()
    {
        questStartedEvent.OnEventTriggered += IngestQuestStarted;
        questFinishedEvent.OnEventTriggered += IngestQuestEnded;
    }

    private void OnDisable()
    {
        questStartedEvent.OnEventTriggered -= IngestQuestStarted;
        questFinishedEvent.OnEventTriggered -= IngestQuestEnded;
    }
    private void enableCheckpoint(bool value)
    {
        myRenderer.enabled = value;
        myCollider.enabled = value;
        triggerable = value;
    }
    private void OnTriggerEnter(Collider other)
    {
        // only care if the object is a player-controlled vehicle
        if (other.gameObject.GetComponent<PlayerVehicleController>() == null) return;
        if (!triggerable) return;
        enableCheckpoint(false);
        checkpointReachedEvent.TriggerEvent();

    }

    private void IngestQuestStarted(EnumQuest quest)
    {
        // turn on when the "gas can" quest begins
        if (quest != EnumQuest.GAS_CAN) return;
        enableCheckpoint(true);
    }
    private void IngestQuestEnded(EnumQuest quest)
    {
        // turn off when the "gas can" quest is complete
        if (quest != EnumQuest.GAS_CAN) return;
        enableCheckpoint(false);
    }

}
