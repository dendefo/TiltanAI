using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomDijkstraPathFinding : DijkstraPathFinding
{
    private bool allowDiagonal;

    public CustomDijkstraPathFinding(bool allowDiagonalMovement = true)
    {
        allowDiagonal = allowDiagonalMovement;
    }

    public new PathFindingResult FindPath(GridManager grid, Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.GetNodeFromWorldPoint(startPos);
        Node targetNode = grid.GetNodeFromWorldPoint(targetPos);
        int nodesProcessed = 0;

        // Initialize data structures
        List<Node> path = new List<Node>();
        Dictionary<Node, float> distances = new Dictionary<Node, float>();
        Dictionary<Node, Node> parentMap = new Dictionary<Node, Node>();
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        // Set initial distances to infinity except start node
        foreach (Node node in grid.GetAllNodes())
        {
            distances[node] = float.MaxValue;
        }
        
        distances[startNode] = 0;
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            openSet.Sort((a, b) => distances[a].CompareTo(distances[b]));
            Node current = openSet[0];
            openSet.RemoveAt(0);
            
            nodesProcessed++;

            if (current == targetNode)
            {
                path = PathFindingUtils.RetracePath(startNode, targetNode, parentMap);
                return new PathFindingResult(path, nodesProcessed);
            }

            closedSet.Add(current);

            foreach (Node neighbor in GetValidNeighbors(grid, current))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                float distance = distances[current] + CalculateDistance(current, neighbor);

                if (distance < distances[neighbor])
                {
                    parentMap[neighbor] = current;
                    distances[neighbor] = distance;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return new PathFindingResult(new List<Node>(), nodesProcessed);
    }

    private IEnumerable<Node> GetValidNeighbors(GridManager grid, Node current)
    {
        var neighbors = grid.GetNeighbors(current);
        
        if (!allowDiagonal)
        {
            return neighbors.Where(neighbor => 
                (Mathf.Abs(neighbor.gridX - current.gridX) + Mathf.Abs(neighbor.gridY - current.gridY)) == 1);
        }

        return neighbors;
    }

    private new float CalculateDistance(Node nodeA, Node nodeB)
    {
        float dx = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        float dy = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (allowDiagonal)
        {
            // If diagonal movement is allowed, use diagonal distance
            if (dx == 1 && dy == 1)
            {
                return 1.4142f; // √2 for diagonal movement
            }
        }

        // For orthogonal movement or when diagonals are disabled
        return dx + dy;
    }
}