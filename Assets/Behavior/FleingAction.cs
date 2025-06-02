using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Fleing", story: "[Self] running away from [Enemy]", category: "Action", id: "7f7091b6abab8b8dabd69369fef98079")]
public partial class FleingAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Enemy;
    private NavMeshAgent m_NavMeshAgent;

    protected override Status OnStart()
    {
        m_NavMeshAgent = Self.Value.GetComponent<NavMeshAgent>();
        if (m_NavMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent component is missing on the Self GameObject.");
            return Status.Failure;
        }
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Enemy.Value == null)
        {
            //Enemy is null means we succesfully fled from it and dont sense it anymore
            return Status.Success;
        }
        if (Self.Value == null)
        {
            Debug.LogError("Self GameObject is not set.");
            return Status.Failure;
        }
        if (m_NavMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent component is missing on the Self GameObject.");
            return Status.Failure;
        }
        // Calculate the direction away from the enemy
        Vector3 directionAwayFromEnemy = (Self.Value.transform.position - Enemy.Value.transform.position).normalized;
        // Calculate a point far away in that direction
        Vector3 fleeTarget = Self.Value.transform.position + directionAwayFromEnemy * 10f; // Adjust the distance as needed

        NavMeshHit hit;
        if (!NavMesh.SamplePosition(fleeTarget, out hit, 10f, NavMesh.AllAreas))
        {
            Debug.LogError("Failed to find a valid flee target on the NavMesh.");
            return Status.Failure;
        }

        m_NavMeshAgent.SetDestination(fleeTarget);

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

