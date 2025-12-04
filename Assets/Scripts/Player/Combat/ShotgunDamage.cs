using SmolTheftAuto.NPCs.Behavior;
using UnityEngine;
using Vehicles; // for VehicleHealth
public class ShotgunDamage : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float gunRange = 50f;
    [SerializeField] private int pellets = 8;
    [SerializeField] private float angleOfSpread = 5f;

    [SerializeField] private Camera cam;

    public void Shoot()
    {
        Ray gunRaycast =  cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        Vector3 startDirection = gunRaycast.direction;

        for (int i = 0; i < pellets; i++)
        {
            Vector3 direction = RandomSpreadDirection(startDirection, angleOfSpread);
            //Debug.DrawRay(cam.transform.position, direction * gunRange, Color.red, 10f);
            Ray pelletRaycast = new Ray(cam.transform.position, direction);

            if (Physics.Raycast(pelletRaycast, out RaycastHit hit, gunRange))
                if (hit.collider.TryGetComponent(out NPCHealth enemy))
                   { 
                    enemy.Health -= damage;
                    continue; // Cause multiple pellets can hit the diffrent targets i assume so no return here(ibad)
                   }
       
        VehicleHealth vehicleHealth =
                    hit.collider.GetComponentInParent<VehicleHealth>();

                if (vehicleHealth != null)
                {
                    vehicleHealth.ApplyDamage(damage);
                    continue; 
                }
              } 
            }

    private Vector3 RandomSpreadDirection(Vector3 startDirection, float spreadAngle)
    {
        float spreadRadius = Mathf.Tan(spreadAngle * Mathf.Deg2Rad);
        Vector2 randomPoint = Random.insideUnitCircle * spreadRadius;
        Vector3 spreadDirection = startDirection + cam.transform.right * randomPoint.x + cam.transform.up * randomPoint.y;
        return spreadDirection.normalized;
    }
}
