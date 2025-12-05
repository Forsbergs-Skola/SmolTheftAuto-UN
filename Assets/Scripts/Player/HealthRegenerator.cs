// Put this prefab under the GameMangerSingleton object in Bootstrap if it is not already there.

using UnityEngine;
using GameTools;
using Events;

public class HealthRegenerator : MonoBehaviour
{
    [SerializeField] private IntPayloadEvent healthEvent;

    private const float UPDATE_INTERVAL = 2.0f;
    private float timeAccumulator = 0.0f;

    private GameManagerSingleton gm = null;

    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>();
    }

    private void Update()
    {
        // throttle health gain to once per UPDATE_INTERVAL seconds
        timeAccumulator += Time.deltaTime;
        if (timeAccumulator < UPDATE_INTERVAL) return;
        timeAccumulator = 0.0f;

        // Only proceed if the player is active in the hierarchy
        if (GameObject.FindGameObjectWithTag("Player") == null) return;


        // make sure you have a gm reference
        if (gm == null)
        {
            gm = GameObject.FindGameObjectWithTag(Constants.Tags.GAME_MANAGER).GetComponent<GameManagerSingleton>();
        } if (gm == null) return;

        // if the player is < their max health, increase their health by 1
        int maxHealth = gm.MaxHealth;
        int currentHealth = gm.CurrentPlayerData.health;
        if (currentHealth < maxHealth && currentHealth > 0)
        {
            //Debug.Log("Player heals by 1.");
            healthEvent.TriggerEvent(1);
        }
    }
}
