using UnityEngine;
namespace Events
{
    [CreateAssetMenu(fileName = "Vector3PayloadEvent", menuName = "Event Channels/Vector3 Payload Event")]
    public class Vector3PayloadEvent : ScriptableObject
    {
        public event System.Action<Vector3> OnEventTriggered;
        public void TriggerEvent(Vector3 payload)
        {
            OnEventTriggered?.Invoke(payload);
        }
    }
}