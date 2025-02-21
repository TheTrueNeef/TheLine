using UnityEngine;

public class Block : MonoBehaviour
{
    public bool spawnedNextRow = false; // This will be checked by RowManager

    void OnEnable()
    {
        spawnedNextRow = false; // Reset when reusing from the pool
    }
}
