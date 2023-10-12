using TMPro.Examples;
using UnityEngine;

public class PlayerPlatformSync: MonoBehaviour
{
    private Transform currentPlatform; // Reference to the current platform the player is standing on
    private Vector3 platformRelativePosition; // Player's relative position on the platform

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Platform")) // Change "Platform" to the appropriate tag of your child platforms
        {
            Debug.Log("On platform");
            currentPlatform = other.transform;
            platformRelativePosition = currentPlatform.InverseTransformPoint(transform.position);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform == currentPlatform)
        {
            currentPlatform = null;
        }
    }

    private void FixedUpdate()
    {
        if (currentPlatform != null)
        {
            Vector3 targetPosition = currentPlatform.TransformPoint(platformRelativePosition);
            Vector3 moveDirection = targetPosition - transform.position;

            // Update player's position to stay on the moving platform
            transform.position += moveDirection;
        }
    }
}
