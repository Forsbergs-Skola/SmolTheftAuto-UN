using UnityEngine;
using Events;

public class JunkVehicleScript : MonoBehaviour
{
    [SerializeField] private IntPayloadEvent playerHealthEvent;

    //.....

    private void OnVehicleDestroyed()
    {
        playerHealthEvent.TriggerEvent(-1000);
    }
}
