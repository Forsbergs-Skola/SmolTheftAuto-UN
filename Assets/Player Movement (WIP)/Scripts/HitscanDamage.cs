using UnityEngine;

public class HitscanDamage : MonoBehaviour
{
    public float damage;
    public float gunRange;
    public Camera cam;
    
    public void Shoot()
    {
        Ray gunRaycast = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        
        if (Physics.Raycast(gunRaycast, out RaycastHit hitInfo, gunRange))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out Enemy enemy))
            {
                enemy.Health -= damage;
            }
        }
    }
}
