using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCollision : MonoBehaviour
{
    public float radius = 0.5f; // Player radius
    public RowManager rowManager; // Reference to RowManager
    public BoxCollider2D coll;
    public GameObject gameOverMenu;
    public bool invisible = false;
    public bool collisionDetected = false; // Prevent multiple detections

    public SpriteRenderer spriteRenderer; // Reference to player's sprite
    public Color normalColor = Color.white;  // Default color
    public Color boostColor = Color.yellow;  // Flashing color when booster is active

    void Start()
    {
    }

    void Update()
    {
        if (!invisible && !collisionDetected)
        {
            CheckCollisions();
        }
    }

    void CheckCollisions()
    {
        foreach (List<GameObject> row in rowManager.activeRows) // Get all active rows
        {
            foreach (GameObject block in row) // Check all blocks in each row
            {
                if (IsColliding(block))
                {
                    Debug.Log("Collision Detected with: " + block.name);
                    HandleCollision(block);
                    return; // Exit loop on first detected collision
                }
            }
        }
    }

    bool IsColliding(GameObject block)
    {
        Vector3 blockPos = block.transform.position;
        Vector3 blockScale = block.transform.localScale;

        float blockHalfWidth = blockScale.x / 2f;
        float blockHalfHeight = blockScale.y / 2f;

        float closestX = Mathf.Clamp(transform.position.x, blockPos.x - blockHalfWidth, blockPos.x + blockHalfWidth);
        float closestY = Mathf.Clamp(transform.position.y, blockPos.y - blockHalfHeight, blockPos.y + blockHalfHeight);

        float distanceX = transform.position.x - closestX;
        float distanceY = transform.position.y - closestY;

        return (distanceX * distanceX + distanceY * distanceY) < (radius * radius);
    }

    void HandleCollision(GameObject block)
    {
        if (collisionDetected) return; // Prevent multiple detections
        collisionDetected = true; // Stop further detections

        Debug.Log("Collision Handled with: " + block.name);
        block.GetComponent<CollisionBoundryLogic>().CollidedWith();
        coll.enabled = false;
        Time.timeScale = 0f;

        StartCoroutine(ShowGameOverMenu()); // Start a coroutine to wait 3 seconds
    }

    IEnumerator ShowGameOverMenu()
    {
        yield return new WaitForSecondsRealtime(3f); // Waits 3 seconds (ignores Time.timeScale)
        if (collisionDetected) gameOverMenu.SetActive(true);
    }

    public void ResetCollision()
    {
        collisionDetected = false;
        coll.enabled = true;
        Time.timeScale = 1f; // Resets game time
        gameOverMenu.SetActive(false);
    }

    //Activate Booster Effect
    public void ActivateBooster(float effectDuration)
    {
        StartCoroutine(FlashPlayerWhileActive(effectDuration));
    }

    private IEnumerator FlashPlayerWhileActive(float effectDuration)
    {
        float timeElapsed = 0f;
        float maxFlashSpeed = 0.1f; // Fastest flash interval
        float minFlashSpeed = 0.5f; // Slowest flash interval near the end

        while (timeElapsed < effectDuration)
        {
            float progress = timeElapsed / effectDuration; // 0 to 1 as time passes
            float flashSpeed = Mathf.Lerp(maxFlashSpeed, minFlashSpeed, progress); // Gradually slow down flashing

            spriteRenderer.color = boostColor;
            yield return new WaitForSeconds(flashSpeed);

            spriteRenderer.color = normalColor;
            yield return new WaitForSeconds(flashSpeed);

            timeElapsed += flashSpeed * 2; // Update elapsed time
        }

        // Ensure the final color is reset
        spriteRenderer.color = normalColor;
    }
}
