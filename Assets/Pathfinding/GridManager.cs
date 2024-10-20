using System.Collections.Generic;
using TMPro; // Import TextMeshPro namespace
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float tileSize = 1f;
    public Transform target;
    public Transform startTile;
    public float moveSpeed = 5f;
    public LayerMask obstacleLayer;

    private bool[,] grid; // Simple 2D array representing the grid
    private List<Vector2Int> path; // Path of grid positions
    private Vector2Int targetTile;
    private Vector2Int startTileGridPosition; // Store the grid position for startTile
    private Vector3 currentTargetPosition;

    public TextMeshProUGUI pathLabel; // Reference to the TextMeshPro UI for displaying the path

    public List<Vector2Int> Path
    {
        get { return path; }
    }

    private GameObject tileParent;

    void Start()
    {
        tileParent = GameObject.Find("Tiles");

        // Find the "Path Label" game object and get its TextMeshPro component
        GameObject pathLabelObject = GameObject.Find("Path Label");
        if (pathLabelObject != null)
        {
            pathLabel = pathLabelObject.GetComponent<TextMeshProUGUI>();
        }

        InitializeGrid();

        // Set the starting position based on the startTile Transform
        startTileGridPosition = WorldToGridPosition(startTile.position);
        targetTile = WorldToGridPosition(target.position);

        // Find the path using BFS
        FindPathBFS(startTileGridPosition, targetTile);
        UpdatePathLabel(); // Update the path label with the current path
    }

    public void ResetGrid()
    {
        // Set the starting position based on the startTile Transform
        startTileGridPosition = WorldToGridPosition(startTile.position);
        targetTile = WorldToGridPosition(target.position);

        // Find the path using BFS
        FindPathBFS(startTileGridPosition, targetTile);
        UpdatePathLabel(); // Update the path label with the new path
    }

    void Update()
    {
        // Move along the path
        if (path != null && path.Count > 0)
        {
            currentTargetPosition = GridToWorldPosition(path[0]);
            transform.position = Vector3.MoveTowards(
                transform.position,
                currentTargetPosition,
                moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, currentTargetPosition) < 0.1f)
            {
                path.RemoveAt(0); // Move to the next point
                UpdatePathLabel(); // Update the path label when path changes
            }
        }

        // Update tile colors
        UpdateTileColors();
    }

    // Initialize the grid and mark walkable/unwalkable tiles
    void InitializeGrid()
    {
        grid = new bool[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                Vector3 worldPos = GridToWorldPosition(new Vector2Int(x, z));
                bool isWalkable = !Physics.CheckSphere(worldPos, tileSize / 2, obstacleLayer); // Check for obstacles
                grid[x, z] = isWalkable;
            }
        }
    }

    // Simple BFS implementation to find the shortest path
    public void FindPathBFS(Vector2Int start, Vector2Int target)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == target)
            {
                RetracePath(cameFrom, target);
                UpdatePathLabel(); // Update the path label when a path is found
                return;
            }

            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                if (!visited.Contains(neighbor) && grid[neighbor.x, neighbor.y])
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                    cameFrom[neighbor] = current; // Store where we came from
                }
            }
        }

        Debug.LogWarning("No path found!");
    }

    // Retrace the path from target to start and log it
    void RetracePath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int target)
    {
        path = new List<Vector2Int>();
        Vector2Int current = target;

        while (cameFrom.ContainsKey(current))
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Reverse(); // Reverse the path so we can follow it from start to end
    }

    // Update the "Path Label" text to display the current path
    void UpdatePathLabel()
    {
        if (pathLabel != null && path != null)
        {
            string pathText = "";
            for (int i = 0; i < path.Count; i++)
            {
                // Assuming 'path' is a List<Vector3> or similar with world positions
                int roundedX = Mathf.RoundToInt(path[i].x); // Taking x from the world position
                int roundedZ = Mathf.RoundToInt(path[i].y); // Taking z from the world position

                // Formatting to show the x and z positions
                pathText += $"({roundedX},{roundedZ})";

                if (i < path.Count - 1)
                {
                    pathText += " -> ";
                }
            }
            pathLabel.text = pathText;
        }
    }

    // Get valid neighbors of a tile (up, down, left, right)
    List<Vector2Int> GetNeighbors(Vector2Int tile)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        if (tile.x > 0)
            neighbors.Add(new Vector2Int(tile.x - 1, tile.y)); // left
        if (tile.x < gridWidth - 1)
            neighbors.Add(new Vector2Int(tile.x + 1, tile.y)); // right
        if (tile.y > 0)
            neighbors.Add(new Vector2Int(tile.x, tile.y - 1)); // down
        if (tile.y < gridHeight - 1)
            neighbors.Add(new Vector2Int(tile.x, tile.y + 1)); // up

        return neighbors;
    }

    // Mark tile as unwalkable
    public void MarkTileAsUnwalkable(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGridPosition(worldPos);
        grid[gridPos.x, gridPos.y] = false;
    }

    // Convert grid position to world position
    public Vector3 GridToWorldPosition(Vector2Int gridPos)
    {
        float halfWidth = gridWidth / 2f;
        float halfHeight = gridHeight / 2f;

        // Center the grid based on grid size
        return new Vector3(
            (gridPos.x - halfWidth) * tileSize,
            0,
            (gridPos.y - halfHeight) * tileSize
        );
    }

    // Convert world position to grid position
    public Vector2Int WorldToGridPosition(Vector3 worldPos)
    {
        float halfWidth = gridWidth / 2f;
        float halfHeight = gridHeight / 2f;

        int x = Mathf.RoundToInt(worldPos.x / tileSize + halfWidth);
        int z = Mathf.RoundToInt(worldPos.z / tileSize + halfHeight);
        return new Vector2Int(x, z);
    }

    // Update tile colors and the path label
    void UpdateTileColors()
    {
        Dictionary<Vector2Int, string> gridPosToTileName = new Dictionary<Vector2Int, string>();

        // First, create a mapping of grid positions to tile names
        foreach (Transform tile in tileParent.transform)
        {
            Vector2Int gridPos = WorldToGridPosition(tile.position);
            gridPosToTileName[gridPos] = tile.name;

            Renderer tileRenderer = tile.GetComponent<Renderer>();
            if (path.Contains(gridPos))
            {
                tileRenderer.material.color = Color.red; // Tile is in the path
            }
            else
            {
                tileRenderer.material.color = Color.green; // Tile is not in the path
            }
        }

        // Now, create the list of tile names in the correct order
        List<string> pathTileNames = new List<string>();
        foreach (Vector2Int pos in path)
        {
            if (gridPosToTileName.TryGetValue(pos, out string tileName))
            {
                pathTileNames.Add(tileName);
            }
        }

        UpdatePathLabel(pathTileNames);
    }

    // Update the "Path Label" text to display the names of tiles in the path
    void UpdatePathLabel(List<string> pathTileNames)
    {
        if (pathLabel != null)
        {
            string pathText = "Node Path => " + string.Join(" -> ", pathTileNames);
            pathLabel.text = pathText;
        }
    }
}
