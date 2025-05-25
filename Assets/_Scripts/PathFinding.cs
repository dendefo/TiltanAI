using System;
using UnityEngine;
using System.Collections.Generic;

public class PathFinding
{
    private readonly GridManager grid;
    private readonly IPathFindingStrategy bfsStrategy;
    private readonly IPathFindingStrategy dfsStrategy;

    public PathFinding(GridManager gridManager)
    {
        grid = gridManager ?? throw new ArgumentNullException(nameof(gridManager));
        
        // Initialize the strategies
        bfsStrategy = new BFSPathFinding();
        dfsStrategy = new DFSPathFinding();
    }

    public PathFindingResult FindPath(Vector3 startPos, Vector3 targetPos, PathFindingStrategy strategy)
    {
        // Select the appropriate strategy
        IPathFindingStrategy pathFinder = strategy switch
        {
            PathFindingStrategy.BFS => bfsStrategy,
            PathFindingStrategy.DFS => dfsStrategy,
            _ => throw new ArgumentException($"Unsupported pathfinding strategy: {strategy}")
        };

        // Use the selected strategy to find the path
        return pathFinder.FindPath(grid, startPos, targetPos);
    }
}
