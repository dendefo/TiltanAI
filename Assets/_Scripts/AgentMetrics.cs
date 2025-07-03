using System;
using UnityEngine;

[Serializable]
public class AgentMetrics
{
    public float lastPathTime;
    public int lastNodesProcessed;
    public float lastPathDistance;
    public PathAlgorithm lastAlgorithm;
    public int rePathCount;
    public float lastRePathTimestamp;
}