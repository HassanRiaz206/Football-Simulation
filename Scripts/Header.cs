using UnityEngine;

public class Header : MonoBehaviour
{
    public float headForceMagnitude = 8f; // Adjust this value in the Inspector
    public PlayerController playerController; // Assign the PlayerController in the Inspector
    public GameObject headCollider; // The designated head collider GameObject assigned via the Inspector

    // Use OnCollisionEnter to detect when the headCollider hits the football.
    private void OnCollisionEnter(Collision collision)
    {
        // Check if colliding with the football and the player has pressed Q (i.e. isHeading is true)
        if (collision.gameObject.CompareTag("Football") && playerController.isHeading)
        {
            // Ensure this collision is coming from the designated headCollider.
            if (this.gameObject == headCollider)
            {
                Rigidbody footballRigidbody = collision.gameObject.GetComponent<Rigidbody>();
                if (footballRigidbody != null)
                {
                    // Apply a force that is forward (based on headCollider’s forward) and upward.
                    Vector3 forceDirection = headCollider.transform.forward + Vector3.up;
                    footballRigidbody.AddForce(forceDirection.normalized * headForceMagnitude, ForceMode.Impulse);
                    playerController.isHeading = false; // Reset immediately after applying the head action
                    Debug.Log("Ball headed with designated head collider!");
                }
            }
        }
    }
}


