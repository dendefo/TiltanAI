using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FormationAroundKing : MonoBehaviour
{
    public float formationSpacing = 2f;
    public float updateInterval = 0.1f;
    public float kingMoveSpeed = 5f;
    public Transform targetPosition;
    public float minimalDistance = 3f;      // Minimal distance followers should keep from each other
    
    private NavMeshAgent navAgent;
    private List<Transform> agents;
    private Transform kingTransform;
    private Vector3 targetFormationPosition;
    private float nextUpdateTime;
    private bool isKing;

    void Start()
    {
        isKing = CompareTag("King");
        
        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent != null && isKing)
        {
            navAgent.speed = kingMoveSpeed;
        }
        
        agents = new List<Transform>();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Flockmate"))
        {
            agents.Add(obj.transform);
        }

        if (isKing)
        {
            kingTransform = transform;
            if (targetPosition == null)
            {
                Debug.LogError("Target position not assigned for the king!");
            }
        }
        else
        {
            GameObject kingObject = GameObject.FindGameObjectWithTag("King");
            if (kingObject != null)
            {
                kingTransform = kingObject.transform;
            }
            else
            {
                Debug.LogError("No game object with tag 'King' found!");
            }
        }
    }

    void Update()
    {
        if (isKing)
        {
            HandleKingMovement();
        }
        else
        {
            HandleFollowerMovement();
        }
    }

    void HandleKingMovement()
    {
        if (targetPosition != null)
        {
            navAgent.SetDestination(targetPosition.position);
        }
    }

    void HandleFollowerMovement()
    {
        if (Time.time >= nextUpdateTime)
        {
            UpdateFormationPosition();
            nextUpdateTime = Time.time + updateInterval;
        }

        if (kingTransform != null)
        {
            Vector3 formationOffset = CalculateFormationOffset();
            Vector3 separationForce = CalculateSeparation();
            Vector3 pathFollowForce = navAgent.desiredVelocity;

            // Combine all forces
            Vector3 combinedForce = pathFollowForce + formationOffset + separationForce;
            combinedForce = Vector3.ClampMagnitude(combinedForce, navAgent.speed);

            navAgent.velocity = combinedForce;
            navAgent.SetDestination(targetFormationPosition);
        }
    }

    Vector3 CalculateSeparation()
    {
        Vector3 separation = Vector3.zero;
        int neighborCount = 0;

        foreach (Transform agent in agents)
        {
            if (agent == transform) continue;

            float distance = Vector3.Distance(transform.position, agent.position);
            if (distance < minimalDistance)
            {
                Vector3 moveAway = transform.position - agent.position;
                separation += moveAway.normalized / distance; // Weight by distance
                neighborCount++;
            }
        }

        if (neighborCount > 0)
        {
            separation /= neighborCount;
            separation = separation.normalized * navAgent.speed;
        }

        return separation;
    }

    void UpdateFormationPosition()
    {
        if (kingTransform == null) return;
        CalculateFormationOffset();
    }

    Vector3 CalculateFormationOffset()
    {
        int index = agents.IndexOf(transform);
        
        float angleStep = 360f / agents.Count;
        float currentAngle = index * angleStep;
        float angleRad = currentAngle * Mathf.Deg2Rad;
        
        // Ensure minimum formation spacing is not less than minimal distance
        float actualFormationSpacing = Mathf.Max(formationSpacing, minimalDistance);
        float radius = actualFormationSpacing * (1 + index / agents.Count);
        
        float x = Mathf.Cos(angleRad) * radius;
        float z = Mathf.Sin(angleRad) * radius;

        targetFormationPosition = kingTransform.position + new Vector3(x, 0, z);
        Vector3 offset = targetFormationPosition - transform.position;
        
        return offset.normalized;
    }

    void OnDrawGizmos()
    {
        if (agents == null || kingTransform == null) return;

        // Draw formation positions
        for (int i = 0; i < agents.Count; i++)
        {
            float angleStep = 360f / agents.Count;
            float currentAngle = i * angleStep * Mathf.Deg2Rad;
            float radius = Mathf.Max(formationSpacing, minimalDistance) * (1 + i / agents.Count);
            
            Vector3 formationPos = kingTransform.position + new Vector3(
                Mathf.Cos(currentAngle) * radius,
                0,
                Mathf.Sin(currentAngle) * radius
            );

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(formationPos, 0.5f);
        }

        // Draw minimum distance radius
        if (!isKing)
        {
            Gizmos.color = new Color(1, 0, 0, 0.2f); // Semi-transparent red
            Gizmos.DrawWireSphere(transform.position, minimalDistance);
        }
    }
}
