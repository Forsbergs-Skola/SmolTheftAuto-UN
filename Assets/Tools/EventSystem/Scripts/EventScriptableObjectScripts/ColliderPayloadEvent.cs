using UnityEngine;
namespace Events
{
    [CreateAssetMenu(fileName = "ColliderPayloadEvent", menuName = "Event Channels/Collider Payload Event")]
    public class ColliderPayloadEvent : ScriptableObject
    {
        public event System.Action<Collider> OnEventTriggered;
        public void TriggerEvent(Collider payload)
        {
            OnEventTriggered?.Invoke(payload);
        }
    }
}