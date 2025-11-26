using UnityEngine;

namespace Vehicles
{
    [RequireComponent(typeof(Rigidbody))]
    public class VehicleMover : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float acceleration = 25f;
        [SerializeField] private float maxSpeed = 20f;
        [SerializeField] private float steeringSpeed = 90f;
        [SerializeField] private float brakeStrength = 40f;

        private Rigidbody rb;

        // Thes are the inputs set by the player or AI
        private float throttleInput;
        private float steerInput;
        private bool brakePressed;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.centerOfMass += Vector3.down * 0.5f; // helps stability
        }

        //this will be called by the input system or AI to set the movement
        public void SetInput(float throttle, float steer, bool brake)
        {
            throttleInput = Mathf.Clamp(throttle, -1f, 1f);
            steerInput   = Mathf.Clamp(steer,   -1f, 1f);
            brakePressed = brake;
        }

        private void FixedUpdate()
        {
            // Forward accelerate / reverse
            if (!brakePressed)
            {
                Vector3 force = transform.forward * (throttleInput * acceleration);
                rb.AddForce(force, ForceMode.Acceleration);
            }
            else
            {
                // simple brake: push opposite of current velocity?
                Vector3 v = rb.linearVelocity;
                if (v.sqrMagnitude > 0.01f)
                {
                    rb.AddForce(-v.normalized * brakeStrength, ForceMode.Acceleration);
                }
            }

            // Limit horizontal speed
            Vector3 vel = rb.linearVelocity;
            Vector3 horiz = new Vector3(vel.x, 0f, vel.z);
            if (horiz.magnitude > maxSpeed)
            {
                horiz = horiz.normalized * maxSpeed;
                rb.linearVelocity = new Vector3(horiz.x, vel.y, horiz.z);
            }

            // Steering (only if moving a bit)
            if (horiz.magnitude > 0.1f)
            {
                float turn = steerInput * steeringSpeed * Time.fixedDeltaTime;
                Quaternion rot = Quaternion.Euler(0f, turn, 0f);
                rb.MoveRotation(rb.rotation * rot);
            }
        }
    }
}
