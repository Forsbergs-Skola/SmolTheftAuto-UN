using UnityEngine;
namespace Events
{
    [CreateAssetMenu(fileName = "EnumQuestPayloadEvent", menuName = "Event Channels/EnumQuest Payload Event")]
    public class EnumQuestPayloadEvent : ScriptableObject
    {
        public event System.Action<EnumQuest> OnEventTriggered;
        public void TriggerEvent(EnumQuest payload)
        {
            OnEventTriggered?.Invoke(payload);
        }
    }
}