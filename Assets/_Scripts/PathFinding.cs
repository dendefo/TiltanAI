using UnityEngine;
using System.Collections.Generic;
using System;

public class PathFinding : MonoBehaviour
{
    private GridManager grid;
    public Action OnNodeProcessed;
    void Awake()
    {
        grid = GetComponent<GridManager>();
        
    }

    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos, PathAlgorithm algorithm)
    {
        switch (algorithm)
        {
            case PathAlgorithm.BFS:
                return FindPathBFS(startPos, targetPos);
            case PathAlgorithm.Dijkstra:
                return FindPathDijkstra(startPos, targetPos);
            case PathAlgorithm.AStar:
                return FindPathAStar(startPos, targetPos);
            case PathAlgorithm.GreedyBestFirst:
                return FindPathGreedyBestFirst(startPos, targetPos);
            default:
                return new List<Node>();
        }
    }

    public List<Node> FindPathBFS(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.GetNodeFromWorldPoint(startPos);
        Node targetNode = grid.GetNodeFromWorldPoint(targetPos);

        List<Node> path = new List<Node>();
        HashSet<Node> visited = new HashSet<Node>();
        Dictionary<Node, Node> parentMap = new Dictionary<Node, Node>();
        Queue<Node> queue = new Queue<Node>();

        queue.Enqueue(startNode);
        visited.Add(startNode);

        while (queue.Count > 0)
        {
            Node current = queue.Dequeue();

            if (current == targetNode)
            {
                // Path found, reconstruct it
                path = RetracePath(startNode, targetNode, parentMap);
                return path;
            }

            foreach (Node neighbor in grid.GetNeighbors(current))
            {
                if (!visited.Contains(neighbor) && neighbor.walkable)
                {
                    visited.Add(neighbor);
                    parentMap[neighbor] = current;
                    queue.Enqueue(neighbor);
                }
            }
        }

        return path; // Return empty path if no path is found
    }

    public List<Node> FindPathDijkstra(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.GetNodeFromWorldPoint(startPos);
        Node targetNode = grid.GetNodeFromWorldPoint(targetPos);

        var openSet = new SimplePriorityQueue<Node>();
        var cameFrom = new Dictionary<Node, Node>();
        var costSoFar = new Dictionary<Node, float>();

        openSet.Enqueue(startNode, 0);
        costSoFar[startNode] = 0;

        while (openSet.Count > 0)
        {
            Node current = openSet.Dequeue();

            if (current == targetNode)
                return RetracePath(startNode, targetNode, cameFrom);

            foreach (Node neighbor in grid.GetNeighbors(current))
            {
                if (!neighbor.walkable) continue;
                float newCost = costSoFar[current] + neighbor.TotalCost;
                if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
                {
                    costSoFar[neighbor] = newCost;
                    openSet.Enqueue(neighbor, newCost);
                    cameFrom[neighbor] = current;
                }
            }
        }
        return new List<Node>();
    }

    public List<Node> FindPathAStar(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.GetNodeFromWorldPoint(startPos);
        Node targetNode = grid.GetNodeFromWorldPoint(targetPos);

        var openSet = new SimplePriorityQueue<Node>();
        var cameFrom = new Dictionary<Node, Node>();
        var gScore = new Dictionary<Node, float> { [startNode] = 0 };
        var fScore = new Dictionary<Node, float> { [startNode] = Heuristic(startNode, targetNode) };

        openSet.Enqueue(startNode, fScore[startNode]);

        while (openSet.Count > 0)
        {
            Node current = openSet.Dequeue();

            if (current == targetNode)
                return RetracePath(startNode, targetNode, cameFrom);

            foreach (Node neighbor in grid.GetNeighbors(current))
            {
                if (!neighbor.walkable) continue;
                float tentativeG = gScore[current] + neighbor.TotalCost;
                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    fScore[neighbor] = tentativeG + Heuristic(neighbor, targetNode);
                    openSet.Enqueue(neighbor, fScore[neighbor]);
                }
            }
        }
        return new List<Node>();
    }

    public List<Node> FindPathGreedyBestFirst(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.GetNodeFromWorldPoint(startPos);
        Node targetNode = grid.GetNodeFromWorldPoint(targetPos);

        var openSet = new SimplePriorityQueue<Node>();
        var cameFrom = new Dictionary<Node, Node>();
        var visited = new HashSet<Node>();

        openSet.Enqueue(startNode, Heuristic(startNode, targetNode));

        while (openSet.Count > 0)
        {
            Node current = openSet.Dequeue();

            if (current == targetNode)
                return RetracePath(startNode, targetNode, cameFrom);

            visited.Add(current);

            foreach (Node neighbor in grid.GetNeighbors(current))
            {
                if (!neighbor.walkable || visited.Contains(neighbor)) continue;
                cameFrom[neighbor] = current;
                openSet.Enqueue(neighbor, Heuristic(neighbor, targetNode));
            }
        }
        return new List<Node>();
    }

    private List<Node> RetracePath(Node startNode, Node endNode, Dictionary<Node, Node> parentMap)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = parentMap[currentNode];
        }
        path.Add(startNode);
        path.Reverse();
        return path;
    }

    private float Heuristic(Node a, Node b)
    {
        // Manhattan or Euclidean distance depending on movement rules
        return Vector3.Distance(a.worldPosition, b.worldPosition);
    }
}

public enum PathAlgorithm
{
    BFS,
    Dijkstra,
    AStar,
    GreedyBestFirst
}