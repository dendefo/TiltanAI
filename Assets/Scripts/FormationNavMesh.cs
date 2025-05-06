using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FormationNavMesh : MonoBehaviour
{
    public Transform targetPosition;
    public float formationSpacing = 2f; // Spacing between agents in the formation

    private NavMeshAgent navAgent;
    private List<Transform> agents;

    private Vector3 targetFormationPosition;
    private int index;
    void Start()
    {

        index = GetChildIndex();
        
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.SetDestination(targetPosition.position);

        agents = new List<Transform>();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Flockmate"))
        {
            agents.Add(obj.transform);
        }
    }

    void Update()
    {
        // Calculate formation offset
        Vector3 formationOffset = CalculateFormationOffset();

        // Get pathfinding desired velocity
        Vector3 pathFollowForce = navAgent.desiredVelocity;

        // Combine forces
        Vector3 combinedForce = pathFollowForce + formationOffset;
        combinedForce = Vector3.ClampMagnitude(combinedForce, navAgent.speed);

        // Apply the combined force to the agent's movement
        navAgent.velocity = combinedForce;

        // Optionally, you can also re-set the destination to ensure agents keep moving towards the target
        navAgent.SetDestination(targetFormationPosition);
    }

    Vector3 CalculateFormationOffset()
    {
        // Determine the index of this agent in the agents list
        int index = agents.IndexOf(transform);
        gameObject.name = "Agent " + index.ToString();
        
        // Calculate the target position in the formation based on the index
        int formationSize = (int)Mathf.Sqrt(agents.Count);
        int row = index / formationSize;
        int column = index % formationSize;

        // Calculate the offset for this agent in the formation
        float offsetX = (column - (formationSize - 1) / 2f) * formationSpacing;
        float offsetZ = (row - (formationSize - 1) / 2f) * formationSpacing;

        targetFormationPosition = targetPosition.position + new Vector3(offsetX, 0, offsetZ);
        Vector3 offset = targetFormationPosition - transform.position;

        return offset.normalized;
    }

    
    void OnDrawGizmos()
    {
        if (agents == null) return;

        for (int i = 0; i < agents.Count; i++)
        {
            int row = i / (int)Mathf.Sqrt(agents.Count);
            int column = i % (int)Mathf.Sqrt(agents.Count);

            Vector3 targetFormationPosition = targetPosition.position + new Vector3(column * formationSpacing, 0, row * formationSpacing);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(targetFormationPosition, 5f);
        }
    }


    public int GetChildIndex()
    {
        // Check if the GameObject has a parent
        if (transform.parent == null)
        {
            Debug.LogError("This GameObject has no parent.");
            return -1;
        }

        // Get the parent Transform
        Transform parentTransform = transform.parent;

        // Iterate over the children of the parent
        for (int i = 0; i < parentTransform.childCount; i++)
        {
            if (parentTransform.GetChild(i) == transform)
            {
                // Return the index if the current GameObject matches
                return i;
            }
        }
        
        Debug.LogError("This GameObject is not found among its parent's children.");
        return -1;
    }
}