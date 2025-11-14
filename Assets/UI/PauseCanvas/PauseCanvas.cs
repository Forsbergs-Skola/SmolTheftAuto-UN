using UnityEngine;

public class PauseCanvas : MonoBehaviour, ICanvasable
{
    public EnumCanvasName CanvasName()
    {
        return EnumCanvasName.PAUSE;
    }
}
