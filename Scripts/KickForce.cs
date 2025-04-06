using UnityEngine;

public class KickForce : MonoBehaviour
{
    public float kickForceMagnitude = 10f;   // Adjust the forward kick strength
    public float bounceForce = 5f;           // Adjust the upward bounce force
    public PlayerController playerController; // Assign the appropriate PlayerController in the Inspector

    private Rigidbody ballRigidbody;
    private bool ballInContact = false;

    // When the football first touches the collider.
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Football"))
        {
            ballInContact = true;
            ballRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Debug.Log("Football collision entered");
            // If the kick flag is active, kick immediately.
            if (playerController.isKicking)
            {
                KickBall();
            }
        }
    }

    // While the football remains in contact.
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Football"))
        {
            ballInContact = true;
            ballRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            // If the kick flag is active during continuous contact, kick.
            if (playerController.isKicking)
            {
                KickBall();
            }
        }
    }

    // When the football leaves the collider.
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Football"))
        {
            ballInContact = false;
            ballRigidbody = null;
            Debug.Log("Football collision exited");
        }
    }

    // Extra safety check in Update in case collision events haven't triggered the kick.
    void Update()
    {
        if (ballInContact && playerController.isKicking)
        {
            KickBall();
        }
    }

    // Applies the kick force.
    void KickBall()
    {
        // Only trigger if still in kicking mode.
        if (ballRigidbody != null && playerController.isKicking)
        {
            // Use the player's forward direction and add an upward component.
            Vector3 forceDirection = transform.forward;
            Vector3 kickForce = forceDirection * kickForceMagnitude + Vector3.up * bounceForce;
            ballRigidbody.AddForce(kickForce, ForceMode.Impulse);
            Debug.Log("Ball kicked with force: " + kickForce);

            // Reset the kick flag so that the kick only triggers once per activation.
            playerController.isKicking = false;
            ballInContact = false;
        }
    }
}
