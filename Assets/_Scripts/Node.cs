using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;
    public float cost; // Base movement cost (default 1)
    public float dynamicCost; // For dynamic obstacles

    public Node(bool walkable, Vector3 worldPos, int x, int y, float cost = 1f)
    {
        this.walkable = walkable;
        this.worldPosition = worldPos;
        this.gridX = x;
        this.gridY = y;
        this.cost = cost;
        this.dynamicCost = 0f;
    }

    public float TotalCost => cost + dynamicCost;
}