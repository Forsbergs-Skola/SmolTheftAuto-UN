using UnityEngine;
using UnityEngine.UI;
using GameTools;
using Events;
using StateMachine;

public class TestScene : MonoBehaviour
{

    [SerializeField] private EnumQuestPayloadEvent questStartedEvent;
    [SerializeField] private EnumQuestPayloadEvent questEndedEvent;

    [SerializeField] private Button startSunglassesButton;
    [SerializeField] private Button startMatchesButton;
    [SerializeField] private Button startGasCanButton;
    [SerializeField] private Button finishSunglassesButton;
    [SerializeField] private Button finishMatchesButton;
    [SerializeField] private Button finishGasCanButton;


    private CanvasManager cm;

    private void Start()
    {
        cm = GameObject.FindGameObjectWithTag(Constants.Tags.CANVAS_MANAGER).GetComponent<CanvasManager>();
        cm.ClearCanvases();

        finishSunglassesButton.gameObject.SetActive(false);
        finishMatchesButton.gameObject.SetActive(false);
        finishGasCanButton.gameObject.SetActive(false);

    }


    public void TestStartQuest(string questString)
    {
        switch (questString)
        {
            case "SUNGLASSES":
                questStartedEvent.TriggerEvent(EnumQuest.SUNGLASSES);
                startSunglassesButton.gameObject.SetActive(false);
                finishSunglassesButton.gameObject.SetActive(true);
                break;
            case "MATCHES":
                questStartedEvent.TriggerEvent(EnumQuest.MATCHES);
                startMatchesButton.gameObject.SetActive(false);
                finishMatchesButton.gameObject.SetActive(true);
                break;
            case "GAS_CAN":
                questStartedEvent.TriggerEvent(EnumQuest.GAS_CAN);
                startGasCanButton.gameObject.SetActive(false);
                finishGasCanButton.gameObject.SetActive(true);
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
        bool finalReady = (gm.currentPlayerData.hasGasCan && gm.currentPlayerData.hasSunglasses && gm.currentPlayerData.hasMatches);
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


    public void ClearAllCanvases()
    {
        cm.ClearCanvases();
    }
}
