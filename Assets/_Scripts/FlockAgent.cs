using System.Collections.Generic;
using UnityEngine;

public class FlockAgent : MonoBehaviour
{
    public float speed = 5f;
    public float neighborRadius = 3f;
    public float separationDistance = 1.5f;

    void Update()
    {
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;
        Vector3 separation = Vector3.zero;
        int neighborCount = 0;

        // Find nearby flockmates
        Collider[] neighbors = Physics.OverlapSphere(transform.position, neighborRadius);
        foreach (var neighbor in neighbors)
        {
            if (neighbor.gameObject == this.gameObject) continue;

            Vector3 toNeighbor = neighbor.transform.position - transform.position;
            float distance = toNeighbor.magnitude;

            // Accumulate alignment and cohesion
            alignment += neighbor.transform.forward;
            cohesion += neighbor.transform.position;

            // Apply separation if too close
            if (distance < separationDistance)
                separation -= toNeighbor.normalized / distance;

            neighborCount++;
        }

        if (neighborCount > 0)
        {
            alignment /= neighborCount;
            cohesion = (cohesion / neighborCount - transform.position).normalized;
        }

        // Combine all behaviors
        Vector3 moveDirection = alignment + cohesion + separation;

        // Move the agent
        if (moveDirection != Vector3.zero)
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * 5f);

        transform.position += transform.forward * speed * Time.deltaTime;
    }
}