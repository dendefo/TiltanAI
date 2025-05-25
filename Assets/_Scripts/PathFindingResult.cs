using System.Collections.Generic;

public struct PathFindingResult
{
    public List<Node> Path { get; private set; }
    public int NodesProcessed { get; private set; }

   
    public PathFindingResult(List<Node> path, int nodesProcessed)
    {
        Path = path;
        NodesProcessed = nodesProcessed;
    }
}