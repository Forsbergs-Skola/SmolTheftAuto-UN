using UnityEngine;
using TMPro;

public class QuestItem : MonoBehaviour
{
    [SerializeField] private EnumQuest questName;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    public EnumQuest QuestName { get => questName; }

    public void StrikethoughText()
    {
        titleText.fontStyle = FontStyles.Strikethrough;
        descriptionText.fontStyle = FontStyles.Strikethrough;
    }

}
