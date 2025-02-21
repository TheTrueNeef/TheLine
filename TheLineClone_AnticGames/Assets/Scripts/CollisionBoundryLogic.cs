using UnityEngine;

public class CollisionBoundryLogic : MonoBehaviour
{
    public float speed = 1.0f; // Speed at which the object moves downward
    private bool isActive = true; // Determines if the object is currently active

    public RowManager rowManager; // Reference to RowManager to access the game speed

    public Color color = Color.white;  // Default color of the object
    public Color cMain = Color.red;    // Color used for flashing effect
    public float flashSpeed = 0.1f;    // Speed at which the object flashes when colliding

    private SpriteRenderer spriteRenderer; // Reference to the object's SpriteRenderer
    private bool isFlashing = false; // Flag to indicate if the object is flashing
    private float lastFlashTime = 0f; // Tracks the last time the object flashed
    private bool isMainColor = false; // Toggle flag to alternate between colors

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component

        // Find the RowManager in the scene and assign it
        GameObject rowM = GameObject.Find("RowManager");
        rowManager = rowM.GetComponent<RowManager>();
    }

    void Update()
    {
        // If the object is active, move it downward at the defined speed
        if (isActive)
        {
            transform.position += Vector3.down * speed * Time.deltaTime;
        }

        // Update the object's speed to match the current speed from RowManager
        speed = rowManager.currentSpeed;

        // Handle flashing logic when required
        FlashColorUnscaled();
    }

    public void Activate(Vector3 startPosition)
    {
        transform.position = startPosition; // Set the object's position to the given start position
        isActive = true; // Mark the object as active
        gameObject.SetActive(true); // Enable the object in the scene
    }

    public void Deactivate()
    {
        isActive = false; // Mark the object as inactive
        gameObject.SetActive(false); // Disable the object in the scene
        isFlashing = false; // Stop flashing effect
        spriteRenderer.color = color; // Reset the object to its default color
    }

    public void CollidedWith()
    {
        StartFlashing(); // Trigger the flashing effect upon collision
    }

    private void StartFlashing()
    {
        if (!isFlashing) // If not already flashing, start flashing
        {
            isFlashing = true; // Enable the flashing effect
            lastFlashTime = Time.unscaledTime; // Store the current time to track flashing intervals
        }
    }

    private void FlashColorUnscaled()
    {
        if (!isFlashing) return; // If flashing is not active, do nothing

        // Check if enough time has passed to toggle the color
        if (Time.unscaledTime - lastFlashTime >= flashSpeed)
        {
            spriteRenderer.color = isMainColor ? color : cMain; // Alternate between the two colors
            isMainColor = !isMainColor; // Toggle the color flag
            lastFlashTime = Time.unscaledTime; // Update last flash time to track the next interval
        }
    }
}
