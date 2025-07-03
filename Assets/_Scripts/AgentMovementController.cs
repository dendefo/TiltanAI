using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(PathFinding))]
public class AgentMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxSpeed = 5f;
    public float acceleration = 10f;
    public float turnSpeed = 180f; // Degrees per second
    public float turnCostMultiplier = 1.2f; // Cost multiplier for sharp turns

    [Header("Pathfinding")]
    public PathAlgorithm pathAlgorithm = PathAlgorithm.AStar;
    public Transform target;
    public bool autoRepath = true;
    public float repathInterval = 1f;

    private PathFinding pathFinder;
    private Agent agent;
    private List<Node> currentPath;
    private int pathIndex;
    private Vector3 velocity;
    private float lastRepathTime;
    private Vector3 lastDirection;


    void Awake()
    {
        pathFinder = GetComponent<PathFinding>();
        agent = GetComponent<Agent>();
    }

    void Start()
    {
        RequestPath();
    }

    void Update()
    {
        if (autoRepath && Time.time - lastRepathTime > repathInterval)
        {
            RequestPath();
        }

        MoveAlongPath();
    }

    public void RequestPath()
    {
        if (target == null) return;
        var metrics = agent != null ? agent.metrics : null;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        int nodesProcessed = 0;
        pathFinder.OnNodeProcessed = () => nodesProcessed++; // Add this delegate to PathFinding

        var path = pathFinder.FindPath(transform.position, target.position, pathAlgorithm);

        stopwatch.Stop();

        currentPath = path;
        pathIndex = 0;
        lastRepathTime = Time.time;
        lastDirection = transform.forward;

        // Metrics
        if (metrics != null)
        {
            metrics.lastPathTime = (float)stopwatch.Elapsed.TotalMilliseconds;
            metrics.lastNodesProcessed = nodesProcessed;
            metrics.lastAlgorithm = pathAlgorithm;
            metrics.lastPathDistance = CalculatePathDistance(path);
            metrics.rePathCount++;
            metrics.lastRePathTimestamp = Time.time;
        }
    }

    private float CalculatePathDistance(List<Node> path)
    {
        float dist = 0f;
        for (int i = 1; i < path.Count; i++)
            dist += Vector3.Distance(path[i - 1].worldPosition, path[i].worldPosition);
        return dist;
    }

    void MoveAlongPath()
    {
        if (currentPath == null || pathIndex >= currentPath.Count)
            return;

        Vector3 targetPos = currentPath[pathIndex].worldPosition;
        Vector3 toTarget = (targetPos - transform.position);
        Vector3 desiredDirection = toTarget.normalized;

        // Turning cost: if direction changes sharply, apply a cost/slowdown
        float angle = Vector3.Angle(lastDirection, desiredDirection);
        float turnModifier = 1f + (angle / 180f) * (turnCostMultiplier - 1f);

        // Accelerate towards desired direction
        velocity = Vector3.MoveTowards(velocity, desiredDirection * maxSpeed / turnModifier, acceleration * Time.deltaTime);

        // Move agent
        transform.position += velocity * Time.deltaTime;

        // Rotate agent smoothly
        if (velocity != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }

        // Check if reached current node
        if (toTarget.magnitude < 0.2f)
        {
            pathIndex++;
            lastDirection = desiredDirection;
        }
    }

    // For external triggers (e.g., dynamic obstacles)
    public void OnPathInvalidated()
    {
        RequestPath();
    }
}