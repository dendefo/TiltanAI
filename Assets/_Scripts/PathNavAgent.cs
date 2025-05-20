
using UnityEngine;
using System.Collections.Generic;


public class PathNavAgent : MonoBehaviour
{
    private PathFinding pathFinder;
    public Vector3 someTargetPosition; // Set this to your desired target position

    void Start()
    {
        // Get reference to the PathFinding component
        pathFinder = FindObjectOfType<PathFinding>();
    }

    void FindPath()
    {
        Vector3 startPosition = transform.position; // Starting position
        Vector3 targetPosition = someTargetPosition; // Your target position

        List<Node> path = pathFinder.FindPathBFS(startPosition, targetPosition);

        if (path.Count > 0)
        {
            // Path found, do something with it
            foreach (Node node in path)
            {
                // Use the path nodes
                Debug.Log($"Path node position: {node.worldPosition}");
            }
        }
        else
        {
            Debug.Log("No path found!");
        }
    }
}
