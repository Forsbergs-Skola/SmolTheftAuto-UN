using UnityEngine;
using Events;
using UnityEngine.Playables;
using Unity.Cinemachine;
public class EndGameTrigger : MonoBehaviour
{
    [SerializeField] private EnumQuestPayloadEvent questStartedEvent;
    [SerializeField] private EmptyPayloadEvent endgameEvent;
    [SerializeField] private PlayableDirector timeline;
    [SerializeField] private GameObject player;
    [SerializeField] private CinemachineCamera cutsceneCamera;

    private Collider myCollider;
    private bool triggerable = false; // <--Prevents redundant triggering

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
        myCollider.enabled = false;
    }

    private void Start()
    {
        timeline.stopped += OnTimelineFinished;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!triggerable) return;
        if (!other.gameObject.CompareTag("Player")) return;

        // Disable the collider and kick off the end game sequence
        triggerable = false; 
        myCollider.enabled = false;
        
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlayMusic("EndingMusic");
        
        cutsceneCamera.Priority = 100;
        player.SetActive(false); 
        timeline.Play();
        
        
    }

    private void OnEnable()
    {
        questStartedEvent.OnEventTriggered += HandleOnQuestStarted;
    }
    private void OnDisable()
    {
        questStartedEvent.OnEventTriggered -= HandleOnQuestStarted;
    }

    private void HandleOnQuestStarted(EnumQuest quest)
    {
        // enable player detection when the final quest begins
        if (quest != EnumQuest.FINAL) return;
        myCollider.enabled = true;
        triggerable = true;
    }

    private void OnTimelineFinished(PlayableDirector director)
    {
        endgameEvent.TriggerEvent();
    }
}
