using UnityEngine;
using UnityEngine.Playables;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class CutsceneTrigger : MonoBehaviour
{
    [SerializeField] private PlayableDirector timeline;
    [SerializeField] private PlayerController player;
    [SerializeField] private CinemachineCamera cutsceneCamera;
    [SerializeField] private Animator playerAnimator;     

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cutsceneCamera.Priority = 100;
            player.enabled = false;
            playerAnimator.Play("Idle");
            timeline.Play();
        }
    }
}