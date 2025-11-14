using UnityEngine;

public class HudCanvas : MonoBehaviour, ICanvasable
{
    public EnumCanvasName CanvasName()
    {
        return EnumCanvasName.HUD;
    }
}
