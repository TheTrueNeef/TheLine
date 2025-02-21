using UnityEngine;

public class PathMaker : MonoBehaviour
{
    public RowManager rowManager; // Reference to the RowManager that controls row patterns

    void Start()
    {
        // If the RowManager is not assigned in the Inspector, find it in the scene automatically
        if (rowManager == null)
        {
            rowManager = FindObjectOfType<RowManager>();
        }
    }

    // Modifies the row pattern by randomly adjusting values to ensure paths are possible
    public void ModifyRowPattern()
    {
        // Ensure RowManager is assigned and the row pattern has at least 3 elements to prevent errors
        if (rowManager == null || rowManager.rowPattern.Length < 3) return;

        // Clone the current row pattern so modifications do not affect the original directly
        int[] newPattern = (int[])rowManager.rowPattern.Clone();

        for (int i = 0; i < newPattern.Length; i++)
        {
            // If the current position in the pattern is 0, randomly determine how it should change
            if (rowManager.rowPattern[i] == 0)
            {
                bool extendLeft = (i > 0) && Random.value > 0.7f; // 30% chance to extend left
                bool extendRight = (i < newPattern.Length - 1) && Random.value > 0.7f; // 30% chance to extend right
                bool turnIntoOne = Random.value > 0.2f; // 80% chance to turn into 1

                // If extending left, set the previous index to 0
                if (extendLeft) newPattern[i - 1] = 0;

                // If extending right, set the next index to 0
                if (extendRight) newPattern[i + 1] = 0;

                // If the conditions are met, turn the current 0 into a 1 while maintaining an open path
                if (turnIntoOne && ((i > 0 && rowManager.rowPattern[i - 1] == 0) || (i < newPattern.Length - 1 && rowManager.rowPattern[i + 1] == 0)))
                {
                    if (i < newPattern.Length - 1 && rowManager.rowPattern[i + 1] == 0)
                    {
                        newPattern[i] = 1;
                        newPattern[i + 1] = 0;
                    }
                    else if (rowManager.rowPattern[i - 1] == 0)
                    {
                        newPattern[i] = 1;
                        newPattern[i - 1] = 0;
                    }
                }
            }
        }

        // Ensure that the row contains at least one path
        EnsureAtLeastOnePath(newPattern);

        // Apply the modified pattern back to the RowManager
        rowManager.rowPattern = newPattern;
    }

    // Ensures that at least one index in the pattern is set to 1 to prevent completely blocked paths
    private void EnsureAtLeastOnePath(int[] pattern)
    {
        bool hasPath = false;

        // Check if there is at least one 1 in the pattern
        for (int i = 0; i < pattern.Length; i++)
        {
            if (pattern[i] == 1)
            {
                hasPath = true;
                break;
            }
        }

        // If no path exists, randomly set one index to 1 to ensure a valid route
        if (!hasPath)
        {
            int randomIndex = Random.Range(0, pattern.Length);
            pattern[randomIndex] = 1;
        }
    }
}
