using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPath : MonoBehaviour
{
    [SerializeField]
    public List<Vector3> waypoints = new List<Vector3>(); // Path in world space

    [SerializeField]
    public List<Vector3> stopWaypoints = new List<Vector3>(); // Waypoints for the enemies to follow

    private GridManager gridManager;

    void Start()
    {
        // Convert grid-based path to world space positions
        foreach (Vector2Int gridPosition in gridManager.Path)
        {
            Vector3 worldPos = gridManager.GridToWorldPosition(gridPosition); // Convert grid to world
            waypoints.Add(worldPos);
        }

        if (waypoints.Count == 0)
        {
            Debug.LogError("Path not found for enemy movement!");
            return;
        }
    }
}
