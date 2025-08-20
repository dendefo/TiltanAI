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

    [Header("Arrival")]
    [Tooltip("How close the agent must get to the goal to consider it reached.")]
    public float stoppingDistance = 0.2f;

    private PathFinding pathFinder;
    private Agent agent;
    private List<Node> currentPath;
    private int pathIndex;
    private Vector3 velocity;
    private float lastRepathTime;
    private Vector3 lastDirection;

    // Goal tracking when navigating to a raw position (no Transform target)
    private Vector3? goalPosition;

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
        if (autoRepath && target != null && Time.time - lastRepathTime > repathInterval)
        {
            RequestPath();
        }

        MoveAlongPath();
    }

    public void RequestPath()
    {
        if (target == null) return;

        goalPosition = target.position; // ensure final approach to actual target
        var metrics = agent != null ? agent.metrics : null;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        int nodesProcessed = 0;
        pathFinder.OnNodeProcessed = () => nodesProcessed++;

        var path = pathFinder.FindPath(transform.position, target.position, pathAlgorithm);

        stopwatch.Stop();

        currentPath = path;
        pathIndex = 0;
        lastRepathTime = Time.time;
        lastDirection = transform.forward;

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
        if (path == null || path.Count < 2) return 0f;
        float dist = 0f;
        for (int i = 1; i < path.Count; i++)
            dist += Vector3.Distance(path[i - 1].worldPosition, path[i].worldPosition);
        return dist;
    }

    void MoveAlongPath()
    {
        // If we finished the node path, continue direct approach to the goal using stoppingDistance.
        if (currentPath == null || pathIndex >= currentPath.Count)
        {
            MoveDirectlyToGoal();
            return;
        }

        Vector3 targetPos = currentPath[pathIndex].worldPosition;
        Vector3 toTarget = targetPos - transform.position;
        Vector3 desiredDirection = toTarget.normalized;

        float angle = Vector3.Angle(lastDirection, desiredDirection);
        float turnModifier = 1f + (angle / 180f) * (turnCostMultiplier - 1f);

        velocity = Vector3.MoveTowards(
            velocity,
            desiredDirection * maxSpeed / turnModifier,
            acceleration * Time.deltaTime
        );

        transform.position += velocity * Time.deltaTime;

        if (velocity != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }

        if (toTarget.magnitude <= stoppingDistance)
        {
            pathIndex++;
            lastDirection = desiredDirection;

            // If we just consumed the last node, start final approach immediately this frame.
            if (pathIndex >= currentPath.Count)
            {
                MoveDirectlyToGoal();
            }
        }
    }

    private void MoveDirectlyToGoal()
    {
        if (!goalPosition.HasValue)
            return;

        Vector3 goal = goalPosition.Value;
        Vector3 toGoal = goal - transform.position;
        float dist = toGoal.magnitude;

        if (dist <= stoppingDistance)
        {
            StopMoving();
            return;
        }

        Vector3 desiredDirection = toGoal.normalized;

        float angle = Vector3.Angle(lastDirection, desiredDirection);
        float turnModifier = 1f + (angle / 180f) * (turnCostMultiplier - 1f);

        velocity = Vector3.MoveTowards(
            velocity,
            desiredDirection * maxSpeed / turnModifier,
            acceleration * Time.deltaTime
        );

        // Clamp to avoid overshoot
        Vector3 step = velocity * Time.deltaTime;
        if (step.magnitude > dist)
        {
            step = desiredDirection * dist;
        }

        transform.position += step;

        if (step != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(step);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }

        lastDirection = desiredDirection;
    }

    public void OnPathInvalidated()
    {
        RequestPath();
    }

    public void RequestPathToPosition(Vector3 position)
    {
        // One-time target position (no Transform target)
        target = null;
        goalPosition = position;

        var metrics = agent != null ? agent.metrics : null;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        int nodesProcessed = 0;
        pathFinder.OnNodeProcessed = () => nodesProcessed++;

        var path = pathFinder.FindPath(transform.position, position, pathAlgorithm);

        stopwatch.Stop();

        if (path != null && path.Count > 0)
        {
            currentPath = path;
            pathIndex = 0;
            lastRepathTime = Time.time;
            lastDirection = transform.forward;

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
        else
        {
            // No valid path found; allow direct approach
            currentPath = null;
            pathIndex = 0;
            lastDirection = transform.forward;
        }
    }

    public void StopMoving()
    {
        // Stop movement and clear path and goal
        currentPath = null;
        pathIndex = 0;
        velocity = Vector3.zero;
        goalPosition = null;
    }
}