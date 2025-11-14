using UnityEngine;
using GameTools;

public class UIEventSystemController : MonoBehaviour
{
    private void Awake()
    {
        if (GameObject.FindGameObjectsWithTag(Constants.Tags.UI_EVENT_SYSTEM).Length > 0) { Destroy(gameObject); }
        tag = Constants.Tags.UI_EVENT_SYSTEM;
        DontDestroyOnLoad(gameObject);
    }
}
