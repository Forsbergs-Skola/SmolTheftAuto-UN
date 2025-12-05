using UnityEngine;

public class LevelMusicManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       AudioManager.Instance.PlayMusic("LevelMusic");
    }
    
}
