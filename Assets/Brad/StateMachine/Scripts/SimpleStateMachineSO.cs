using UnityEngine;
using System.Collections.Generic;

namespace StateMachine
{
    [CreateAssetMenu(fileName = "SimpleStateMachineSO", menuName = "State Machine/Simple State Machine")]
    public class SimpleStateMachineSO : ScriptableObject
    {

        public event System.Action StateMachineReady;

        private const int HISTORY_SIZE = 10;

        [SerializeField] private StateSO initialState;
        [SerializeField] private List<StateSO> states;
        [SerializeField] private List<StateTransitionSO> transitions;

        private bool isReady = false;
        private StateData currentStateData;
        private List<StateData> stateHistory = new List<StateData>();

        public bool IsReady { get => isReady; }
        public StateData CurrentStateData { get => currentStateData; }
        public List<StateData> StateHistory { get => stateHistory; }


        public void Initialize()
        {
            foreach (StateSO state in states)
            {
                state.Initialize();
            }
            foreach (StateTransitionSO transition in transitions)
            {
                transition.Initialize();
            }
            currentStateData = initialState.Data;
            StateMachineReady?.Invoke();
            isReady = true;
        }

        private void UpdateStateHistory(StateData previousStateData)
        {
            stateHistory.Add(previousStateData);
            if (stateHistory.Count > HISTORY_SIZE) { stateHistory.RemoveAt(0); }
            //Debug.Log($"Adding {previousStateData.StateName} to state history. History count: {stateHistory.Count}");
        }

        public void TriggerTransition(string _eventString)
        {
            StateSO currentState = GetStateByID(currentStateData.StateID);
            StateSO nextState = null;

            if (GetStateByID(currentStateData.StateID) == null)
            {
                Debug.LogError("Something weird happened");
                return;
            }
            foreach (string transID in currentStateData.TransitionIDs)
            {
                
                StateTransitionSO transition = GetTransitionByID(transID);
                if (transition.Data.EventString == _eventString)
                {
                    string nextStateID = transition.Data.ToStateID;
                    nextState = GetStateByID(nextStateID);
                    break;
                }
            }
            if (nextState == null)
            {
                Debug.LogError($"{currentStateData.StateName} has no transition with string {_eventString}");
                return;
            }

            currentState.Exit();
            UpdateStateHistory(currentStateData);
            currentStateData = nextState.Data;
            nextState.Enter();

        }

        private StateTransitionSO? GetTransitionByID(string id)
        {
            foreach (StateTransitionSO trans in transitions)
            {
                if (trans.GetID() == id) { return trans; }
            }
            return null;
        }
        private StateSO? GetStateByID(string id)
        {
            foreach (StateSO state in states)
            {
                if (state.Data.StateID == id) { return state; }
            }
            return null;
        }

        public List<StateSO> GetStates()
        {
            return states;
        }
    }
}