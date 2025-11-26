using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using GameTools;

public class UIEventSystemController : MonoBehaviour
{
    private void Awake()
    {
        gameObject.name = "SingletonEventSystem";
        
       foreach(GameObject rootObj in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (gameObject.name == "EventSystem") { Destroy(gameObject); }
            if (gameObject.name == "Canvas") { Destroy(gameObject); }
        }


        if (GameObject.FindGameObjectsWithTag(Constants.Tags.UI_EVENT_SYSTEM).Length > 0) { Destroy(gameObject); }
        tag = Constants.Tags.UI_EVENT_SYSTEM;
    }
}
