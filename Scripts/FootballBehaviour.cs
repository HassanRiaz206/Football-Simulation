using UnityEngine;

public class FootballBehavior : MonoBehaviour
{
    public static FootballBehavior instance;

    private Rigidbody rb;
    private float maxYPosition = 4.6f;
    private float bounceForce = 1f; // Reduced for less bouncing
    private float downwardForce = 0.5f;
    private float maxXPosition = 10.95f;
    private float minXPosition = -11.95f;
    private float sideBounceForce = 1f;

    void Start()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionZ;
        rb.mass = 1f; // Default is 1, adjust if needed
    }

    void FixedUpdate()
    {
        // Apply upward force only when falling below threshold, not when stationary
        if (transform.position.y < -3.16f && rb.velocity.y < 0)
        {
            rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
        }

        // Cap height and apply downward force
        if (transform.position.y > maxYPosition)
        {
            rb.AddForce(Vector3.down * downwardForce, ForceMode.Impulse);
            Vector3 newPosition = transform.position;
            newPosition.y = maxYPosition;
            transform.position = newPosition;
        }

        // Side boundaries
        if (transform.position.x < minXPosition)
        {
            rb.AddForce(Vector3.right * sideBounceForce, ForceMode.Impulse);
            Vector3 newPosition = transform.position;
            newPosition.x = minXPosition;
            transform.position = newPosition;
        }

        if (transform.position.x > maxXPosition)
        {
            rb.AddForce(Vector3.left * sideBounceForce, ForceMode.Impulse);
            Vector3 newPosition = transform.position;
            newPosition.x = maxXPosition;
            transform.position = newPosition;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Random bounce on collision (e.g., with ground or players)
        Vector3 randomBounce = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0.2f, 0.5f), 0f);
        rb.AddForce(randomBounce * bounceForce, ForceMode.Impulse);
    }
}