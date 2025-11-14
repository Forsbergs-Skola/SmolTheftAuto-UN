using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
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
        [SerializeField] private BoolPayloadEvent gamePausedEvent;

        private Vector2 _moveInput = Vector2.zero;
        private Vector2 moveInput
        {
            get => _moveInput;
            set
            {
                if (value == _moveInput) { return; }
                _moveInput = value;
                moveInputEvent.TriggerEvent(_moveInput);
            }
        }
        private bool _sprintIsPressed = false;
        private bool sprintIsPressed
        {
            get => _sprintIsPressed;
            set
            {
                if (value == _sprintIsPressed) { return; }
                _sprintIsPressed = value;
                sprintInputEvent.TriggerEvent(_sprintIsPressed);
            }
        }
        private bool pressedInputDampened = false;
        private bool gamepadIsDetected = false;

        private void Awake()
        {
            if (GameObject.FindGameObjectsWithTag(Constants.Tags.INPUT_HANDLER).Length > 1) { Destroy(gameObject); }
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
            // GP move input
            moveInput = gp.leftStick.ReadValue().normalized;

            // GP sprint input
            sprintIsPressed = gp.leftShoulder.isPressed;

            // GP fire input
            if (!pressedInputDampened)
            {
                if (gp.rightTrigger.wasPressedThisFrame)
                {
                    fireInputEvent.TriggerEvent();
                    StartCoroutine(DampenPressedInput());
                }
                if (gp.startButton.wasPressedThisFrame)
                {
                    TogglePause();
                }
            }
        }

        private void IngestMouseKeyboardInput(Keyboard kb)
        {
            // KB move input
            float moveX = 0.0f;
            float moveY = 0.0f;
            if (kb.dKey.isPressed) { moveX += 1.0f; }
            if (kb.aKey.isPressed) { moveX -= 1.0f; }
            if (kb.wKey.isPressed) { moveY += 1.0f; }
            if (kb.sKey.isPressed) { moveY -= 1.0f; }
            moveInput = new Vector2(moveX, moveY).normalized;

            // KB sprint input
            sprintIsPressed = kb.leftShiftKey.isPressed;

            // KB fire input
            if (!pressedInputDampened)
            {
                if (kb.spaceKey.wasPressedThisFrame)
                {
                    fireInputEvent.TriggerEvent();
                    StartCoroutine(DampenPressedInput());
                }

                if (kb.escapeKey.wasPressedThisFrame)
                {
                    TogglePause();
                }

            }
        }

        private void TogglePause()
        {
            GameObject smObj = GameObject.FindGameObjectWithTag(Constants.Tags.SCENE_MANAGER);
            if (smObj != null)
            {
                smObj.GetComponent<SceneManager>().TogglePauseGame();
            }
        }

        private System.Collections.IEnumerator DampenPressedInput()
        {
            yield return new WaitForSeconds(pressedInputCooldownSeconds);
            pressedInputDampened = false;
        }
    }
}


