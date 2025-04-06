using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // For human player only. If you have multiple players, don't overwrite the instance.
    public static PlayerController instance;
    public float moveSpeed = 100f;
    public float jumpForce = 10f;
    private bool isGrounded = true;
    private float fixedZPosition;
    public int player; // 1 for human player, 2 for AI
    private Vector3 startPosition; // Used for human player movement limits

    public bool isKicking = false;
    public bool isHeading = false;
    public bool isKickingCooldown = false;
    private Rigidbody rb;

    // AI delay
    private bool canMove = false;

    public GameObject headCollider;

    void Start()
    {
        // Only assign the static instance for the human player (player == 1)
        if (player == 1)
        {
            instance = this;
        }
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        fixedZPosition = transform.position.z;
        startPosition = transform.position;
    }

    // This method is called every time the script is enabled.
    void OnEnable()
    {
        if (player == 2)
        {
            canMove = false;
            StartCoroutine(AIFreezeStart());
        }
        else if (player == 1)
        {
            canMove = true;
        }
    }

    IEnumerator AIFreezeStart()
    {
        yield return new WaitForSeconds(1f);
        canMove = true;
    }

    void FixedUpdate()
    {
        if (player == 1)
        {
            MovePlayer();
        }
        else if (player == 2)
        {
            if (!canMove)
                return; // AI remains frozen until delay is over
            AIMove();
        }
    }

    void Update()
    {
        if (player == 1)
        {
            // For human player, use key presses to kick or head.
            if (Input.GetKeyDown(KeyCode.S))
            {
                Kick();
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Head();
            }
        }
    }

    // Player Movement
    void MovePlayer()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 move = new Vector3(horizontalInput, 0f, 0f) * moveSpeed * Time.deltaTime;
        Vector3 newPosition = rb.position + move;
        float minX = startPosition.x - 9f;
        float maxX = startPosition.x + 9f;
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = fixedZPosition;
        rb.position = newPosition;

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    // AI movement follows the ball without clamping.
    void AIMove()
    {
        Transform ballTransform = FootballBehavior.instance.transform;

        if (isKickingCooldown)
        {
            // When in cooldown, move away from the ball.
            Vector3 directionAway = (transform.position - ballTransform.position).normalized;
            Vector3 move = directionAway * moveSpeed * Time.deltaTime;
            Vector3 newPosition = rb.position + move;
            newPosition.z = fixedZPosition;
            rb.position = newPosition;
        }
        else
        {
            // Move towards the ball.
            Vector3 direction = (ballTransform.position - transform.position).normalized;
            Vector3 move = direction * moveSpeed * Time.deltaTime;
            Vector3 newPosition = rb.position + move;
            newPosition.z = fixedZPosition;
            rb.position = newPosition;

            // When close enough, perform actions based on the ball’s position.
            if (Vector3.Distance(transform.position, ballTransform.position) < 1f)
            {
                if (isGrounded && ballTransform.position.y > transform.position.y + 0.5f)
                {
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                    isGrounded = false;
                }
                else if (ballTransform.position.y < 0.1f)
                {
                    Kick();
                }
                else if (ballTransform.position.y > transform.position.y)
                {
                    Head();
                }
            }
        }
    }

    public void Kick()
    {
        if (!isKicking)
        {
            isKicking = true;
            StartCoroutine(ResetKicking());
            if (player == 2)
            {
                isKickingCooldown = true;
                StartCoroutine(ResetKickingCooldown());
            }
        }
    }

    public void Head()
    {
        if (!isHeading)
        {
            isHeading = true;
            StartCoroutine(ResetHeading());
        }
    }

    IEnumerator ResetKicking()
    {
        yield return new WaitForSeconds(0.5f);
        isKicking = false;
    }

    IEnumerator ResetHeading()
    {
        yield return new WaitForSeconds(0.5f);
        isHeading = false;
    }

    IEnumerator ResetKickingCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        isKickingCooldown = false;
    }

    // On collision with ground or football.
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        // If the AI player collides with the football, auto-trigger a kick.
        if (player == 2 && collision.gameObject.CompareTag("Football"))
        {
            Kick();
        }
    }
}
