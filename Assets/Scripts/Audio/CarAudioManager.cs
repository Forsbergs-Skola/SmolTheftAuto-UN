using UnityEngine;

public class CarAudioManager : MonoBehaviour
{
    public AudioSource idleSource;
    public AudioSource driveSource;
    public float maxSpeed = 20f;

    private Rigidbody rb;

    void Start() => rb = GetComponent<Rigidbody>();

    void Update()
    {
        float speed = rb.linearVelocity.magnitude;
        float blending = Mathf.Clamp01(speed / maxSpeed);

        idleSource.volume = 1f - blending;   
        driveSource.volume = blending;
        driveSource.pitch = Mathf.Lerp(1f, 2f, blending);
    }
    
}