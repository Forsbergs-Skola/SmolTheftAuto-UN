using UnityEngine;
using Events;

public class ExamplePlayerMovement : MonoBehaviour
{

    
    
    [SerializeField] private Vector2PayloadEvent moveInputUpdate;
    [SerializeField] private BoolPayloadEvent sprintInputUpdate;

    [SerializeField] float moveSpeed = 4.0f;
    [Range(1.0f, 5.0f)] [SerializeField] float sprintMultiplier;

    private Vector2 currentMoveInput = Vector2.zero;
    private bool isSprinting;


    private void Update()
    {
        float sprint = 1.0f;
        if (isSprinting) { sprint *= sprintMultiplier; }
        Vector3 moveVector = new Vector3(currentMoveInput.x, 0.0f, currentMoveInput.y);
        transform.Translate(moveVector * moveSpeed * sprint * Time.deltaTime);

    }

    private void OnEnable()
    {
        moveInputUpdate.OnEventTriggered += IngestMoveUpdate;
        sprintInputUpdate.OnEventTriggered += IngestSprintUpdate;
    }
    private void OnDisable()
    {
        moveInputUpdate.OnEventTriggered -= IngestMoveUpdate;
        sprintInputUpdate.OnEventTriggered -= IngestSprintUpdate;
    }

    private void IngestMoveUpdate(Vector2 _input)
    {
        currentMoveInput = _input;
    }

    private void IngestSprintUpdate(bool _input)
    {
        isSprinting = _input;
    }




}
