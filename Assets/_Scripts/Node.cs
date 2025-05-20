using UnityEngine;

public class Node
{
    public bool walkable;         // Can the AI walk here?
    public Vector3 worldPosition; // Actual position in the world
    public int gridX;             // Grid index X
    public int gridY;             // Grid index Y

    public Node(bool walkable, Vector3 worldPos, int x, int y)
    {
        this.walkable = walkable;
        this.worldPosition = worldPos;
        this.gridX = x;
        this.gridY = y;
    }
}