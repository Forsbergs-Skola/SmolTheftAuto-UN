using UnityEngine;
using GameTools;
using Events;

public class ExamplePlayerGun : MonoBehaviour
{
    [SerializeField] private EmptyPayloadEvent fireInputEvent;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform spawnTransform;


    private void OnEnable()
    {
        fireInputEvent.OnEventTriggered += IngestFireInput;
    }
    private void OnDisable()
    {
        fireInputEvent.OnEventTriggered -= IngestFireInput;
    }

    private void IngestFireInput()
    {

        Debug.Log("FOOOOOOO");

        
        GameObject bulletObj = Instantiate(bulletPrefab);
        ExampleBullet bullet = bulletObj.GetComponent<ExampleBullet>();
        bullet.transform.position = spawnTransform.position;
        bullet.tag = Constants.Tags.BULLET;
        
    }

}
