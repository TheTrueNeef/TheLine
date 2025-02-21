using UnityEngine;
using System.Collections;

public class Booster : MonoBehaviour
{
    public int boosterType = 0; // Defines the type of booster (0 = invincibility, 1 = smaller radius, etc.)
    public float pickupRadius = 1.0f; // The distance at which the player can pick up the booster
    public float effectDuration = 5f; // How long the booster effect lasts

    private GameObject player; // Reference to the player object
    private RowManager rowManager; // Reference to the RowManager for game speed

    void Start()
    {
        // Find the player in the scene (player must have the tag "Player")
        player = GameObject.FindWithTag("Player");

        // Find the RowManager in the scene and assign it
        GameObject rowM = GameObject.Find("RowManager");
        rowManager = rowM.GetComponent<RowManager>();
    }

    void Update()
    {
        // Move the booster downward at the same speed as blocks (retrieved from RowManager)
        transform.position += Vector3.down * rowManager.currentSpeed * Time.deltaTime;

        // Check if the player is within the pickup radius
        if (player != null && IsPlayerNear())
        {
            // Activate the booster effect and move the object off-screen instead of destroying it
            OnBoosterCollected(player);
            this.transform.position = new Vector3(100, 100, 0);
        }
    }

    bool IsPlayerNear()
    {
        // Calculate the distance between the booster and the player
        float distance = Vector3.Distance(transform.position, player.transform.position);

        // Return true if the player is within the pickup radius
        return distance <= pickupRadius;
    }

    void OnBoosterCollected(GameObject player)
    {
        // Log booster collection for debugging
        Debug.Log("Booster collected! Type: " + boosterType);

        // Get the PlayerCollision script attached to the player
        PlayerCollision playerScript = player.GetComponent<PlayerCollision>();

        // Call the function to activate the booster effect visually
        playerScript.ActivateBooster(effectDuration);

        // Ensure the playerScript is valid before applying the booster effect
        if (playerScript != null)
        {
            switch (boosterType)
            {
                case 0:
                    // Start the invincibility effect coroutine
                    StartCoroutine(ActivateInvincibility(playerScript));
                    break;

                case 1:
                    // Start the shrinking effect coroutine
                    StartCoroutine(ShrinkPlayerRadius(playerScript));
                    break;

                default:
                    break;
            }
        }
    }

    IEnumerator ActivateInvincibility(PlayerCollision playerScript)
    {
        // Set the player to be invincible
        Debug.Log("Invincibility Activated!");
        playerScript.invisible = true;

        // Wait for the effect duration before turning off invincibility
        yield return new WaitForSeconds(effectDuration);

        // Reset the player's invincibility status
        playerScript.invisible = false;
        Debug.Log("Invincibility Ended!");
    }

    IEnumerator ShrinkPlayerRadius(PlayerCollision playerScript)
    {
        // Store the original radius before shrinking
        Debug.Log("Player Shrinking!");
        float originalRadius = playerScript.radius;

        // Reduce the player's radius by 50%
        playerScript.radius *= 0.5f;

        // Wait for the effect duration before restoring the radius
        yield return new WaitForSeconds(effectDuration);

        // Reset the player's radius to its original value
        playerScript.radius = originalRadius;
        Debug.Log("Player Back to Normal!");
    }
}
