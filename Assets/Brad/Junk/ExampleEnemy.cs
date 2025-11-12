using UnityEngine;
using Events;
using GameTools;
public class ExampleEnemy : MonoBehaviour
{

    [SerializeField] private FloatPayloadEvent hitEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag(Constants.Tags.BULLET)) { return; }
        ExampleBullet bullet = other.gameObject.GetComponent<ExampleBullet>();
        float damageTaken = bullet.damage;
        bullet.DieEarly();
        hitEvent.TriggerEvent(damageTaken);
    }
}
