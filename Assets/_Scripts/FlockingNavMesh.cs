using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlockingNavMesh : MonoBehaviour
{
    public Transform targetPosition;
    public float neighborRadius = 3f;
    public float separationRadius = 1.5f;
    public float cohesionWeight = 1f;
    public float alignmentWeight = 1f;
    public float separationWeight = 2f;

    private NavMeshAgent navAgent;
    private List<Transform> flockmates;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.SetDestination(targetPosition.position);

        flockmates = new List<Transform>();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Flockmate"))
        {
            flockmates.Add(obj.transform);
        }
    }

    void Update()
    {
        Vector3 flockingForce = CalculateFlockingForce();

        navAgent.velocity = navAgent.desiredVelocity + flockingForce;


        // Optionally, you can also re-set the destination to ensure agents keep moving towards the target
        //navAgent.SetDestination(targetPosition.position);
    }

    Vector3 CalculateFlockingForce()
    {
        Vector3 cohesion = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 separation = Vector3.zero;
        int neighborCount = 0;

        foreach (Transform mate in flockmates)
        {
            if (mate == transform) continue;

            float distance = Vector3.Distance(transform.position, mate.position);
            if (distance <= neighborRadius)
            {
                cohesion += mate.position;
                alignment += mate.forward;
                if (distance < separationRadius)
                {
                    separation += (transform.position - mate.position);
                }

                neighborCount++;
            }
        }

        if (neighborCount == 0) return Vector3.zero;

        cohesion = (cohesion / neighborCount - transform.position).normalized;
        alignment = (alignment / neighborCount).normalized;
        separation = (separation / neighborCount).normalized;

        return cohesion * cohesionWeight + alignment * alignmentWeight + separation * separationWeight;
    }
}