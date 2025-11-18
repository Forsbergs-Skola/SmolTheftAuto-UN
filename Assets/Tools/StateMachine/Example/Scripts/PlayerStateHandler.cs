using UnityEngine;
using StateMachine;

public class PlayerStateHandler : MonoBehaviour
{
    [SerializeField] private SimpleStateMachineSO stateMachine;

    private void Awake()
    {
        stateMachine.Initialize();
    }

    private void OnEnable()
    {
        foreach(StateSO state in stateMachine.GetStates())
        {
            state.OnStateEntered += IngestOnStateEntered;
        } 
    }

    
    private void OnDisable()
    {
        foreach (StateSO state in stateMachine.GetStates())
        {
            state.OnStateEntered -= IngestOnStateEntered;
        }
    }

    private void IngestOnStateEntered(StateData stateData)
    {
        ExamplePlayerMovement playerMovement = GetComponent<ExamplePlayerMovement>();
        playerMovement.HandleOnStateEntered(stateData.StateName);
    }



}
