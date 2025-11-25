using UnityEngine;
using UnityEngine.UI;

public class LoadcingCanvas : MonoBehaviour, ICanvasable
{
    [SerializeField] private Image blackingPanel;

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


    public void SetBlackingPanelAlpha(float _alpha)
    {
        blackingPanel.color = new Color(0, 0, 0, _alpha);
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
