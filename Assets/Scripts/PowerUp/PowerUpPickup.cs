using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    [SerializeField] private float driftSpeed = 1.5f;
    [SerializeField] private float bobAmplitude = 0.15f;
    [SerializeField] private float bobFrequency = 2f;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        float bobVelocity = bobAmplitude * bobFrequency * Mathf.Cos(Time.time * bobFrequency);
        rb.velocity = new Vector2(0f, -driftSpeed + bobVelocity);

        if (transform.position.y < -6f) Destroy(gameObject);
    }
}
