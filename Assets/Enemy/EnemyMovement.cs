using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 2f;
    public float rotationSpeed = 5f;
    private int currentWaypointIndex = 0;
    private List<Vector3> worldPath = new List<Vector3>(); // Path in world space

    private GridManager gridManager;

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        if (gridManager == null)
        {
            Debug.LogError("GridManager not found in the scene!");
            return;
        }

        // Initialize the path
        UpdatePath(gridManager.Path);
    }

    void Update()
    {
        MoveAlongPath();
    }

    public void UpdatePath(List<Vector2Int> newPath)
    {
        worldPath.Clear(); // Clear current path

        // Convert the new grid path to world positions
        foreach (Vector2Int gridPos in newPath)
        {
            Vector3 worldPos = gridManager.GridToWorldPosition(gridPos);
            worldPath.Add(worldPos);
        }

        currentWaypointIndex = 0; // Start moving from the beginning of the new path

        if (worldPath.Count == 0)
        {
            Debug.LogError("New path is empty!");
            return;
        }

        Debug.Log("New path updated. Enemy is ready to move.");
    }

    private void MoveAlongPath()
    {
        if (worldPath.Count == 0 || currentWaypointIndex >= worldPath.Count)
            return;

        Vector3 target = worldPath[currentWaypointIndex];
        Vector3 direction = target - transform.position;
        direction.y = 0; // Ensure the enemy doesn't move vertically

        if (direction.magnitude < 0.1f)
        {
            if (currentWaypointIndex == worldPath.Count - 1)
            {
                Destroy(gameObject); // Destroy enemy instance after reaching the end
                Debug.Log("Enemy reached the end and was destroyed.");
            }
            else
            {
                currentWaypointIndex++;
                Debug.Log(
                    "Reached waypoint: " + currentWaypointIndex + " / " + (worldPath.Count - 1)
                );
            }
        }
        else
        {
            // Smooth rotation towards the next waypoint
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            // Move forward
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }
}
