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
        OliveOil projectileScript = projectile.GetComponent<OliveOil>();
        if (projectileScript != null)
        {
            Vector3 direction = -projectileSpawnPoint.right;
            direction = Quaternion.Euler(0f, 0f, transform.eulerAngles.z) * direction;
            projectileScript.SetDirection(direction);
        }
        else
        {
            Debug.LogWarning("The projectile prefab is missing the OliveOil component!");
        }
    }
}
