using System.Collections.Generic;

public static class PathFindingUtils
{
    public static List<Node> RetracePath(Node startNode, Node endNode, Dictionary<Node, Node> parentMap)
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