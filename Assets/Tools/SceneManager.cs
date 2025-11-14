using UnityEngine;
using Events;
using GameTools;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform playerStart;
    [SerializeField] private Transform enemyStart;


    [SerializeField] private BoolPayloadEvent gameIsPausedEvent;

    private bool _gameIsPaused = false;
    private bool gameIsPaused
    {
        get => _gameIsPaused;
        set
        {
            if (value == _gameIsPaused) { return; }
            _gameIsPaused = value;
            gameIsPausedEvent.TriggerEvent(_gameIsPaused);
        }
    }


    private void Awake()
    {
        if (GameObject.FindGameObjectsWithTag(Constants.Tags.SCENE_MANAGER).Length > 0) { Destroy(gameObject); }
        tag = Constants.Tags.SCENE_MANAGER;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        GameObject playerObj = Instantiate(playerPrefab);
        GameObject enemyObj = Instantiate(enemyPrefab);
        playerObj.transform.position = playerStart.position;
        enemyObj.transform.position = enemyStart.position;
    }


    public void TogglePauseGame()
    {
        gameIsPaused = !gameIsPaused;
    }

}
