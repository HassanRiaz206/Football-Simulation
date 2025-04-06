using UnityEngine;

public class PauseManager : MonoBehaviour
{
    // Assign the four scripts (components) you want to disable/enable in the Inspector.
    public MonoBehaviour[] scriptsToToggle;

    // Assign your Pause and Play button GameObjects (or UI buttons).
    public GameObject pauseButton;
    public GameObject playButton;

    // Assign the ball GameObject to freeze/unfreeze.
    public GameObject ball;

    // Assign your Scoreboard Panel (when active, buttons will be disabled).
    public GameObject scoreboardPanel;

    // Private variables to hold the ball's Rigidbody and its original constraints.
    private Rigidbody ballRigidbody;
    private RigidbodyConstraints originalConstraints;

    void Start()
    {
        // Get the ball's Rigidbody and store its original constraints.
        if (ball != null)
        {
            ballRigidbody = ball.GetComponent<Rigidbody>();
            if (ballRigidbody != null)
            {
                originalConstraints = ballRigidbody.constraints;
            }
        }
    }

    // Check every frame if the scoreboard panel is active.
    void Update()
    {
        if (scoreboardPanel != null && scoreboardPanel.activeSelf)
        {
            // Force both Pause and Play buttons to be inactive when the scoreboard is visible.
            if (pauseButton != null) pauseButton.SetActive(false);
            if (playButton != null) playButton.SetActive(false);
        }
    }

    // This function should be linked to the Pause button's OnClick event.
    public void PauseGame()
    {
        // Disable each assigned script.
        foreach (MonoBehaviour script in scriptsToToggle)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }

        // Freeze the ball by freezing all its Rigidbody movement.
        if (ballRigidbody != null)
        {
            ballRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }

        // Switch button visibility.
        if (pauseButton != null) pauseButton.SetActive(false);
        if (playButton != null) playButton.SetActive(true);
    }

    // This function should be linked to the Play button's OnClick event.
    public void ResumeGame()
    {
        // Enable each assigned script.
        foreach (MonoBehaviour script in scriptsToToggle)
        {
            if (script != null)
            {
                script.enabled = true;
            }
        }

        // Unfreeze the ball by restoring its original constraints.
        if (ballRigidbody != null)
        {
            ballRigidbody.constraints = originalConstraints;
        }

        // Switch button visibility.
        if (pauseButton != null) pauseButton.SetActive(true);
        if (playButton != null) playButton.SetActive(false);
    }
}
