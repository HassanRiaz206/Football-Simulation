using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip mainSound;  // Main background sound
    public AudioClip goalSound;  // Goal Sound
    public AudioClip kickSound;  // Kick Sound

    [Header("Kick Objects")]
    // Assign the GameObjects in the Inspector that will trigger the kick sound.
    public GameObject[] kickObjects;

    [Header("Football Object")]
    // Assign your Football GameObject in the Inspector.
    public GameObject football;

    [Header("Scoreboard Panel")]
    // Assign your Scoreboard Panel in the Inspector.
    public GameObject scoreboardPanel;

    private AudioSource bgSource;
    private AudioSource effectSource;
    private bool soundsStopped = false;

    void Start()
    {
        bgSource = gameObject.AddComponent<AudioSource>();
        effectSource = gameObject.AddComponent<AudioSource>();

        // Setup and start playing the background sound at half volume.
        if (mainSound != null)
        {
            bgSource.clip = mainSound;
            bgSource.volume = 0.1f; // Reduced volume (half of 0.1f)
            bgSource.loop = true;
            bgSource.Play();
        }
        else
        {
            Debug.LogWarning("Main sound not assigned in the Inspector.");
        }

        if (football != null)
        {
            FootballCollisionHandler handler = football.GetComponent<FootballCollisionHandler>();
            if (handler == null)
            {
                handler = football.AddComponent<FootballCollisionHandler>();
            }
            handler.manager = this;
        }
        else
        {
            Debug.LogWarning("Football GameObject not assigned in the Inspector.");
        }
    }

    void Update()
    {
        // If the scoreboard panel is active, ensure sounds remain stopped.
        if (scoreboardPanel != null)
        {
            if (scoreboardPanel.activeSelf)
            {
                if (!soundsStopped)
                {
                    StopAllSounds();
                    soundsStopped = true;
                }
            }
            else
            {
                // If the scoreboard panel is inactive and sounds were previously stopped, resume the background sound.
                if (soundsStopped)
                {
                    if (mainSound != null)
                    {
                        bgSource.clip = mainSound;
                        bgSource.volume = 0.1f;
                        bgSource.loop = true;
                        bgSource.Play();
                    }
                    soundsStopped = false;
                }
            }
        }
    }

    public void ProcessCollision(GameObject other)
    {
        // If the Scoreboard Panel is active, do not process collision sounds.
        if (scoreboardPanel != null && scoreboardPanel.activeSelf)
        {
            Debug.Log("Scoreboard active, collision sounds suppressed.");
            return;
        }

        Debug.Log("Football collided with: " + other.name);

        // If colliding with an object tagged "plat", play the goal sound.
        if (other.CompareTag("plat"))
        {
            Debug.Log("Collision with plat detected.");
            PlayGoalSound();
        }

        // Check if the collision is with one of the assigned kick objects.
        foreach (GameObject kickObj in kickObjects)
        {
            if (other == kickObj)
            {
                Debug.Log("Collision with Kick Object: " + kickObj.name);
                PlayKickSound();
                break;
            }
        }
    }

    void PlayGoalSound()
    {
        // Play the goal sound immediately at volume 0.25.
        if (goalSound != null)
        {
            effectSource.PlayOneShot(goalSound, 0.25f);
        }
        else
        {
            Debug.LogWarning("Goal sound not assigned in the Inspector.");
        }
    }

    void PlayKickSound()
    {
        // Play the kick sound immediately.
        if (kickSound != null)
        {
            effectSource.PlayOneShot(kickSound, 1f);
        }
        else
        {
            Debug.LogWarning("Kick sound not assigned in the Inspector.");
        }
    }

    // This method can be called (for example, via your Scoreboard panel) to stop all sounds.
    public void StopAllSounds()
    {
        bgSource.Stop();
        effectSource.Stop();
        Debug.Log("All sounds stopped.");
    }
}

// It forwards collision events to the SoundManager.
public class FootballCollisionHandler : MonoBehaviour
{
    [HideInInspector]
    public SoundManager manager;

    void OnCollisionEnter(Collision collision)
    {
        if (manager != null)
        {
            manager.ProcessCollision(collision.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (manager != null)
        {
            manager.ProcessCollision(other.gameObject);
        }
    }
}
