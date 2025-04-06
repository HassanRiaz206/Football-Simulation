using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    // Loads the Tournament Selection scene when the Play button is pressed.
    public void OnPlayButtonPressed()
    {
        SceneManager.LoadScene("Tournament Selection");
    }

    // Loads the Tournament Selection scene when the Restart button is pressed.
    public void OnRestartButtonPressed()
    {
        SceneManager.LoadScene("Tournament Selection");
    }

    // Exits the game when the Exit button is pressed.
    public void OnExitButtonPressed()
    {
        Application.Quit();
    }
}
