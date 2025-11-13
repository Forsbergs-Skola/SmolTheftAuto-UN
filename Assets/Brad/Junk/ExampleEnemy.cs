using UnityEngine;
using Events;
using StateMachine;
using TMPro;
using GameTools;
public class ExampleEnemy : MonoBehaviour
{
    private const string PARKED = "PARKED";
    private const string IDLE = "IDLE";
    private const string REACT = "REACT";
    private const string TO_PARKED = "TO_PARKED";
    private const string TO_IDLE = "TO_IDLE";
    private const string TO_REACT = "TO_REACT";
    private const float REACT_DURATION = 0.2f;

    [SerializeField] private SimpleStateMachineSO stateMachine;
    [SerializeField] private TMP_Text animText;
    [SerializeField] private BoolPayloadEvent gamePausedEvent;
    [SerializeField] private FloatPayloadEvent hitEvent;


    private void Start()
    {
        stateMachine.Initialize();
        stateMachine.TriggerTransition(TO_IDLE);
    }

    private void OnEnable()
    {
        foreach (StateSO state in stateMachine.GetStates())
        {
            state.OnStateEntered += HandleOnEnterState;
        }
        gamePausedEvent.OnEventTriggered += IngestGamePausedEvent;
    }
    private void OnDisable()
    {
        foreach (StateSO state in stateMachine.GetStates())
        {
            state.OnStateEntered -= HandleOnEnterState;
        }
        gamePausedEvent.OnEventTriggered -= IngestGamePausedEvent;
    }

   

    private void HandleOnEnterState(StateData stateData)
    {
        switch (stateData.StateName)
        {
            case PARKED:
                StopAllCoroutines();
                animText.text = "PARKED\n(no animation)";
                return;
            case IDLE:
                animText.text = "IDLE\nANIMATION";
                return;
            case REACT:
                animText.text = "REACT\nANIMATION";
                return;
        }
    }

    

    private void IngestGamePausedEvent(bool _isPaused)
    {
        if (_isPaused)
        {
            stateMachine.TriggerTransition(TO_PARKED);
            return;
        }
        else
        {
            stateMachine.TriggerTransition(TO_IDLE);
            return;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (stateMachine.CurrentStateData.StateName != IDLE) { return; }

        stateMachine.TriggerTransition(TO_REACT);
        StartCoroutine(Reacting());

        if (!other.gameObject.CompareTag(Constants.Tags.BULLET)) { return; }
        ExampleBullet bullet = other.gameObject.GetComponent<ExampleBullet>();
        float damageTaken = bullet.damage;
        bullet.DieEarly();
        hitEvent.TriggerEvent(damageTaken);
    }

    private System.Collections.IEnumerator Reacting()
    {
        yield return new WaitForSeconds(REACT_DURATION);
        stateMachine.TriggerTransition(TO_IDLE);
    }
}
