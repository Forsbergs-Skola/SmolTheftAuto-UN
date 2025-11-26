using UnityEngine;
using GameTools;

public class Junk : MonoBehaviour
{
    /*
    // In the NPC script...
    private void dealDamage(RaycastHit hit)
    {
        // if it didn't hit the player, then do nothing
        if (hit.collider.gameObject.GetComponent<PlayerController>() == null) return;
        hit.collider.gameObject.GetComponent<PlayerController>().TakeDamage(EnumWeapon.RIFLE);
        //...^^or .PISTOL, or .STOTGUN
    }

    // In the player script...
    public void TakeDamage(EnumWeapon weapon)
    {
        int pistolDamage = -2;
        int rifleDamage = -5;
        int shotgunDamage = -10;

        switch (weapon)
        {
            case EnumWeapon.PISTOL:
                healthChangedEvent.TriggerEvent(pistolDamage);
                break;
            case EnumWeapon.RIFLE:
                healthChangedEvent.TriggerEvent(rifleDamage);
                break;
            case EnumWeapon.SHOTGUN:
                healthChangedEvent.TriggerEvent(shotgunDamage);
                break;
        }
        // play hit-react animation, make sound, etc...
    }
    */
}
