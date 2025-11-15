using UnityEngine;
using UnityEngine.UI;

public class QuestPanel : MonoBehaviour
{
    private const float RECT_HEIGHT = 160f;
    private const float TOP_SLOT_TOP = 82f;
    private const float BOTTOM_SLOT_BOTTOM = 45f;

    [SerializeField] private GameObject sunglassesPrefab;
    [SerializeField] private GameObject gasCanPrefab;
    [SerializeField] private GameObject matchesPrefab;
    [SerializeField] private GameObject finalPrefab;

    private void Start()
    {
        GameObject testObj = Instantiate(gasCanPrefab);
        testObj.transform.parent = transform;
        RectTransform testXform = testObj.GetComponent<RectTransform>();

        PositionRectTransform(testXform, 0);

    }


    private void PositionRectTransform(RectTransform rectXForm, int order)
    {
        if (order < 0) { order = 0; }
        if (order > 4) { order = 4; }

        float rectTop = TOP_SLOT_TOP + ((float)order * RECT_HEIGHT);
        float rectBottom = BOTTOM_SLOT_BOTTOM + ((float)order * RECT_HEIGHT);

        rectXForm.anchorMin = new Vector2(0f, 1f);
        rectXForm.anchorMax = new Vector2(1f, 1f);
        rectXForm.offsetMax = new Vector2(0f, rectTop);
        rectXForm.offsetMin = new Vector2(0f, rectBottom);

        // I want to manipulate rectXForm as follows...
        //rectXForm.rect.left = 0f;
        //rectXForm.rect.right = 0f;
        //rectXForm.rect.top = rectTop;
        //rectXForm.rect.bottom = rectBottom;

        // ..but Unity bitches about everything I try
    }
}
