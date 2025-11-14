using UnityEngine;

public class ExampleBullet : MonoBehaviour
{
    private const float SPEED = 20.0f;
    private const float LIFE = 1.0f;
    private Rigidbody myRB;
    public float damage = 10.0f;

    private void Start()
    {
        myRB = GetComponent<Rigidbody>();
        myRB.linearVelocity = new Vector3(SPEED, 0.0f, 0.0f);
        StartCoroutine(StartLifetime());
    }
    public void DieEarly()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }
    private System.Collections.IEnumerator StartLifetime()
    {
        yield return new WaitForSeconds(LIFE);
        Destroy(gameObject);
    }
}
