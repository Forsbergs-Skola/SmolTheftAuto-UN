using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Events;
using TMPro;

public class DialogueCanvas : MonoBehaviour, ICanvasable
{
    //private const float REVEAL_INTERVAL = 0.01f;

    [Range(0.001f, 0.5f)][SerializeField] private float textRevealInterval = 0.01f;

    [SerializeField] private RawImage leftPortraitImage;
    [SerializeField] private RawImage rightPortraitImage;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueLineText;
    [SerializeField] private Button continueButton;

    [Header("Event Channels")]
    [SerializeField] private EmptyPayloadEvent advanceDialogueEvent; // maybe for a SFX
    [SerializeField] private EmptyPayloadEvent dialogueCompleteEvent;
    
    [SerializeField] private List<Conversation> conversations;

    private Conversation? currentConvo = null;
    private int currentLineIdx = -1;

    private bool _isVisible = false;
    private bool isVisible
    {
        get => _isVisible;
        set
        {
            if (value != _isVisible)
            {
                _isVisible = value;
                gameObject.SetActive(_isVisible);
            }
        }
    }
    private void Awake()
    {
        isVisible = gameObject.activeInHierarchy;
    }

    public bool PrepareConvo(string convoName)
    {
        foreach(Conversation convo in conversations)
        {
            if (convo.ConversationName == convoName) { currentConvo = convo; return true; }
        }
        return false;
    }

    public void StartCurrentConvo()
    {
        currentLineIdx = -1;
        AdvanceDialogue();
    }
    public void AdvanceDialogue()
    {
        if (currentLineIdx >= currentConvo.LineCount - 1)
        {
            dialogueCompleteEvent.TriggerEvent();
        }
        else
        {
            advanceDialogueEvent.TriggerEvent();
            currentLineIdx++;
            DialogueLine thisLine = currentConvo.GetLineAtIdx(currentLineIdx);
            speakerNameText.text = thisLine.speakerName;

            //portraitImage.texture = Resources.Load<Texture>(thisLine.portraitTexturePath);

            

            leftPortraitImage.texture = Resources.Load<Texture>(thisLine.leftTexturePath);
            rightPortraitImage.texture = Resources.Load<Texture>(thisLine.rightTexturePath);

            if (leftPortraitImage.texture == null) { leftPortraitImage.texture = Resources.Load<Texture>("DialoguePortraits/Blank"); }
            if (rightPortraitImage.texture == null) { rightPortraitImage.texture = Resources.Load<Texture>("DialoguePortraits/Blank"); }

            if (thisLine.subdueLeft) { leftPortraitImage.color = Color.grey; }
            else { leftPortraitImage.color = Color.white; }
            if (thisLine.subdueRight) { rightPortraitImage.color = Color.grey; }
            else { rightPortraitImage.color = Color.white; }



            continueButton.gameObject.SetActive(false);
            StartCoroutine(LineRevealer(thisLine.dialogueLine));
        }
    }






    //private void RevealLineText(string lineText)
    //{
    //    continueButton.gameObject.SetActive(false);
    //    StartCoroutine(LineRevealer(lineText));
    //}
    private System.Collections.IEnumerator LineRevealer(string lineText)
    {
        dialogueLineText.text = "";
        foreach(char character in lineText)
        {
            dialogueLineText.text += character;
            yield return new WaitForSeconds(textRevealInterval);
        }
        continueButton.gameObject.SetActive(true);
    }

    public void CleanUp()
    {
        //portraitImage.texture = null;
        leftPortraitImage.texture = null;
        rightPortraitImage.texture = null;
        speakerNameText.text = "";
        dialogueLineText.text = "";
        currentConvo = null;
        currentLineIdx = -1;
    }


    public EnumCanvasName CanvasName()
    {
        return EnumCanvasName.DIALOGUE;
    }
    public GameObject GetCanvasObject()
    {
        return gameObject;
    }
    public bool GetIsVisible()
    {
        return isVisible;
    }
    public void SetIsVisible(bool val)
    {
        isVisible = val;
    }
}
