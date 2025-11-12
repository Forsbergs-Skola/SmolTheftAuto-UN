using UnityEngine;
namespace Events
{
    [CreateAssetMenu(fileName = "NoPayloadEvent", menuName = "Event Channels/Empty Payload Event")]
    public class NoPayloadEvent : ScriptableObject
    {
        public event System.Action OnEventTriggered;
        public void TriggerEvent()
        {
            OnEventTriggered?.Invoke();
        }
    }
}