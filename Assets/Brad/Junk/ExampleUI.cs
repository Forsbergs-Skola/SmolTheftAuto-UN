using UnityEngine;
using TMPro;
using Events;

public class ExampleUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject pausePanel;
    [Header("Event Channels")]
    [SerializeField] private FloatPayloadEvent enemyHitEvent;
    [SerializeField] private BoolPayloadEvent gamePausedEvent;
    
    private int score = 0;

    private bool _isPaused = false;
    private bool isPaused
    {
        get => _isPaused;
        set
        {
            if (value == _isPaused) { return; }
            _isPaused = value;
            pausePanel.SetActive(_isPaused);
            gamePausedEvent.TriggerEvent(_isPaused);
        }
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
    }
    private void OnDisable()
    {
        enemyHitEvent.OnEventTriggered -= IngestHitEvent;
    }
    private void IngestHitEvent(float hitDamage)
    {
        score += (int)hitDamage;
        scoreText.text = $"SCORE: {score}";
    }
}
