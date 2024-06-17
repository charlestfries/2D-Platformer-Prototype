// Extensive commenting for my own learning
// Import necessary Unity libraries
using UnityEngine;

// Allows class to be attached to GameObjects
public class CameraFollow : MonoBehaviour
{
    //Public variable to assign the player's Transform component
    public Transform playerTransform;
    //Public Vector3 variable to set the distance between the camera and the player
    public Vector3 offset;
    //Public float variable to control the speed of the camera's smooth following effect
    public float smoothSpeed = 0.125f;
    public float leftLimit = 0f;

    //FixedUpdate is called at a consistent rate; suitable for physics-related updates
    void FixedUpdate()
    {
        // Calculate the camera's position based on the player's position and the offset
        Vector3 desiredPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z) + offset;
        // Prevent the camera from moving left at the start of the level
        if (desiredPosition.x < leftLimit)
        {
            desiredPosition.x = leftLimit;
        }
        // Lock the camera to prevent vertical movement
        desiredPosition.y = transform.position.y;
        // Smoothly transition from the camera's current position to the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        // Update the camera's position to the smoothed position
        transform.position = smoothedPosition;
    }
}
