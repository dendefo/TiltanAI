using UnityEngine;

public class ChebyshevDijkstraPathFinding : DijkstraPathFinding
{
    private new float CalculateDistance(Node nodeA, Node nodeB)
    {
        // Calculate Chebyshev distance between nodes
        float dx = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        float dy = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        
        // Chebyshev distance is the maximum of the horizontal and vertical distances
        return Mathf.Max(dx, dy);
    }
}