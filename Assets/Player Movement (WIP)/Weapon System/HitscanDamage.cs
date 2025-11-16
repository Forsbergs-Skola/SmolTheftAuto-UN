using UnityEngine;

public class HitscanDamage : MonoBehaviour
{
    public float damage;
    public float gunRange;
    public Transform attackPoint;
    
    public void Shoot()
    {
        Ray gunRaycast = new Ray(attackPoint.position, attackPoint.forward);
        if (Physics.Raycast(gunRaycast, out RaycastHit hitInfo, gunRange))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out Enemy enemy))
            {
                enemy.Health -= damage;
            }
        }
    }
}
