using UnityEngine;
using GameTools;
using System.Collections.Generic;


public interface ICanvasable
{
    EnumCanvasName CanvasName();
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

    private List<ICanvasable> canvases = new List<ICanvasable>();

    private void Awake()
    {
        // Lazy singleton logic
        if (GameObject.FindGameObjectsWithTag(Constants.Tags.CANVAS_MANAGER).Length > 0) { Destroy(gameObject); }
        tag = Constants.Tags.CANVAS_MANAGER;
        DontDestroyOnLoad(gameObject);

        foreach(Transform xform in GetComponentInChildren<Transform>())
        {
            GameObject obj = xform.gameObject;
        }

    }
}
