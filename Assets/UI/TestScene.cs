using UnityEngine;
using Events;
using StateMachine;

public class TestScene : MonoBehaviour
{
    [SerializeField] private EmptyPayloadEvent pausePressedEvent;
    [SerializeField] private BoolPayloadEvent gamePausedChangedEvent;

    private bool gameIsPaused = false;


    private void Start()
    {
        
    }

    private void OnEnable()
    {
        pausePressedEvent.OnEventTriggered += HandlePausePressed;
    }
    private void OnDisable()
    {
        pausePressedEvent.OnEventTriggered -= HandlePausePressed;
    }

    private void HandlePausePressed()
    {
        

    }
}
