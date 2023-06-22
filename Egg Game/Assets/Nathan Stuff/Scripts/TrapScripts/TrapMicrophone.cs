using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapMicrophone : MonoBehaviour
{
    public Transform projectileSpawnPoint;
    public GameObject projectilePrefab;

    public void ActivateFunction()
    {
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        SoundWave projectileScript = projectile.GetComponent<SoundWave>();
        if (projectileScript != null)
        {
            // Set the direction of the projectile based on the trap microphone's rotation
            Vector3 direction = projectileSpawnPoint.up;
            Quaternion rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z);
            
            if (Mathf.Approximately(rotation.eulerAngles.z, 90f) || Mathf.Approximately(rotation.eulerAngles.z, 270f))
            {
                direction = -direction; // Reverse direction if z rotation is 90 or 270 degrees
            }

            direction = rotation * direction;
            projectileScript.SetDirection(direction);

            // Set the z rotation of the projectile to match the trap microphone's z rotation
            Vector3 eulerRotation = projectile.transform.rotation.eulerAngles;
            eulerRotation.z = transform.rotation.eulerAngles.z;
            projectile.transform.rotation = Quaternion.Euler(eulerRotation);
        }
        else
        {
            Debug.LogWarning("The projectile prefab is missing the SoundWave component!");
        }
    }
}