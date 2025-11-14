using UnityEngine;

public class DialogueCanvas : MonoBehaviour, ICanvasable
{
    public EnumCanvasName CanvasName()
    {
        return EnumCanvasName.DIALOGUE;
    }
}
