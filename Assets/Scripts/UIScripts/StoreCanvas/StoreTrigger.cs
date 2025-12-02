using UnityEngine;
using GameTools;
using Events;

public class StoreTrigger : MonoBehaviour
{
    [SerializeField] private EmptyPayloadEvent storeInteractionStartedEvent;
    [SerializeField] private EmptyPayloadEvent storeInteractionFinishedEvent;

   // private GameManagerSingleton gm;
    private Collider myCollider;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
    }


    private void Start()
    {
        //gm = GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>();
    }

    private void OnEnable()
    {
        storeInteractionFinishedEvent.OnEventTriggered += HandleOnInteractionFinished;
    }
    private void OnDisable()
    {
        storeInteractionFinishedEvent.OnEventTriggered -= HandleOnInteractionFinished;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!GetIsPlayer(other)) return;
        myCollider.enabled = false;
        storeInteractionStartedEvent.TriggerEvent();
    }



    private void HandleOnInteractionFinished()
    {
        StartCoroutine(WaitThenReenable());
    }

    System.Collections.IEnumerator WaitThenReenable()
    {
        yield return new WaitForSecondsRealtime(8.0f);
        myCollider.enabled = true;
    }


    private bool GetIsPlayer(Collider _coll)
    {
        if (_coll.gameObject.CompareTag("Player")) return true;
        return false;
    }

}
