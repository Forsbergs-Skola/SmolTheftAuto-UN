using UnityEngine;
using GameTools;
using Events;

public class ExamplePlayerGun : MonoBehaviour
{
    [SerializeField] private EmptyPayloadEvent fireInputEvent;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private BoolPayloadEvent gamePausedEvent;
    [Min(6.0f)][SerializeField] private float maxRoundsPerMinute = 30.0f;

    private bool dampened = false;
    private bool isPaused = false;

    private void OnEnable()
    {
        fireInputEvent.OnEventTriggered += IngestFireInput;
        gamePausedEvent.OnEventTriggered += IngestGamePausedEvent;
    }
    private void OnDisable()
    {
        fireInputEvent.OnEventTriggered -= IngestFireInput;
        gamePausedEvent.OnEventTriggered -= IngestGamePausedEvent;
    }

    private void IngestFireInput()
    {
        if (dampened) { return; }
        if (isPaused) { return; }
        dampened = true;
        StartCoroutine(EnforceRateOfFire());
        GameObject bulletObj = Instantiate(bulletPrefab);
        ExampleBullet bullet = bulletObj.GetComponent<ExampleBullet>();
        bullet.transform.position = spawnTransform.position;
        bullet.tag = Constants.Tags.BULLET;
    }
    private void IngestGamePausedEvent(bool _isPaused)
    {
        isPaused = _isPaused;
    }

    System.Collections.IEnumerator EnforceRateOfFire()
    {
        yield return new WaitForSeconds(1 / (maxRoundsPerMinute / 60.0f)); // 60 seconds in one minute
        dampened = false;
    }

}
