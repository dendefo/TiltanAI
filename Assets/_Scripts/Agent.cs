using UnityEngine;

[ExecuteInEditMode] // This makes certain functions run in editor mode
public class Agent : MonoBehaviour
{
    [Header("Agent Settings")]
    [SerializeField] protected AgentStats baseStats;
    [SerializeField] protected string agentName = "Agent";
    
    [Header("Agent Group Settings")]
    [SerializeField] private AgentGroup agentGroup;
    [SerializeField] private AgentGroupTextures groupTextures;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    private AgentGroup lastGroup; // To track changes

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

    private void Update()
    {
        // Check for group changes in editor
        if (!Application.isPlaying && lastGroup != agentGroup)
        {
            lastGroup = agentGroup;
            UpdateAgentAppearance();
        }
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