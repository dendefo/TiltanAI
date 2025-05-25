﻿using UnityEngine;

public interface IPathFindingStrategy
{
    PathFindingResult FindPath(GridManager grid, Vector3 startPos, Vector3 targetPos);
}