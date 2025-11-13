using UnityEngine;
using GameTools;
namespace Quests
{
    public struct QuestData
    {
        private string questName;
        private string questDescription;
        private string questID;
        private bool isComplete;
        public string QuestName { get => questName; }
        public string QuestDescription { get => questDescription; }
        public string QuestID { get => questID; }
        public bool IsComplete { get => isComplete; }
        public QuestData
            (
                string _questName,
                string _questDescription,
                string _questID,
                bool _isComplete

            )
        {
            questName = _questName;
            questDescription = _questDescription;
            questID = _questID;
            isComplete = _isComplete;
        }
    }


    [CreateAssetMenu(fileName = "QuestSO", menuName = "Quests/Quest")]
    public class QuestSO : ScriptableObject
    {
        [SerializeField] private string questName = "";
        [SerializeField] private string questDescription = "(Quest description)";

        private string questID = "";
        private bool isComplete = false;
        private QuestData myData;


        public QuestData Data { get => myData; }



        public void InitializeNew()
        {
            myData = new QuestData
                (
                    questName,
                    questDescription,
                    questID,
                    isComplete
                );
        }
        public void InitializeSaved(bool _isComplete)
        {
            myData = new QuestData
                (
                    questName,
                    questDescription,
                    questID,
                    _isComplete
                );
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(questID))
            {
                questID = System.Guid.NewGuid().ToString();
            }
        }

    }
}


