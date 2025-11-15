using UnityEngine;
using GameTools;
using Events;
using StateMachine;

public class TestScene : MonoBehaviour
{
    [SerializeField] private CanvasManager cm;

    private void Start()
    {
        cm = GameObject.FindGameObjectWithTag(Constants.Tags.CANVAS_MANAGER).GetComponent<CanvasManager>();
        cm.ClearCanvases();
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
