using UnityEngine;
namespace Events
{
    [CreateAssetMenu(fileName = "BoolPayloadEvent", menuName = "Event Channels/Boolean Payload Event")]
    public class BoolPayloadEvent : ScriptableObject
    {
        public event System.Action<bool> OnEventTriggered;
        public void TriggerEvent(bool payload)
        {
            OnEventTriggered?.Invoke(payload);
        }
    }
}