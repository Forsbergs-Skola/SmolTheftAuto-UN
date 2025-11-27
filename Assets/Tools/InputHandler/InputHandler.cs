using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Events;
namespace GameTools
{
    public class InputHandler : MonoBehaviour
    {
        [Range(0.005f, 0.1f)] [SerializeField] private float pressedInputCooldownSeconds = 0.05f;
        [Header("Event Channels")]
        [SerializeField] private Vector2PayloadEvent moveInputEvent;
        [SerializeField] private EmptyPayloadEvent fireInputEvent;
        [SerializeField] private BoolPayloadEvent sprintInputEvent;
        [SerializeField] private EmptyPayloadEvent gamePausedPressedEvent;
        


        private Vector2 _moveInput = Vector2.zero;
        private Vector2 moveInput
        {
            get => _moveInput;
            set
            {
                if (value == _moveInput) { return; }
                _moveInput = value;
                if (moveInputEvent != null)
                {
                    moveInputEvent.TriggerEvent(_moveInput);
                }
            }
        }
        /*
        private bool _sprintIsPressed = false;
        private bool sprintIsPressed
        {
            get => _sprintIsPressed;
            set
            {
                if (value == _sprintIsPressed) { return; }
                _sprintIsPressed = value;
                if (sprintInputEvent != null)
                {
                    sprintInputEvent.TriggerEvent(_sprintIsPressed);
                }
            }
        }
        */
        private bool pressedInputDampened = false;
        private bool gamepadIsDetected = false;

        private void Awake()
        {
            if (GameObject.FindGameObjectsWithTag(Constants.Tags.INPUT_HANDLER).Length > 0) { Destroy(gameObject); }
            tag = Constants.Tags.INPUT_HANDLER;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {

            if (Gamepad.current != null)
            {
                IngestGamepadInput(Gamepad.current);
            }
            else
            {
                IngestMouseKeyboardInput(Keyboard.current);
            }
        }

        private void IngestGamepadInput(Gamepad gp)
        {
            moveInput = gp.leftStick.ReadValue().normalized;
            if (!pressedInputDampened)
            {
                if (gp.startButton.wasPressedThisFrame)
                {
                    if (gamePausedPressedEvent != null)
                    {

                        if (SceneManager.GetActiveScene().name == "Bootstrap") return;

                        gamePausedPressedEvent.TriggerEvent();
                        StartCoroutine(DampenPressedInput());
                    }
                }


            }
        }

        private void IngestMouseKeyboardInput(Keyboard kb)
        {
            float moveX = 0.0f;
            float moveY = 0.0f;
            if (kb.dKey.isPressed) { moveX += 1.0f; }
            if (kb.aKey.isPressed) { moveX -= 1.0f; }
            if (kb.wKey.isPressed) { moveY += 1.0f; }
            if (kb.sKey.isPressed) { moveY -= 1.0f; }
            moveInput = new Vector2(moveX, moveY).normalized;
            if (!pressedInputDampened)
            {
                if (kb.escapeKey.wasPressedThisFrame)
                {
                    if (gamePausedPressedEvent != null)
                    {
                        if (SceneManager.GetActiveScene().name == "Bootstrap") return;
                        gamePausedPressedEvent.TriggerEvent();
                        StartCoroutine(DampenPressedInput());
                    }
                }
            }
        }

        private System.Collections.IEnumerator DampenPressedInput()
        {
            yield return new WaitForSeconds(pressedInputCooldownSeconds);
            pressedInputDampened = false;
        }
    }
}


