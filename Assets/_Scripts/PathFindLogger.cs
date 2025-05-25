using System.Collections.Generic;
using UnityEngine;

public static class PathFindLogger
{
    public static void LogPath(Vector3 startPos, Vector3 targetPos, PathFindingResult pathFindingResult)
    {
        var path = pathFindingResult.Path;
        Debug.Log($"Path found from ({startPos}) to ({targetPos}). Path length: {path.Count} nodes");
        Debug.Log("Nodes Processed: " + pathFindingResult.NodesProcessed);
        // Optional: If you want to log individual nodes
        foreach (Node node in path)
        {
            Debug.Log($"Path node: ({node.worldPosition})");
        }
    }

    public static void LogError(Vector3 startPos, Vector3 targetPos)
    {
        Debug.LogError($"No path found from ({startPos}) to ({targetPos})");
    }
}