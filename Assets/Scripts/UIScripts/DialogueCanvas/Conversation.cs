using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Conversation", menuName = "Dialogue/Conversation")]
public class Conversation : ScriptableObject
{
    [SerializeField] private string conversationName;
    [SerializeField] private List<DialogueLine> lines;
    private string conversationID;


    public string ConversationName { get => conversationName; }
    public int LineCount { get => lines.Count; }

    public DialogueLine? GetLineAtIdx(int idx)
    {
        if (idx < 0) { Debug.LogError("Error negative line number"); return null; }
        if (idx > LineCount - 1) { Debug.LogError($"Conversation only has {LineCount} lines"); return null; }
        return lines[idx];
    }


    private void OnValidate()
    {
        if (string.IsNullOrEmpty(conversationID))
        {
            conversationID = System.Guid.NewGuid().ToString();
        }
    }
}
