using UnityEngine;
namespace Events
{
    [CreateAssetMenu(fileName = "Vector2PayloadEvent", menuName = "Event Channels/Vector2 Payload Event")]
    public class Vector2PayloadEvent : ScriptableObject
    {
        public event System.Action<Vector2> OnEventTriggered;
        public void TriggerEvent(Vector2 payload)
        {
            OnEventTriggered?.Invoke(payload);
        }
    }
}