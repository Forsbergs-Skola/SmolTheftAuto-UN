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
using GameTools;
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
    LOADING
}


public class CanvasManager : MonoBehaviour
{
    [SerializeField] private QuestPanel _questPanel;
    public QuestPanel questPanel { get => _questPanel; }


    private List<ICanvasable> canvases = new List<ICanvasable>();

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
        ClearCanvases();
    }

    public void DisplayCanvas(EnumCanvasName canvasName, bool clearFirst = true)
    {
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
                canvas.SetIsVisible(true);
                // set the sorting order to 10, so it is on top
                canvas.GetCanvasObject().GetComponent<Canvas>().sortingOrder = 10;
                return;
            }
        }
        Debug.LogError($"Invalid canvas name: {canvasName}");
        return;
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
