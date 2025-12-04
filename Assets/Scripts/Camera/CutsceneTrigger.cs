using UnityEngine;
using UnityEngine.Playables;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using Events;

public class CutsceneTrigger : MonoBehaviour
{
    [SerializeField] private PlayableDirector timeline;
    [SerializeField] private GameObject player;
    [SerializeField] private CinemachineCamera cutsceneCamera;
    [SerializeField] private EmptyPayloadEvent endGameEvent;


   /* private void OnEnable()
    {
        endGameEvent.OnEventTriggered += HandleEndgame;
    }*/

   /* private void OnDisable()
    {
        endGameEvent.OnEventTriggered -= HandleEndgame;
    }*/

    //private void HandleEndgame()
    //{
   //     cutsceneCamera.Priority = 100;
   //     player.SetActive(false); 
   //     timeline.Play();
  //  }

    //private void OnTriggerEnter(Collider other)
    // {
    //   if (other.CompareTag("Player"))
    //    {
    //      cutsceneCamera.Priority = 100;
    //       player.SetActive(false);
    //       timeline.Play();
    //   }
    //}
}