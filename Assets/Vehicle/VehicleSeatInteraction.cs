using UnityEngine;
using Vehicles;

[RequireComponent(typeof(Collider))]
public class VehicleSeatInteraction : MonoBehaviour
{
    [field: SerializeField] public VehicleMover Vehicle { get; private set; }
    [field: SerializeField] public Transform SeatTransform { get; private set; }
//unuking for help to steph


    private void Reset()
    {
        // Make sure this collider is a trigger
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        // Auto-fill references if possible
        if (Vehicle == null)
            Vehicle = GetComponentInParent<VehicleMover>();

        if (SeatTransform == null && Vehicle != null)
        {
            // Try find a child called "Seat" under the vehicle
            var seat = Vehicle.transform.Find("Seat");
            SeatTransform = seat != null ? seat : Vehicle.transform;
        }
    }
}
