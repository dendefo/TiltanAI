using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;
    public float cost;
    public float dynamicCost;

    public Node(bool walkable, Vector3 worldPos, int x, int y, float cost = 1f)
    {
        this.walkable = walkable;
        worldPosition = worldPos;
        gridX = x;
        gridY = y;
        this.cost = cost;
        dynamicCost = 0f;
    }

    public float TotalCost => cost + dynamicCost;
}