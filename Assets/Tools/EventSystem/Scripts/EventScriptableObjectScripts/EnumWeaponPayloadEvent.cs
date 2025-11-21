using UnityEngine;
namespace Events
{
    [CreateAssetMenu(fileName = "EnumWeaponPayloadEvent", menuName = "Event Channels/EnumWeapon Payload Event")]
    public class EnumWeaponPayloadEvent : ScriptableObject
    {
        public event System.Action<EnumWeapon> OnEventTriggered;
        public void TriggerEvent(EnumWeapon payload)
        {
            OnEventTriggered?.Invoke(payload);
        }
    }
}