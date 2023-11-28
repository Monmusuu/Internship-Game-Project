using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class VacuumSuck : NetworkBehaviour
{
    public float suctionSpeed = 100f; // Adjustable speed from the Unity Editor
    public Transform targetObject;    // Set this in the Unity Editor to specify the target object

    private void OnTriggerStay2D(Collider2D other)
    {
        // Check if the other collider has a Rigidbody and the Player script
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        Player playerScript = other.GetComponent<Player>();

        if (rb != null && playerScript != null && !playerScript.isKing)
        {
            // Calculate the direction towards the target object and apply force based on the adjustable speed
            Vector3 direction = targetObject.position - rb.transform.position;
            rb.AddForce(direction.normalized * suctionSpeed, ForceMode2D.Force);

            // Debug information
            Debug.Log($"Sucking: {other.name} towards {targetObject.name} with force {direction.normalized * suctionSpeed}");
        }
        else
        {
            // Debug information if the collider doesn't meet the conditions
            // if (rb == null)
            //     Debug.Log($"No Rigidbody found on {other.name}");
            // if (playerScript == null)
            //     Debug.Log($"No Player script found on {other.name}");
            // if (playerScript != null && playerScript.isKing)
            //     Debug.Log($"{other.name} is the King and cannot be sucked.");
        }
    }
}
