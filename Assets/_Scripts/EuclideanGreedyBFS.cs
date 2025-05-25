using System.Collections.Generic;
using UnityEngine;

public class EuclideanGreedyBFS : GreedyBestFirstSearch
{
    protected override float CalculateHeuristic(Node node, Node targetNode)
    {
        float dx = node.gridX - targetNode.gridX;
        float dy = node.gridY - targetNode.gridY;
        return Mathf.Sqrt(dx * dx + dy * dy);
    }
}