using UnityEngine;
using Events;

public class UiTestPlayer : MonoBehaviour
{
    [SerializeField] private Vector2PayloadEvent moveInputEvent;
    [SerializeField] private StringPayloadEvent dialogueStartEvent;
    [SerializeField] private StringPayloadEvent dialogueFinishedEvent;


    [SerializeField] private IntPayloadEvent moneyEvent;


    [SerializeField] private EnumWeaponPayloadEvent weaponReloadEvent;

    private void OnEnable()
    {
        moveInputEvent.OnEventTriggered += HandleMoveInput;
        dialogueStartEvent.OnEventTriggered += HandleOnDialogueStarted;
        dialogueFinishedEvent.OnEventTriggered += HandleOnDialogueFinished;
    }

    private void OnDisable()
    {
        moveInputEvent.OnEventTriggered -= HandleMoveInput;
        dialogueStartEvent.OnEventTriggered -= HandleOnDialogueStarted;
        dialogueFinishedEvent.OnEventTriggered -= HandleOnDialogueFinished;
    }

    private void HandleMoveInput(Vector2 moveInput)
    {
        
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb.isKinematic == true) return;
        Vector3 velocity = new Vector3(moveInput.x, 0f, moveInput.y);
        rb.linearVelocity = velocity * 4;
    }

    private void HandleOnDialogueStarted(string _str)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void HandleOnDialogueFinished(string _str)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<TestMoney>() != null)
        {
            TestMoney theMoney = other.gameObject.GetComponent<TestMoney>();
            if (!theMoney.collectible) return;

            Debug.Log($"PLAYER CONTROLLER says: I just picked up {theMoney.amount} money!");
            theMoney.collectible = false;

            int moneyAmount = theMoney.amount;
            theMoney.DoPickupStuff();
            moneyEvent.TriggerEvent(moneyAmount);
        }
    }

    //[SerializeField] private EnumWeaponPayloadEvent weaponReloadEvent;
    private void ReloadPistol()
    {
        weaponReloadEvent.TriggerEvent(EnumWeapon.PISTOL);
    }
    private void ReloadShotgun()
    {
        weaponReloadEvent.TriggerEvent(EnumWeapon.SHOTGUN);
    }
    private void ReloadRifle()
    {
        weaponReloadEvent.TriggerEvent(EnumWeapon.RIFLE);
    }

}
