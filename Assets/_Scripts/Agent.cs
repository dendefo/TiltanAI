using UnityEngine;

public enum AgentBehaviorState
{
    Escaping,
    Chasing,
    Exploring,
    Patrolling,
    Reacting
}

[ExecuteInEditMode] // This makes certain functions run in editor mode
[RequireComponent(typeof(AgentMovementController))]
public class Agent : MonoBehaviour
{
    [Header("Agent Settings")]
    [SerializeField] protected AgentStats baseStats;
    [SerializeField] protected string agentName = "Agent";
    public AgentProfile profile;

    [Header("Agent Group Settings")]
    [SerializeField] private AgentGroup agentGroup;
    [SerializeField] private AgentGroupTextures groupTextures;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    private AgentGroup lastGroup; // To track changes

    [Header("Behavior State")]
    public AgentBehaviorState currentState = AgentBehaviorState.Exploring;

    // Metrics
    [HideInInspector] public AgentMetrics metrics = new AgentMetrics();

    private AgentMovementController movementController;

    private void OnEnable()
    {
        // This will run both in edit mode and play mode
        if (skinnedMeshRenderer == null)
        {
            skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }
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
        // Check for group changes in editor
        if (!Application.isPlaying && lastGroup != agentGroup)
        {
            lastGroup = agentGroup;
            UpdateAgentAppearance();
        }

        // Example: update state based on triggers (replace with your behavior graph logic)
        // currentState = ...;

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
            // Handle material assignment differently in edit mode vs play mode
            if (Application.isPlaying)
            {
                skinnedMeshRenderer.material = new Material(groupMaterial);
            }
            else
            {
                // In edit mode, we can directly assign the material
                skinnedMeshRenderer.sharedMaterial = groupMaterial;
            }
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
                movementController.pathAlgorithm = PathAlgorithm.AStar; // pathfinding1
                break;
            case AgentBehaviorState.Exploring:
            case AgentBehaviorState.Patrolling:
                movementController.pathAlgorithm = PathAlgorithm.Dijkstra; // pathfinding2
                break;
            case AgentBehaviorState.Reacting:
                movementController.pathAlgorithm = PathAlgorithm.GreedyBestFirst; // pathfinding3
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

#if UNITY_EDITOR
    private void OnValidate()
    {
        // This is called when values are changed in the inspector
        if (skinnedMeshRenderer == null)
        {
            skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }

        // Ensure update happens on the next editor frame
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this != null) // Check if object still exists
            {
                UpdateAgentAppearance();
            }
        };
    }
#endif
}