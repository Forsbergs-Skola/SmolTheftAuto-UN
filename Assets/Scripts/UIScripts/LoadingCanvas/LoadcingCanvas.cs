using UnityEngine;

public class LoadcingCanvas : MonoBehaviour, ICanvasable
{
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

    public EnumCanvasName CanvasName()
    {
        return EnumCanvasName.LOADING;
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
