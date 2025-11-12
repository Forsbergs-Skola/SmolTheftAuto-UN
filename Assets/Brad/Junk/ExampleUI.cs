using UnityEngine;
using TMPro;
using Events;

public class ExampleUI : MonoBehaviour
{
    [SerializeField] FloatPayloadEvent enemyHitEvent;
    [SerializeField] TMP_Text scoreText;

    private int score = 0;
    

    private void Start()
    {
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
