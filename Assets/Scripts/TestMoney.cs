using UnityEngine;

public class TestMoney : MonoBehaviour
{
    public int amount = 10;
    public bool collectible = true;

    public void DoPickupStuff()
    {
        Debug.Log("MONEY says: I have been collected");
        StartCoroutine(OnPickedUp()); // <-- this is just an example. This could be VFX, SFX, etc.
    }


    private System.Collections.IEnumerator OnPickedUp() // <-- Again, just an example of an on pickup behavior
    {
        Collider mycollider = GetComponent<Collider>();
        int count = 20;
        while (count > 0)
        {
            transform.localScale *= 0.5f;
            transform.position += Vector3.up * 0.25f;
            yield return new WaitForSeconds(0.1f);
            count--;
        }
        Destroy(gameObject);
    }
}
