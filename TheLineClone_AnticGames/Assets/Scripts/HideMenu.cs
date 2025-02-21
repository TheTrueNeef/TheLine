using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideMenu : MonoBehaviour
{
    [Header("Reset Functions For New Session")]
    public GameObject gameOverMenu; // Reference to the game over menu UI
    public RowManager rowM; // Reference to the RowManager for resetting game objects
    public BoxCollider2D coll; // Reference to the player's collider
    public GameObject player; // Reference to the player object
    public GameObject inputText; // Reference to the input field UI
    public ScoreManager scoreManager; // Reference to the ScoreManager to track and reset scores

    [Header("Main Menu")]
    public GameObject mainMenu; // Reference to the main menu UI

    // Hides the game over menu and resets all necessary game elements for a new session
    public void HideGameOverMenu()
    {
        gameOverMenu.SetActive(false); // Hide the game over menu
        mainMenu.SetActive(false); // Hide the main menu

        rowM.ResetObjects(); // Reset all objects in the game (rows, speed, etc.)
        coll.enabled = true; // Re-enable the player's collider to detect collisions

        // Reset the player's position to the center while keeping the same Y and Z values
        player.transform.position = new Vector3(0, player.transform.position.y, player.transform.position.z);

        inputText.SetActive(true); // Enable the input field UI

        // Reset collision detection so the player can detect new collisions
        player.GetComponent<PlayerCollision>().collisionDetected = false;

        // If the current score is higher than the best score, update the best score
        if (scoreManager.score > scoreManager.bestScore)
        {
            scoreManager.bestScore = scoreManager.score;
        }

        // Reset the player's score to zero
        scoreManager.score = 0;
    }

    // Opens the pause menu and stops the game
    public void openPauseMenu()
    {
        mainMenu.SetActive(true); // Show the main menu UI
        Time.timeScale = 0; // Pause the game
    }

    // Closes the pause menu and resumes the game
    public void keepGoing()
    {
        mainMenu.SetActive(false); // Hide the main menu UI
        Time.timeScale = 1.0f; // Resume the game
    }
}
