using UnityEngine;
namespace Events
{
    [CreateAssetMenu(fileName = "GameObjectPayloadEvent", menuName = "Event Channels/GameObject Payload Event")]
    public class GameObjectPayloadEvent : ScriptableObject
    {
        public event System.Action<GameObject> OnEventTriggered;
        public void TriggerEvent(GameObject payload)
        {
            OnEventTriggered?.Invoke(payload);
        }
    }
}