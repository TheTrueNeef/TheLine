using System.Collections.Generic;
using UnityEngine;

public class RowManager : MonoBehaviour
{
    [Header("Row Control Points")]
    public Transform spawnPoint;  // Position where new rows appear
    public Transform checkpoint;  // Triggers the next row spawn when reached
    public Transform deletePoint; // Position where rows are removed from the game

    [Header("Row Configuration")]
    public int[] rowPattern = { 1, 0, 1, 1, 0 }; // Defines the pattern of blocks and empty spaces in a row
    public GameObject blockPrefab;  // Prefab for standard blocks
    public GameObject boosterPrefab; // Prefab for boosters that can randomly appear
    public int poolSize = 30;       // Number of blocks to keep in the object pool for performance
    public float gridWidth = 8f;    // Width of the playable grid

    [Header("Speed Controls")]
    public float currentSpeed = 1.0f; // Speed at which rows move downward
    public float speedRampUp = 0.01f; // Rate at which speed increases over time

    private Queue<GameObject> blockPool = new Queue<GameObject>();  // Object pool for recycling blocks
    public List<List<GameObject>> activeRows = new List<List<GameObject>>();  // Stores active rows currently in play
    private int checkpointRowIndex = 0; // Tracks which row should trigger the next row spawn

    void Start()
    {
        InitializePool(); // Set up the object pool with pre-instantiated blocks
        SpawnRow(); // Spawn the first row at the start of the game
    }

    void Update()
    {
        CheckForCheckpoint(); // Check if any row reached the checkpoint to spawn a new row
        RecycleRows(); // Remove rows that have reached the delete point
        if (currentSpeed < 10.0f) currentSpeed += speedRampUp * Time.deltaTime; // Gradually increase speed
    }

    // Creates a pool of reusable blocks to optimize performance
    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject block = Instantiate(blockPrefab, Vector3.zero, Quaternion.identity);
            block.SetActive(false);
            blockPool.Enqueue(block);
        }
    }

    // Generates a new row based on the current row pattern
    void SpawnRow()
    {
        this.GetComponent<PathMaker>().ModifyRowPattern(); // Modify the row pattern before spawning
        if (rowPattern.Length == 0) return; // Ensure there is a valid pattern before spawning

        float blockWidth = gridWidth / rowPattern.Length; // Determine the width of each block
        float startX = -gridWidth / 2f + blockWidth / 2f; // Center the row properly

        List<GameObject> newRow = new List<GameObject>(); // List to store blocks in the new row

        for (int i = 0; i < rowPattern.Length; i++)
        {
            Vector3 spawnPos = new Vector3(startX + i * blockWidth, spawnPoint.position.y, 0.1f); // Position for each block

            if (rowPattern[i] == 1) // If the value is 1, spawn a block
            {
                if (blockPool.Count > 0)
                {
                    GameObject block = blockPool.Dequeue(); // Retrieve a block from the pool
                    block.transform.position = spawnPos;
                    block.transform.localScale = new Vector3(blockWidth, block.transform.localScale.y, block.transform.localScale.z);
                    block.SetActive(true);

                    newRow.Add(block);
                }
                else
                {
                    Debug.LogWarning("Object Pool is empty! Consider increasing pool size.");
                }
            }
            else if (rowPattern[i] == 0) // If the value is 0, there is a 10% chance to spawn a booster
            {
                if (boosterPrefab != null && Random.value <= 0.1f) // 10% chance
                {
                    Instantiate(boosterPrefab, spawnPos, Quaternion.identity);
                }
            }
        }

        if (newRow.Count > 0)
        {
            activeRows.Add(newRow); // Store the row if it contains any blocks
        }
    }

    // Checks if any row has reached the checkpoint and spawns a new row if necessary
    void CheckForCheckpoint()
    {
        if (activeRows.Count == 0) return; // If there are no active rows, do nothing
        if (checkpointRowIndex >= activeRows.Count) checkpointRowIndex = 0; // Ensure index stays within bounds

        List<GameObject> rowToCheck = activeRows[checkpointRowIndex]; // Select the current row to check

        foreach (GameObject block in rowToCheck)
        {
            Block blockScript = block.GetComponent<Block>();

            // If a block reaches the checkpoint and hasn't triggered the next row yet
            if (!blockScript.spawnedNextRow && block.transform.position.y <= checkpoint.position.y)
            {
                blockScript.spawnedNextRow = true; // Mark that this row triggered the next row spawn
                SpawnRow(); // Spawn the next row

                // Move to the next row for the next cycle
                checkpointRowIndex = (checkpointRowIndex + 1) % activeRows.Count;

                return; // Ensure only one row triggers a new row per frame
            }
        }
    }

    // Removes rows that have reached the delete point and returns blocks to the pool
    void RecycleRows()
    {
        if (activeRows.Count == 0) return; // If no rows exist, do nothing

        List<GameObject> firstRow = activeRows[0]; // Select the first row in the list

        // If the row has moved past the delete point, recycle its blocks
        if (firstRow.Count > 0 && firstRow[0].transform.position.y < deletePoint.position.y)
        {
            foreach (GameObject block in firstRow)
            {
                block.SetActive(false); // Disable the block
                blockPool.Enqueue(block); // Return the block to the object pool
            }

            activeRows.RemoveAt(0); // Remove the row from the list

            // Ensure the checkpointRowIndex stays within bounds
            if (checkpointRowIndex > 0) checkpointRowIndex--;
        }
    }

    // Resets all game objects and values to restart the game
    public void ResetObjects()
    {
        Time.timeScale = 0; // Pause the game

        // Disable all active rows and return blocks to the pool
        foreach (List<GameObject> row in activeRows)
        {
            foreach (GameObject block in row)
            {
                block.SetActive(false);
                blockPool.Enqueue(block);
            }
        }

        activeRows.Clear(); // Clear the active row list

        // Reset speed to initial value
        currentSpeed = 1.0f;

        // Reset the row pattern to the default size
        rowPattern = new int[] { 1, 0, 1 }; // Small pattern that can expand later

        // Reset row spawning index
        checkpointRowIndex = 0;

        Debug.Log("All objects have been reset. Speed and rows cleared.");

        // Spawn the initial row again to restart the game
        SpawnRow();
    }
}
