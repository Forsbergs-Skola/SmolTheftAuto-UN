///////////////////////////////////
// HOW TO USE THE CANVAS MANAGER //
///////////////////////////////////
///
// Example -- to display the DialogueCanvas:
//
// Get a reference to the CanvasManager:
//      CanvasManager cm = GameObject.FindGameObjectWithTag("CANVAS_MANAGER").GetComponent<CanvasManager>();
// ...or, preferably:
//      using GameTools; <-- At the top of your script. Gives you access to Constants.Tags
//      CanvasManager cm = GameObject.FindGameObjectWithTag(Constants.Tags.CANVAS_MANAGER).GetComponent<CanvasManager>();
//
// ...the tell the CanvasManager to display the DialogueCanvas:
//      cm.DisplayCanvas(EnumCanvasName.DIALOGUE);
//
// By default, the CanvasManager will clear (un-display) all the canvases except the one you specify.
// If you want to override that behavior and display a canvas on top the already visible canvas:
//      cm.DisplayCanvas(EnumCanvasName.DIALOGUE, false);
//
// To clear all the canvases and show none of them:
//      cm.ClearCanvases();

using UnityEngine;
using UnityEngine.InputSystem;
using GameTools;
using Events;
using Tweens;
using System.Collections.Generic;

public interface ICanvasable
{
    EnumCanvasName CanvasName();
    GameObject GetCanvasObject();
    bool GetIsVisible();
    void SetIsVisible(bool val);
}
public enum EnumCanvasName
{
    PAUSE,
    MAIN,
    DIALOGUE,
    HUD,
    LOADING,
    STORE
}


public class CanvasManager : MonoBehaviour
{
    [SerializeField] private QuestPanel _questPanel;
    [SerializeField] private EmptyPayloadEvent playerUpdatedEvent;
    public QuestPanel questPanel { get => _questPanel; }
    private List<ICanvasable> canvases = new List<ICanvasable>();

    private EnumCanvasName? currentActiveCanvas = null;
    private EnumCanvasName? previousActiveCanvas = null;
    public EnumCanvasName? CurrentActiveCanvas { get => currentActiveCanvas; }
    public EnumCanvasName? PreviousActiveCanvas { get => previousActiveCanvas; }

    [SerializeField] private Canvas testButtons;
    [SerializeField] private bool testMode = false;

   

    private void Awake()
    {
        // Lazy singleton logic
        if (GameObject.FindGameObjectsWithTag(Constants.Tags.CANVAS_MANAGER).Length > 0) { Destroy(gameObject); }
        tag = Constants.Tags.CANVAS_MANAGER;
        DontDestroyOnLoad(gameObject);

        foreach (Transform xform in GetComponentInChildren<Transform>())
        {
            GameObject obj = xform.gameObject;
            if (obj.GetComponent<ICanvasable>() != null)
            {
                canvases.Add(obj.GetComponent<ICanvasable>());
            }
        }

        testButtons.enabled = testMode;
        //ClearCanvases();
    }
    private void OnEnable()
    {
        playerUpdatedEvent.OnEventTriggered += HandlePlayerDataUpdated;
    }
    private void OnDisable()
    {
        playerUpdatedEvent.OnEventTriggered -= HandlePlayerDataUpdated;
    }

    private void HandlePlayerDataUpdated()
    {
        GameManagerSingleton gm = GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>();

        //Debug.Log(gm.CurrentPlayerData.pistolInClipAmmo);
        
        ICanvasable pauseIC = GetCanvasWithName(EnumCanvasName.PAUSE);
        ICanvasable hudIC = GetCanvasWithName(EnumCanvasName.HUD);

        pauseIC.GetCanvasObject().GetComponent<PauseCanvas>().HandleInventoryUpdate(gm.CurrentPlayerData);
        hudIC.GetCanvasObject().GetComponent<HudCanvas>().HandleOnPlayerDataUpdated(gm.CurrentPlayerData);


    }

    public void DisplayCanvas(EnumCanvasName? canvasName, bool clearFirst = true)
    {
        // If the focused canvas is the HUD, hide the mouse cursor
        // otherwise, show the mouse cursor
        if (!GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>().LabMode)
        {
            if (canvasName == EnumCanvasName.HUD) { Cursor.visible = false; }
            else { Cursor.visible = true; }
        }
        


        //Debug.Log(canvasName);

        if (clearFirst)
        {
            ClearCanvases();
        }
        else
        // if not clearing first, set the sorting order of all visible canvases to 5
        {
            foreach (ICanvasable canvas in canvases)
            {
                if (canvas.GetIsVisible()) { canvas.GetCanvasObject().GetComponent<Canvas>().sortingOrder = 5; }
            }
        }
        foreach(ICanvasable canvas in canvases)
        {
            if (canvas.CanvasName() == canvasName)
            {
                if (previousActiveCanvas != currentActiveCanvas)
                {
                    previousActiveCanvas = currentActiveCanvas;
                }
                canvas.SetIsVisible(true);
                // set the sorting order to 10, so it is on top
                canvas.GetCanvasObject().GetComponent<Canvas>().sortingOrder = 10;
                currentActiveCanvas = canvas.CanvasName();
                //Canvas.ForceUpdateCanvases();
                return;
            }
        }

        Debug.LogError($"Invalid canvas name: {canvasName}");
        return;
    }

    public void GameSavedFeedback()
    {
        ICanvasable pauseIC = GetCanvasWithName(EnumCanvasName.PAUSE);
        PauseCanvas pc = pauseIC.GetCanvasObject().GetComponent<PauseCanvas>();
        pc.GameSavedFeedback();
    }

    public void StartDialogue(string convoName)
    {
        // tell the dialogue canvas to find and load the correct conversation

        ICanvasable dialogueIC = GetCanvasWithName(EnumCanvasName.DIALOGUE);
        if (dialogueIC == null) { Debug.LogError("Can't get DIALOGUE canvas"); return; }
        
        DialogueCanvas dc = dialogueIC.GetCanvasObject().GetComponent<DialogueCanvas>();
        if (dc.PrepareConvo(convoName))
        {
            DisplayCanvas(EnumCanvasName.DIALOGUE);
            dc.StartCurrentConvo();
        }
        else
        {
            Debug.LogError($"Invalid conversation name: {convoName}");
        }
    }

    public void FinishDialogue()
    {
        ICanvasable dialogueIC = GetCanvasWithName(EnumCanvasName.DIALOGUE);
        if (dialogueIC == null) { Debug.LogError("Can't get DIALOGUE canvas"); return; }
        DialogueCanvas dc = dialogueIC.GetCanvasObject().GetComponent<DialogueCanvas>();
        dc.CleanUp();

        DisplayCanvas(previousActiveCanvas);
    }

    public void StartStoreInteraction()
    {
        ICanvasable storeIC = GetCanvasWithName(EnumCanvasName.STORE);
        StoreCanvas sc = storeIC.GetCanvasObject().GetComponent<StoreCanvas>();
        DisplayCanvas(EnumCanvasName.STORE);
        sc.StartStoreInteraction();
    }
    public void FinishStoreInteraction()
    {
        ICanvasable hudIC = GetCanvasWithName(EnumCanvasName.HUD);
        HudCanvas hc = hudIC.GetCanvasObject().GetComponent<HudCanvas>();
        DisplayCanvas(EnumCanvasName.HUD);
    }

    public void ActivateMissionPassed()
    {
        ICanvasable hudIC = GetCanvasWithName(EnumCanvasName.HUD);
        if (hudIC == null) { Debug.LogError("Can't get HUD canvas"); return; }
        HudCanvas hud = hudIC.GetCanvasObject().GetComponent<HudCanvas>();
        hud.ActivateUiEffect(HudCanvas.EnumOnActivateEffectBehavior.MISSION_PASSED, 0.75f);
    }

    public void ActivatePlayerDied()
    {
        ICanvasable hudIC = GetCanvasWithName(EnumCanvasName.HUD);
        if (hudIC == null) { Debug.LogError("Can't get HUD canvas"); return; }
        HudCanvas hud = hudIC.GetCanvasObject().GetComponent<HudCanvas>();
        hud.ActivateUiEffect(HudCanvas.EnumOnActivateEffectBehavior.YOU_DIED, 0.75f);
    }

    public void ActivateEndgame()
    {
        ICanvasable hudIC = GetCanvasWithName(EnumCanvasName.HUD);
        if (hudIC == null) { Debug.LogError("Can't get HUD canvas"); return; }
        HudCanvas hud = hudIC.GetCanvasObject().GetComponent<HudCanvas>();
        hud.ActivateUiEffect(HudCanvas.EnumOnActivateEffectBehavior.YOU_WIN, 0.75f);
    }

    public void ShowHUD()
    {
        //GameManagerSingleton gm = GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>();
        //PlayerData playerData = gm.CurrentPlayerData;
        //ICanvasable hudIC = GetCanvasWithName(EnumCanvasName.HUD);
        //HudCanvas hud = hudIC.GetCanvasObject().GetComponent<HudCanvas>();
        DisplayCanvas(EnumCanvasName.HUD);
    }

    public void ShowMain()
    {
        //
        DisplayCanvas(EnumCanvasName.MAIN);
    }

    public void ShowAndFadeLoadingScreen()
    {
        StartCoroutine(WaitThenFade(0.5f));
    }
    private System.Collections.IEnumerator WaitThenFade(float wait)
    {
        ICanvasable loadingCanvas = GetCanvasWithName(EnumCanvasName.LOADING);
        
        DisplayCanvas(EnumCanvasName.LOADING,true);
        yield return new WaitForSecondsRealtime(wait);
        Tween fadeTween = TweenService.GetFloatTween(gameObject, 1.0f, 0.0f, 0.5f,EnumTweenEase.QUART,EnumTweenDirection.IN);
        fadeTween.StartTween();
        fadeTween.OnValueUpdated += (value) =>
        {
            loadingCanvas.GetCanvasObject().GetComponent<LoadcingCanvas>().SetBlackingPanelAlpha(value.x);
        };
        fadeTween.OnFinished += () =>
        {
            DisplayCanvas(EnumCanvasName.HUD);
            loadingCanvas.GetCanvasObject().GetComponent<LoadcingCanvas>().SetBlackingPanelAlpha(1.0f);
        };
    }


    private ICanvasable? GetCanvasWithName(EnumCanvasName? canvasName)
    {
        foreach (ICanvasable canv in canvases)
        {
            if (canv.CanvasName() == canvasName) { return canv; }
        }
        return null;
    }

  
    public void ClearCanvases()
    {
        foreach(ICanvasable canvas in canvases)
        {
            canvas.GetCanvasObject().GetComponent<Canvas>().sortingOrder = 0;
            canvas.SetIsVisible(false);
        }
    }
}
