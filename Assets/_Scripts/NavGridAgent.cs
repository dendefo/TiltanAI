using UnityEngine;

public class NavGridAgent : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float cellSize = 1f;
    
    [Header("Grid Parameters")]
    [SerializeField] private bool allowDiagonalMovement = true;
    [SerializeField] private LayerMask obstacleLayer;
    
    [Header("Navigation Tracking")]
    [SerializeField] private bool trackNavigationStats = true;
    [SerializeField] private float navigationTimeout = 30f; // Maximum time allowed for navigation
    
    [Header("Visualization")]
    [SerializeField] private bool showDebugPath = false;
    [SerializeField] private Color pathColor = Color.green;

    // Navigation tracking properties
    private float currentNavigationTime = 0f;
    private float totalNavigationTime = 0f;
    private int navigationAttempts = 0;
    private int successfulNavigations = 0;
    private int timeoutNavigations = 0;
    
    private Vector3 targetPosition;
    private bool isMoving = false;
    private Vector3 startPosition; // Store start position for distance calculation
    private float totalDistanceTraveled = 0f;
    private Vector3 lastPosition;
    
    // Current grid position
    private Vector2Int currentGridPosition;
    
    void Start()
    {
        targetPosition = transform.position;
        lastPosition = transform.position;
    }
    
    void Update()
    {
        if (isMoving)
        {
            if (trackNavigationStats)
            {
                UpdateNavigationStats();
            }
            MoveTowardsTarget();
        }
    }

    private void UpdateNavigationStats()
    {
        currentNavigationTime += Time.deltaTime;
        totalDistanceTraveled += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        // Check for timeout
        if (currentNavigationTime >= navigationTimeout)
        {
            TimeoutNavigation();
        }
    }

    private void TimeoutNavigation()
    {
        isMoving = false;
        timeoutNavigations++;
        ResetNavigationTimer();
    }

    public void SetDestination(Vector3 destination)
    {
        targetPosition = SnapToGrid(destination);
        isMoving = true;
        navigationAttempts++;
        startPosition = transform.position;
        ResetNavigationTimer();
    }
    
    private void ResetNavigationTimer()
    {
        currentNavigationTime = 0f;
    }
    
    private void MoveTowardsTarget()
    {
        if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            
            // Move
            transform.position += direction * moveSpeed * Time.deltaTime;
            
            // Rotate
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            CompleteNavigation();
        }
    }

    private void CompleteNavigation()
    {
        transform.position = targetPosition;
        isMoving = false;
        
        if (trackNavigationStats)
        {
            successfulNavigations++;
            totalNavigationTime += currentNavigationTime;
        }
    }

    // Get navigation statistics
    public (int attempts, int successful, int timeouts) GetNavigationAttempts()
    {
        return (navigationAttempts, successfulNavigations, timeoutNavigations);
    }

    public float GetAverageNavigationTime()
    {
        return successfulNavigations > 0 ? totalNavigationTime / successfulNavigations : 0f;
    }

    public float GetCurrentNavigationTime()
    {
        return currentNavigationTime;
    }

    public float GetTotalDistanceTraveled()
    {
        return totalDistanceTraveled;
    }

    public float GetCurrentPathDistance()
    {
        return isMoving ? Vector3.Distance(startPosition, targetPosition) : 0f;
    }
    
    private Vector3 SnapToGrid(Vector3 position)
    {
        float x = Mathf.Round(position.x / cellSize) * cellSize;
        float z = Mathf.Round(position.z / cellSize) * cellSize;
        return new Vector3(x, position.y, z);
    }
    
    private bool IsValidGridPosition(Vector2Int gridPos)
    {
        Vector3 worldPos = new Vector3(gridPos.x * cellSize, transform.position.y, gridPos.y * cellSize);
        return !Physics.CheckSphere(worldPos, cellSize * 0.4f, obstacleLayer);
    }
    
    private void OnDrawGizmos()
    {
        if (showDebugPath)
        {
            Gizmos.color = pathColor;
            Gizmos.DrawWireSphere(targetPosition, 0.3f);
            Gizmos.DrawLine(transform.position, targetPosition);
            
            // Draw navigation stats if tracking is enabled
            if (trackNavigationStats && isMoving)
            {
                // Draw current navigation time
                UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, 
                    $"Time: {currentNavigationTime:F2}s\nDistance: {GetCurrentPathDistance():F2}m");
            }
        }
    }
}