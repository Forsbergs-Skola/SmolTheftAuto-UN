using UnityEngine;
using StateMachine;

public class PlayerStateMachineInitializer : MonoBehaviour
{
    [SerializeField] private SimpleStateMachineSO playerStateMachine;

    private void Awake()
    {
        if (playerStateMachine != null)
        {
            playerStateMachine.Initialize();
        }
        else
        {
            Debug.LogError("PlayerStateMachineInitializer: playerStateMachine is not assigned.");
        }
    }
}
