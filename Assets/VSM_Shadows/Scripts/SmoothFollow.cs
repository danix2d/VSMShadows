using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    public Transform target;  // The object to follow
    public float smoothSpeed = 0.125f;  // Speed of smoothing
    public Vector3 offset;  // Offset from the target

    private Camera mainCamera;
    
    void Start()
    {
        // Get the main camera
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (target == null || mainCamera == null)
            return;

        // Get the screen position of the target
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(target.position);

        // Convert screen position to world position
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        worldPosition.y = transform.position.y;  // Keep the original height of the follower

        // Calculate the target position based on the center point and offset
        Vector3 targetPosition = new Vector3(worldPosition.x + offset.x, transform.position.y, worldPosition.z + offset.z);
        
        // Smoothly move the follower towards the target position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}