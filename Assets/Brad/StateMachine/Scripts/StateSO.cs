using UnityEngine;
//using System.Collections.Generic;

namespace StateMachine
{

    public struct StateData
    {
        private string stateName;
        private string stateID;
        private string[] transitionIDs;

        public string StateName { get => stateName; }
        public string StateID { get => stateID; }
        public string[] TransitionIDs { get => transitionIDs; }

        public StateData(string _stateName, string _stateID, string[] _transitionIDs)
        {
            stateName = _stateName;
            stateID = _stateID;
            transitionIDs = _transitionIDs;
        }
    }

    [CreateAssetMenu(fileName = "StateSO", menuName = "State Machine/State")]
    public class StateSO : ScriptableObject
    {
        public event System.Action<StateData> OnStateEntered;
        public event System.Action<StateData> OnStateExited;

        [SerializeField] private string stateName;
        [SerializeField] private StateTransitionSO[] transitions;
        [SerializeField, HideInInspector] private string stateID;

        private StateData myData;
        public StateData Data { get => myData; } // This is all you get!


        public void Initialize()
        {
            string[] transIDs = new string[transitions.Length];
            for (int i = 0; i < transitions.Length; i++)
            {
                transIDs[i] = transitions[i].GetID();
            }
            myData = new StateData(stateName, stateID, transIDs);
        }

        public string GetID() { return stateID; }

        public void Enter()
        {
            OnStateEntered?.Invoke(myData);
        }
        public void Exit()
        {
            OnStateExited?.Invoke(myData);
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(stateID))
            {
                stateID = System.Guid.NewGuid().ToString();
//#if UNITY_EDITOR
//                UnityEditor.EditorUtility.SetDirty(this);
//#endif
            }
        }
    }
}