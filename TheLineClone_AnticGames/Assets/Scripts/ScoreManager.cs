using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public int score = 0;              // The player's current score
    public float scoreRate = 1f;       // Number of points added per second
    public float scoreRampUp = 0.01f;  // Rate at which scoreRate increases over time
    public float bestScore = 0;        // Stores the highest recorded score

    public TMP_Text scoreText;         // Reference to TMP UI text for in-game score display
    public TMP_Text scoreTextMenu;     // Reference to TMP UI text for the score in the end screen
    public TMP_Text bestScoreTextMenu; // Reference to TMP UI text for best score in the end screen

    public RowManager rowManager;      // Reference to RowManager to manage row behavior

    private float elapsedTime = 0f;    // Tracks time for incremental score updates
    private int nextMilestone = 100;   // Score threshold for triggering events (e.g., difficulty increase)

    void Update()
    {
        elapsedTime += Time.deltaTime; // Increment elapsed time

        // Every second, increase the score and ramp up the score rate
        if (elapsedTime >= 1f)
        {
            score += Mathf.RoundToInt(scoreRate); // Convert to int and add score
            scoreRate += scoreRampUp;  // Gradually increase the score rate
            elapsedTime = 0f; // Reset elapsed time for the next interval

            CheckMilestone(); // Check if the score milestone has been reached
        }

        // Update TMP text displays if all required text objects are assigned
        if (scoreText != null && scoreTextMenu != null && bestScoreTextMenu != null)
        {
            scoreText.text = score.ToString();         // Update in-game score text
            scoreTextMenu.text = score.ToString();     // Update score text in the end screen
            bestScoreTextMenu.text = bestScore.ToString(); // Update best score text in the end screen
        }
    }

    // Checks if the player has reached the next milestone
    private void CheckMilestone()
    {
        if (score >= nextMilestone)
        {
            OnMilestoneReached(); // Perform milestone event actions
            nextMilestone += 100; // Set the next milestone threshold
        }
    }

    // Handles actions when a milestone is reached (e.g., increasing difficulty)
    private void OnMilestoneReached()
    {
        Debug.Log("Milestone reached: " + score);

        // Expand the row pattern in RowManager if it is below the maximum allowed size
        if (rowManager != null && rowManager.rowPattern.Length < 11)
        {
            ExpandRowPattern();
        }
    }

    // Expands the row pattern array in RowManager, increasing the game difficulty
    private void ExpandRowPattern()
    {
        int newSize = rowManager.rowPattern.Length + 1; // Increase array size by 1
        newSize = Mathf.Clamp(newSize, 3, 11); // Ensure it stays within the range of 3-11

        int[] newPattern = new int[newSize]; // Create a new array with the expanded size

        // Copy existing pattern values into the new array
        for (int i = 0; i < rowManager.rowPattern.Length; i++)
        {
            newPattern[i] = rowManager.rowPattern[i];
        }

        // Randomly assign either 0 or 1 to the newly added index
        newPattern[newSize - 1] = Random.value > 0.5f ? 1 : 0;

        rowManager.rowPattern = newPattern; // Apply the updated pattern
        Debug.Log("Expanded Row Pattern to size: " + newSize);
    }
}
