using UnityEngine;
using UnityEngine.UI;
using GameTools;
using Events;
using StateMachine;

public class TestScene : MonoBehaviour
{

    [SerializeField] private EnumQuestPayloadEvent questStartedEvent;
    [SerializeField] private EnumQuestPayloadEvent questEndedEvent;
    [SerializeField] private EmptyPayloadEvent saveGameEvent;
    [SerializeField] private EmptyPayloadEvent clearSaveEvent;
    [SerializeField] private BoolPayloadEvent saveExistsChangedEvent;

    [SerializeField] private Button startSunglassesButton;
    [SerializeField] private Button startMatchesButton;
    [SerializeField] private Button startGasCanButton;
    [SerializeField] private Button finishSunglassesButton;
    [SerializeField] private Button finishMatchesButton;
    [SerializeField] private Button finishGasCanButton;


    [SerializeField] private EmptyPayloadEvent checkpointClearedEvent;
    [SerializeField] private EmptyPayloadEvent killNpcEvent;

    [SerializeField] private Button clearSaveButton;
    


    private CanvasManager cm;

    private void Start()
    {
        cm = GameObject.FindGameObjectWithTag(Constants.Tags.CANVAS_MANAGER).GetComponent<CanvasManager>();
        //cm.ClearCanvases();
    }

    public void FixButtons(PlayerData p, QuestStartedData q)
    {                             
        startSunglassesButton.gameObject.SetActive(!q.sunglasses);
        startMatchesButton.gameObject.SetActive(!q.matches);
        startGasCanButton.gameObject.SetActive(!q.gasCan);
    }


    private void OnEnable()
    {
        saveExistsChangedEvent.OnEventTriggered += HandleSaveExistsChanged;
    }
    private void OnDisable()
    {
        saveExistsChangedEvent.OnEventTriggered -= HandleSaveExistsChanged;
    }


    public void TestStartQuest(string questString)
    {
        switch (questString)
        {
            case "SUNGLASSES":
                questStartedEvent.TriggerEvent(EnumQuest.SUNGLASSES);
                startSunglassesButton.gameObject.SetActive(false);
                //finishSunglassesButton.gameObject.SetActive(true);
                break;
            case "MATCHES":
                questStartedEvent.TriggerEvent(EnumQuest.MATCHES);
                startMatchesButton.gameObject.SetActive(false);
                //finishMatchesButton.gameObject.SetActive(true);
                break;
            case "GAS_CAN":
                questStartedEvent.TriggerEvent(EnumQuest.GAS_CAN);
                startGasCanButton.gameObject.SetActive(false);
                //finishGasCanButton.gameObject.SetActive(true);
                break;
            default: return;
        }
    }
    public void TestFinishQuest(string questString)
    {

        

        switch (questString)
        {
            case "SUNGLASSES":
                finishSunglassesButton.gameObject.SetActive(false);
                questEndedEvent.TriggerEvent(EnumQuest.SUNGLASSES);
                break;
            case "MATCHES":
                finishMatchesButton.gameObject.SetActive(false);
                questEndedEvent.TriggerEvent(EnumQuest.MATCHES);
                break;
            case "GAS_CAN":
                finishGasCanButton.gameObject.SetActive(false);
                questEndedEvent.TriggerEvent(EnumQuest.GAS_CAN);
                break;
            default: return;
        }

        GameManagerSingleton gm = GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>();
        bool finalReady = (gm.CurrentPlayerData.hasGasCan && gm.CurrentPlayerData.hasSunglasses && gm.CurrentPlayerData.hasMatches);
        //if (finalReady) { Debug.Log("FINAL READY"); }
        if (finalReady)
        {
            questStartedEvent.TriggerEvent(EnumQuest.FINAL);
        }

    }





    public void TestDisplayHUD()
    {
        cm.DisplayCanvas(EnumCanvasName.HUD);
    }
    public void TestDisplayPause()
    {
        cm.DisplayCanvas(EnumCanvasName.PAUSE);
    }
    public void TestDisplayDialogue()
    {
        cm.DisplayCanvas(EnumCanvasName.DIALOGUE);
    }
    public void TestDisplayMain()
    {
        cm.DisplayCanvas(EnumCanvasName.MAIN);
    }
    public void TestDisplayLoading()
    {
        cm.DisplayCanvas(EnumCanvasName.LOADING);
    }

    public void TestHUDOnTop()
    {
        cm.DisplayCanvas(EnumCanvasName.HUD, false);
    }
    public void TestPauseOnTop()
    {
        cm.DisplayCanvas(EnumCanvasName.PAUSE, false);
    }
    public void TestDialogueOnTop()
    {
        cm.DisplayCanvas(EnumCanvasName.DIALOGUE, false);
    }
    public void TestMainOnTop()
    {
        cm.DisplayCanvas(EnumCanvasName.MAIN, false);
    }
    public void TestLoadingOnTop()
    {
        cm.DisplayCanvas(EnumCanvasName.LOADING, false);
    }

    public void ClearCheckpoint()
    {
        checkpointClearedEvent.TriggerEvent();
    }

    public void KillNpc()
    {
        killNpcEvent.TriggerEvent();
    }

    public void SaveGame()
    {
        saveGameEvent.TriggerEvent();
    }
    public void ClearSave()
    {
        clearSaveEvent.TriggerEvent();
    }


    public void ResetEverything()
    {
        RectTransform[] xForms = GameObject.FindObjectsOfType<RectTransform>();
        foreach(RectTransform xForm in xForms)
        {
            Debug.Log(xForm.gameObject.name);
            LayoutRebuilder.ForceRebuildLayoutImmediate(xForm);
        }
        Canvas.ForceUpdateCanvases();
    }

    private void HandleSaveExistsChanged(bool exists)
    {
        clearSaveButton.gameObject.SetActive(exists);
    }

    public void ClearAllCanvases()
    {
        cm.ClearCanvases();
        cm.DisplayCanvas(cm.PreviousActiveCanvas);
    }
}
