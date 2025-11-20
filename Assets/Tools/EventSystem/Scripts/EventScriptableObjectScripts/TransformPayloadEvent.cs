using UnityEngine;
namespace Events
{
    [CreateAssetMenu(fileName = "TransformPayloadEvent", menuName = "Event Channels/Transform Payload Event")]
    public class TransformPayloadEvent : ScriptableObject
    {
        public event System.Action<Transform> OnEventTriggered;
        public void TriggerEvent(Transform payload)
        {
            OnEventTriggered?.Invoke(payload);
        }
    }
}