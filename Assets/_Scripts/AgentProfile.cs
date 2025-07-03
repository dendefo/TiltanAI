using UnityEngine;

[CreateAssetMenu(menuName = "Agents/AgentProfile")]
public class AgentProfile : ScriptableObject
{
    public float maxSpeed = 5f;
    public float acceleration = 10f;
    public float turnSpeed = 180f;
    public float turnCostMultiplier = 1.2f;
    public PathAlgorithm pathAlgorithm = PathAlgorithm.AStar;
    // Add more fields for cost modifiers, etc.
}