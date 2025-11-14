using UnityEngine;
namespace Events
{
    [CreateAssetMenu(fileName = "IntPayloadEvent", menuName = "Event Channels/Integer Payload Event")]
    public class IntPayloadEvent : ScriptableObject
    {
        public event System.Action<int> OnEventTriggered;
        public void TriggerEvent(int payload)
        {
            OnEventTriggered?.Invoke(payload);
        }
    }
}