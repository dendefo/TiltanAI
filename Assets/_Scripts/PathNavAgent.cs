using UnityEngine;
using System.Collections.Generic;

public class PathNavAgent : MonoBehaviour
{
    private PathFinding pathFinder;
    private PathFindingResult currentPathResult;
    
    [Tooltip("Current target position for pathfinding")]
    public Vector3 someTargetPosition;
    
    [Tooltip("Choose between BFS or DFS pathfinding strategy")]
    [SerializeField] private PathFindingStrategy pathFindingStrategy = PathFindingStrategy.BFS;
    [Tooltip("Layer mask to filter grid objects for pathfinding")]
    [SerializeField] private LayerMask gridLayerMask;

    // Add these new variables for path visualization
    [Header("Path Visualization")]
    [Tooltip("Color of the path visualization")]
    [SerializeField] private Color pathColor = Color.yellow; // Customizable path color
    [Tooltip("Width of the path lines and node markers")]
    [SerializeField] private float lineWidth = 0.2f; // Customizable line width
    
    [Tooltip("Toggle path visualization on/off")]
    [SerializeField] private bool showPathGizmos = true;


    void Start()
    {
        pathFinder = FindObjectOfType<PathFinding>();
    }

    
    void Update()
    {
        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        // Check for left mouse button click
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // If we hit something on the grid layer
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, gridLayerMask))
            {
                // Update target position and find path
                someTargetPosition = hit.point;
                FindPath();
            }
        }
    }

    public void FindPath()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = someTargetPosition;

        currentPathResult = pathFinder.FindPath(startPosition, targetPosition, pathFindingStrategy);

        if (currentPathResult.Path.Count > 0)
        {
            PathFindLogger.LogPath(startPosition, targetPosition, currentPathResult);
        }
        else
        {
            PathFindLogger.LogError(startPosition, targetPosition);
        }
    }

    private void OnDrawGizmos()
    {
        if (!showPathGizmos || pathFinder == null || currentPathResult.Path == null) return;
        
        var currentPath = currentPathResult.Path;
        if (currentPath == null || currentPath.Count == 0) return;

        // Set the color for the path
        Gizmos.color = pathColor;

        // Draw lines between each node in the path
        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Vector3 startPos = currentPath[i].worldPosition;
            Vector3 endPos = currentPath[i + 1].worldPosition;
            
            // Draw line between nodes
            Gizmos.DrawLine(startPos, endPos);
            
            // Draw sphere at each node position
            Gizmos.DrawSphere(startPos, lineWidth * 0.5f);
        }
        
        // Draw sphere at the last node
        if (currentPath.Count > 0)
        {
            Gizmos.DrawSphere(currentPath[currentPath.Count - 1].worldPosition, lineWidth * 0.5f);
        }
    }

    private void OnGUI()
    {
        if (currentPathResult.Path == null || currentPathResult.Path.Count == 0)
        {
            GUI.Label(new Rect(10, 10, 300, 60), "No path found.");
            return;
        }
        GUI.Label(new Rect(10, 10, 300, 60), 
            $"Path length: {currentPathResult.Path.Count}\n" +
            $"Nodes processed: {currentPathResult.NodesProcessed}\n" +
            $"Strategy: {pathFindingStrategy}");
    }

    public void SetPathFindingStrategy(PathFindingStrategy newStrategy)
    {
        pathFindingStrategy = newStrategy;
    }
}