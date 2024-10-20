using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    private GameObject enemy; // The enemy prefab to be instantiated

    [SerializeField]
    private Transform spawnPoint; // The point where enemies will spawn

    [SerializeField]
    private GameObject tower; // The tower that enemies will target

    private int currentWave = 0; // Current wave count
    private int enemiesPerWave = 1; // Number of enemies per wave
    private int limit; // Limit for spawning enemies

    private EnemyMovement enemyMovement;

    private GridManager gridManager;

    // private GridManager gridManager;

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        if (gridManager == null)
        {
            Debug.LogError("GridManager not found in the scene!");
            return;
        }

        StartNewWave();
    }

    private void StartNewWave()
    {
        limit = enemiesPerWave; // Update limit based on the current wave count
        StartCoroutine(SpawnEnemyCoroutine());
        currentWave++; // Increment the wave count
        enemiesPerWave++; // Increase the number of enemies for the next wave
        gridManager.ResetGrid();
    }

    IEnumerator SpawnEnemyCoroutine()
    {
        while (limit > 0)
        {
            // Instantiate the enemy
            GameObject newEnemy = Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);

            // The EnemyMovement script will now handle pathfinding in its Start method
            // We don't need to set waypoints manually anymore

            limit--;
            yield return new WaitForSeconds(1f); // Wait for 1 second before spawning the next enemy
        }

        // Once all enemies of the current wave are spawned, wait for them to finish
        StartCoroutine(CheckIfWaveIsComplete());
    }

    IEnumerator CheckIfWaveIsComplete()
    {
        while (true)
        {
            // Check if all enemies are destroyed
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            {
                // Start the next wave
                yield return new WaitForSeconds(2f); // Wait before starting the next wave
                StartNewWave();
                yield break; // Exit the coroutine
            }
            yield return null; // Wait until the next frame to check again
        }
    }
}
