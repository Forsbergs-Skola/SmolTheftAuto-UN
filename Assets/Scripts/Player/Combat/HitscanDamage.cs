using SmolTheftAuto.NPCs.Behavior;
using UnityEngine;
using Vehicles; // <-- for VehicleHealth

public class HitscanDamage : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float gunRange;
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject hitEffect;
    
    public void Shoot()
    {
        Ray gunRaycast = cam.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f));

        if (Physics.Raycast(gunRaycast, out RaycastHit hitInfo, gunRange))
        {
            HitEffect(hitInfo);
            
            //Firstly, check if we hit an NPC
            if (hitInfo.collider.TryGetComponent(out NPCHealth enemy))
            {
                enemy.Health -= damage;
                return; // we hit an NPC
            }

            // If not a NPC, see if we hit a vehicle
            VehicleHealth vehicleHealth =
                hitInfo.collider.GetComponentInParent<VehicleHealth>();

            if (vehicleHealth != null)
            {
                vehicleHealth.ApplyDamage(damage);
                Debug.Log("Hit vehicle");
                return;// we hit a vehicle
            }

            
        }
    }

    private void HitEffect(RaycastHit hit)
    {
        GameObject effect = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(effect, 2f);
    }
}
