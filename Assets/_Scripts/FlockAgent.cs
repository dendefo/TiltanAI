using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements flocking behavior based on Craig Reynolds' Boids algorithm
/// Controls how individual agents move within a flock
/// </summary>
public class FlockAgent : MonoBehaviour
{
    // Base movement speed of the agent
    public float speed = 5f;
    
    // Radius within which this agent will look for neighbors
    public float neighborRadius = 3f;
    
    // Minimum distance to maintain from other agents
    public float separationDistance = 1.5f;

    void Update()
    {
        // Initialize vectors for the three flocking rules
        Vector3 alignment = Vector3.zero;   // Match velocity with neighbors
        Vector3 cohesion = Vector3.zero;    // Move toward center of neighbors
        Vector3 separation = Vector3.zero;   // Avoid crowding neighbors
        int neighborCount = 0;

        // Find all colliders within neighborRadius using Physics.OverlapSphere
        Collider[] neighbors = Physics.OverlapSphere(transform.position, neighborRadius);
        foreach (var neighbor in neighbors)
        {
            // Skip self when checking neighbors
            if (neighbor.gameObject == this.gameObject) continue;

            Vector3 toNeighbor = neighbor.transform.position - transform.position;
            float distance = toNeighbor.magnitude;

            // Alignment: Add up all neighbors' forward directions
            alignment += neighbor.transform.forward;
            // Cohesion: Add up all neighbors' positions
            cohesion += neighbor.transform.position;

            // Separation: Add repulsion force if neighbor is too close
            // Force is stronger when neighbors are closer (inverse to distance)
            if (distance < separationDistance)
                separation -= toNeighbor.normalized / distance;

            neighborCount++;
        }

        // Average out the accumulated forces if we have neighbors
        if (neighborCount > 0)
        {
            alignment /= neighborCount;  // Average alignment force
            // Calculate direction to center of mass for cohesion
            cohesion = (cohesion / neighborCount - transform.position).normalized;
        }

        // Combine all three flocking behaviors into final movement direction
        Vector3 moveDirection = alignment + cohesion + separation;

        // Gradually rotate the agent towards the desired direction
        if (moveDirection != Vector3.zero)
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * 5f);

        // Move the agent forward at constant speed
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}