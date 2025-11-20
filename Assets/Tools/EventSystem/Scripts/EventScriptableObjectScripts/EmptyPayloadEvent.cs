using UnityEngine;
namespace Events
{
    [CreateAssetMenu(fileName = "EmptyPayloadEvent", menuName = "Event Channels/Empty Payload Event")]
    public class EmptyPayloadEvent : ScriptableObject
    {
        public event System.Action OnEventTriggered;
        public void TriggerEvent()
        {
            OnEventTriggered?.Invoke();
        }
    }
}