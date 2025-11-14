using UnityEngine;
using TMPro;
using GameTools;
using Events;

public class ExampleUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject pausePanel;
    [Header("Event Channels")]
    [SerializeField] private FloatPayloadEvent enemyHitEvent;
    [SerializeField] private BoolPayloadEvent gamePausedEvent;

    [SerializeField] private EmptyPayloadEvent somethingHappenedEvent;
    
    private int score = 0;

    private bool _isPaused = false;
    private bool isPaused
    {
        get => _isPaused;
        set
        {
            if (value == _isPaused) { return; }
            _isPaused = value;
            GameObject smObj = GameObject.FindGameObjectWithTag(Constants.Tags.SCENE_MANAGER);
            if (smObj != null)
            {
                smObj.GetComponent<SceneManager>().TogglePauseGame();
            }
        }
    }


    private void Awake()
    {
        gamePausedEvent.OnEventTriggered += HandlePauseUI;
    }


    public void HandlePauseUI(bool pausedValue)
    {
        pausePanel.SetActive(pausedValue);
    }

    public void HandleDemoButtonPressed()
    {
        somethingHappenedEvent.TriggerEvent();
    }
    
    

    public void HandleOnPauseToggled()
    {
        isPaused = !isPaused;
    }


    private void Start()
    {
        pausePanel.SetActive(isPaused);
        scoreText.text = $"SCORE: {score}";
    }
    private void OnEnable()
    {
        enemyHitEvent.OnEventTriggered += IngestHitEvent;
        gamePausedEvent.OnEventTriggered += HandlePauseUI;
    }
    private void OnDisable()
    {
        enemyHitEvent.OnEventTriggered -= IngestHitEvent;
        gamePausedEvent.OnEventTriggered -= HandlePauseUI;
    }
    private void IngestHitEvent(float hitDamage)
    {
        score += (int)hitDamage;
        scoreText.text = $"SCORE: {score}";
    }
}
