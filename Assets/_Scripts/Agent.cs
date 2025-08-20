using System;
using Unity.Behavior;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(AgentMovementController))]
public class Agent : MonoBehaviour
{
    [Header("Agent Settings")]
    [SerializeField]
    protected AgentStats baseStats;

    [SerializeField]
    protected string agentName = "Agent";

    public AgentProfile profile;

    [Header("Agent Group Settings")]
    [SerializeField]
    private AgentGroup agentGroup;

    [SerializeField]
    private AgentGroupTextures groupTextures;

    [SerializeField]
    private SkinnedMeshRenderer skinnedMeshRenderer;

    private AgentGroup lastGroup;

    [Header("Behavior State")]
    public AgentBehaviorState currentState = AgentBehaviorState.Exploring;

    [HideInInspector]
    public AgentMetrics metrics = new AgentMetrics();

    private AgentMovementController movementController;
    private Vector3? currentDestination = null;
    private float stoppingDistance = 0.2f;

    private void OnEnable()
    {
        if (skinnedMeshRenderer == null)
            skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        lastGroup = agentGroup;
        UpdateAgentAppearance();
    }

    private void Start()
    {
        movementController = GetComponent<AgentMovementController>();
        if (profile != null && movementController != null)
        {
            movementController.maxSpeed = profile.maxSpeed;
            movementController.acceleration = profile.acceleration;
            movementController.turnSpeed = profile.turnSpeed;
            movementController.turnCostMultiplier = profile.turnCostMultiplier;
            movementController.pathAlgorithm = profile.pathAlgorithm;
        }
        UpdatePathfindingAlgorithm();
    }

    private void Update()
    {
        if (!Application.isPlaying && lastGroup != agentGroup)
        {
            lastGroup = agentGroup;
            UpdateAgentAppearance();
        }

        // Example state transitions or triggers
        UpdatePathfindingAlgorithm();
    }

    private void UpdateAgentAppearance()
    {
        if (skinnedMeshRenderer == null)
        {
            Debug.LogError($"SkinnedMeshRenderer is null on {gameObject.name}!", this);
            return;
        }

        if (groupTextures == null)
        {
            Debug.LogError($"GroupTextures is null on {gameObject.name}!", this);
            return;
        }

        Material groupMaterial = groupTextures.GetMaterialForGroup(agentGroup);
        if (groupMaterial != null)
        {
            if (Application.isPlaying)
                skinnedMeshRenderer.material = new Material(groupMaterial);
            else
                skinnedMeshRenderer.sharedMaterial = groupMaterial;
        }
        else
        {
            Debug.LogWarning($"No material found for group {agentGroup} on {gameObject.name}", this);
        }
    }

    private void UpdatePathfindingAlgorithm()
    {
        if (movementController == null) return;

        switch (currentState)
        {
            case AgentBehaviorState.Escaping:
            case AgentBehaviorState.Chasing:
                movementController.pathAlgorithm = PathAlgorithm.AStar;
                break;
            case AgentBehaviorState.Exploring:
            case AgentBehaviorState.Patrolling:
                movementController.pathAlgorithm = PathAlgorithm.Dijkstra;
                break;
            case AgentBehaviorState.Reacting:
                movementController.pathAlgorithm = PathAlgorithm.GreedyBestFirst;
                break;
        }
    }

    public AgentGroup Group
    {
        get => agentGroup;
        set
        {
            if (agentGroup != value)
            {
                agentGroup = value;
                lastGroup = value;
                UpdateAgentAppearance();
            }
        }
    }

    public BlackboardVariable<float> Speed { get; internal set; }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (skinnedMeshRenderer == null)
            skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this != null)
                UpdateAgentAppearance();
        };
    }
#endif
    internal void SetDestination(Vector3 destination, float stoppingDistance)
    {
        if (movementController == null)
            movementController = GetComponent<AgentMovementController>();

        this.currentDestination = destination;
        this.stoppingDistance = stoppingDistance;

        if (movementController != null)
        {
            movementController.target = null; // Clear any Transform target
            movementController.stoppingDistance = stoppingDistance; // propagate threshold
            movementController.enabled = true;
            movementController.RequestPathToPosition(destination);
        }
    }

    internal bool IsNavigationComplete()
    {
        if (movementController == null)
            movementController = GetComponent<AgentMovementController>();

        if (!currentDestination.HasValue)
            return true;

        float distance = Vector3.Distance(transform.position, currentDestination.Value);
        if (distance <= stoppingDistance)
            return true;

        return false;
    }

    internal void Stop()
    {
        if (movementController == null)
            movementController = GetComponent<AgentMovementController>();

        currentDestination = null;
        if (movementController != null)
        {
            movementController.target = null;
            movementController.StopMoving(); // also clears goal in controller
        }
    }
}

public enum AgentBehaviorState
{
    Escaping,
    Chasing,
    Exploring,
    Patrolling,
    Reacting
}