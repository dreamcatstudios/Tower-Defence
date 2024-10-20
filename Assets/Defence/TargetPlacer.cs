using System.Collections.Generic;
using UnityEngine;

public class TargetPlacer : MonoBehaviour
{
    [SerializeField]
    private GameObject ballistaPrefab;

    [SerializeField]
    private bool isPlaceable = true;

    private TargetInstancer targetInstancer;
    private GameObject targetObject;
    private List<Vector3> placeableLocations = new List<Vector3>();
    private EnemyPath enemyPath;
    private GridManager gridManager;

    private GameObject enemy;

    private void Start()
    {
        targetObject = GameObject.Find("Defences");
        targetInstancer = targetObject.GetComponent<TargetInstancer>();
        enemyPath = GameObject.Find("Path").GetComponent<EnemyPath>();
        gridManager = FindObjectOfType<GridManager>();

        // Populate the list of placeable locations
        if (enemyPath != null && enemyPath.waypoints != null)
        {
            foreach (Vector3 item in enemyPath.waypoints)
            {
                placeableLocations.Add(item);
            }
        }
        else
        {
            Debug.LogError("EnemyPath or waypoints not found!");
        }
        FindEnemy();
    }



    private void OnMouseOver()
    {
        FindEnemy();
        if (isPlaceable && Input.GetMouseButtonDown(0))
        {
            // Check if instantiation is allowed
            if (
                targetInstancer != null
                && targetInstancer.CanInstanceTarget()
                && !placeableLocations.Contains(transform.position)
            )
            {
                // Instantiate ballista
                GameObject newBallista = Instantiate(
                    ballistaPrefab,
                    transform.position + new Vector3(0, 6, 0),
                    Quaternion.identity,
                    targetObject.transform
                );

                // Mark tile as unwalkable
                gridManager.MarkTileAsUnwalkable(transform.position);

                //Debug

                Debug.Log($"Enemy: {enemy}");

                // Recalculate path for enemies
                gridManager.FindPathBFS(
                    gridManager.WorldToGridPosition(enemy.transform.position),
                    gridManager.WorldToGridPosition(gridManager.target.position)
                );

                // Update enemies with the new path
                foreach (var enemy in FindObjectsOfType<EnemyMovement>())
                {
                    enemy.UpdatePath(gridManager.Path);
                }

                isPlaceable = false;
                targetInstancer.DeductCost();
            }
        }
    }

    void FindEnemy()
    {
        enemy = GameObject.Find("Ram(Clone)");
    }
}
