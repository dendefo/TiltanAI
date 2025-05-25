using UnityEngine;
using System.Collections.Generic;


public class PathFinding : MonoBehaviour
{
    private GridManager grid;

    void Awake()
    {
        grid = GetComponent<GridManager>();
    }

    public PathFindingResult FindPath(Vector3 startPos, Vector3 targetPos, PathFindingStrategy strategy)
    {
        switch (strategy)
        {
            case PathFindingStrategy.BFS:
                return FindPathBFS(startPos, targetPos);
            case PathFindingStrategy.DFS:
                return FindPathDFS(startPos, targetPos);
            default:
                Debug.LogError($"Pathfinding strategy {strategy} not implemented!");
                return new PathFindingResult(new List<Node>(), 0);
        }
    }

    private PathFindingResult FindPathDFS(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.GetNodeFromWorldPoint(startPos);
        Node targetNode = grid.GetNodeFromWorldPoint(targetPos);
        int nodesProcessed = 0;

        List<Node> path = new List<Node>();
        HashSet<Node> visited = new HashSet<Node>();
        Dictionary<Node, Node> parentMap = new Dictionary<Node, Node>();
        Stack<Node> stack = new Stack<Node>();

        stack.Push(startNode);
        visited.Add(startNode);

        while (stack.Count > 0)
        {
            Node current = stack.Pop();
            nodesProcessed++;

            if (current == targetNode)
            {
                path = RetracePath(startNode, targetNode, parentMap);
                return new PathFindingResult(path, nodesProcessed);
            }

            foreach (Node neighbor in grid.GetNeighbors(current))
            {
                if (!visited.Contains(neighbor) && neighbor.walkable)
                {
                    visited.Add(neighbor);
                    parentMap[neighbor] = current;
                    stack.Push(neighbor);
                }
            }
        }
        return new PathFindingResult(path, nodesProcessed);
    }

    private PathFindingResult FindPathBFS(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.GetNodeFromWorldPoint(startPos);
        Node targetNode = grid.GetNodeFromWorldPoint(targetPos);
        int nodesProcessed = 0;

        List<Node> path = new List<Node>();
        HashSet<Node> visited = new HashSet<Node>();
        Dictionary<Node, Node> parentMap = new Dictionary<Node, Node>();
        Queue<Node> queue = new Queue<Node>();

        queue.Enqueue(startNode);
        visited.Add(startNode);

        while (queue.Count > 0)
        {
            Node current = queue.Dequeue();
            nodesProcessed++;

            if (current == targetNode)
            {
                path = RetracePath(startNode, targetNode, parentMap);
                return new PathFindingResult(path, nodesProcessed);
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
        return new PathFindingResult(path, nodesProcessed);
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
}