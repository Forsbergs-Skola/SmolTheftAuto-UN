using UnityEngine;
using System.Collections.Generic;
namespace Quests
{
    [CreateAssetMenu(fileName = "QuestTracker", menuName = "Quests/QuestTracker")]



    public class QuestTracker : ScriptableObject
    {
        [SerializeField] private List<QuestSO> quests;
        private bool savedQuestDataFound = false;

        public void Initialize()
        {
            // if saved data found, then flip savedQuestDataFound
            foreach (QuestSO quest in quests)
            {
                if (savedQuestDataFound)
                {
                    string thisQuestID = quest.Data.QuestID;
                    quest.InitializeSaved(GetSavedIsComplete(thisQuestID));
                }
                else
                {
                    quest.InitializeNew();
                }
            }
        }



        private bool GetSavedIsComplete(string _questID)
        {
            // find the isComplete value of the quest with ID _questID and return it
            return false; // placeholder logic
        }


    }



}


