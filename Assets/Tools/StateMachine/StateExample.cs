using UnityEngine;
using StateMachine;

public class StateExample : MonoBehaviour
{

    private const string ON = "ON";
    private const string OFF = "OFF";
    private const string TO_ON = "TO_ON";
    private const string TO_OFF = "TO_OFF";


    [SerializeField] private SimpleStateMachineSO stateMachine;

    private void Awake()
    {
        stateMachine.Initialize();
    }

    private void OnEnable()
    {
        foreach(StateSO state in stateMachine.GetStates())
        {
            state.OnStateEntered += HandleOnStateEntered;
        }
    }
    private void OnDisable()
    {
        foreach (StateSO state in stateMachine.GetStates())
        {
            state.OnStateEntered -= HandleOnStateEntered;
        }
    }


    private void Update()
    {
        switch (stateMachine.CurrentStateData.StateName)
        {
            case ON:
                // do whatever you do when you're ON
                return;
            case OFF:
                // do OFF stuff
                return;
            default:
                return;
        }
    }

    public void HandleChangeStateButtonPressed()
    {
        switch (stateMachine.CurrentStateData.StateName)
        {
            case ON:
                stateMachine.TriggerTransition(TO_OFF);
                return;
            case OFF:
                stateMachine.TriggerTransition(TO_ON);
                return;
            default:
                return;
        }
    }

    private void HandleOnStateEntered(StateData _stateData)
    {
        switch (_stateData.StateName)
        {
            case ON:
                Debug.Log("I am the ON state");
                return;
            case OFF:
                Debug.Log("I am the OFF state;");
                return;
            default:
                return;
        }
    }

}
