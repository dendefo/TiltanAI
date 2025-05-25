using UnityEngine;

public class DiagonalGreedyBFS : GreedyBestFirstSearch
{
    protected override float CalculateHeuristic(Node node, Node targetNode)
    {
        float dx = Mathf.Abs(node.gridX - targetNode.gridX);
        float dy = Mathf.Abs(node.gridY - targetNode.gridY);
        float D = 1f; // Cost of moving straight
        float D2 = 1.414f; // Cost of moving diagonally (√2)
        return D * (dx + dy) + (D2 - 2 * D) * Mathf.Min(dx, dy);
    }
}