using UnityEngine;
namespace StateMachine
{

    public struct StateTransitionData
    {
        private string transitionName;
        private string transitionID;
        private string toStateID;
        private string eventString;

        public string TransitionName { get => transitionName; }
        public string TransitionID { get => transitionID; }
        public string ToStateID { get => toStateID; }
        public string EventString { get => eventString; }


        public StateTransitionData(string _transitionName, string _transID, string _toStateID, string _eventString)
        {
            transitionName = _transitionName;
            transitionID = _transID;
            toStateID = _toStateID;
            eventString = _eventString;
        }

    }


    [CreateAssetMenu(fileName = "StateTransitionSO", menuName = "State Machine/State Transition")]
    public class StateTransitionSO : ScriptableObject
    {

        public event System.Action<StateTransitionData> OnTransitionTaken;

        [SerializeField] private StateSO toState;
        [SerializeField] private string transitionName;
        [SerializeField] private string eventString;
        [SerializeField, HideInInspector] private string transitionID;


        private StateTransitionData myData;
        public StateTransitionData Data { get => myData; } // This is all you get!

        public void Initialize()
        {
            myData = new StateTransitionData(transitionName, transitionID, toState.GetID(), eventString);
        }

        public string GetID() { return transitionID; }

        public void Take()
        {
            OnTransitionTaken?.Invoke(myData);
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(transitionID))
            {
                transitionID = System.Guid.NewGuid().ToString();
//#if UNITY_EDITOR
//                UnityEditor.EditorUtility.SetDirty(this);
//#endif
            }
        }

    }
}