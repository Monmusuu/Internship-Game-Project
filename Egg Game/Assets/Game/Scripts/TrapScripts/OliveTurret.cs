using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class OliveTurret : NetworkBehaviour
{
    public Transform projectileSpawnPoint;
    public GameObject projectilePrefab;
    public Animator animator;

    [Server]
    public void ActivateFunction()
    {
        animator.SetTrigger("Activate");

        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        OliveOil projectileScript = projectile.GetComponent<OliveOil>();
        if (projectileScript != null)
        {
            // Set the direction of the projectile based on the trap microphone's rotation
            Vector3 direction = projectileSpawnPoint.transform.right; // Use the upward direction of the projectileSpawnPoint
            projectileScript.SetDirection(direction);

            // Adjust the projectile's rotation to match the turret's rotation
            Quaternion adjustedRotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z);
            projectile.transform.rotation = adjustedRotation;

            // Spawn the projectile on the network
            NetworkServer.Spawn(projectile);
        }
        else
        {
            Debug.LogWarning("The projectile prefab is missing the OliveOil component!");
        }
    }

}