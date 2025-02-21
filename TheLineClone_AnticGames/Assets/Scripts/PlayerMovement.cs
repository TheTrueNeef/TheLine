using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform player; // Reference to the player transform, assigned in the Inspector
    public BoxCollider2D boxCollider; // Reference to the collider that defines movement bounds
    public GameObject input; // Reference to the UI input object
    public PlayerCollision pColl; // Reference to the PlayerCollision script for managing collisions

    void Start()
    {
        Time.timeScale = 0; // Pause the game at the start to wait for player input
        input.SetActive(true); // Show the input UI to indicate the game is waiting to start
    }

    void Update()
    {
        // If the game is stopped and the player clicks, start the game
        if (Input.GetMouseButtonDown(0) && Time.timeScale == 0)
        {
            Time.timeScale = 1; // Resume the game
            input.SetActive(false); // Hide the input UI
            pColl.collisionDetected = false; // Reset collision detection
        }

        // If the player clicks or holds the mouse button, move the player horizontally
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            // Convert the screen space mouse position to world coordinates
            Vector3 worldClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldClickPos.z = 0f; // Keep the Z-axis fixed to prevent unintended movement

            // Check if the click is within the defined bounds (BoxCollider2D)
            if (boxCollider.bounds.Contains(worldClickPos))
            {
                // Move the player only in the X-axis while keeping Y and Z the same
                player.position = new Vector3(worldClickPos.x, player.position.y, 0f);
            }
        }
    }
}
