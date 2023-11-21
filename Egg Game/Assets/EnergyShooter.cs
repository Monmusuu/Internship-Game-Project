using System.Collections;
using UnityEngine;
using Mirror;

public class EnergyShooter : NetworkBehaviour
{
    public Transform projectileSpawnPoint;
    public GameObject projectilePrefab;
    public RoundControl roundControl;
    private bool roundControlFound = false;

    // Adjustable rate for shooting
    public float shootCooldown = 7.0f;
    private float shootTimer = 0.0f;

    private void Start()
    {
        roundControlFound = false;
        StartCoroutine(WaitForRoundControl());
    }

    IEnumerator WaitForRoundControl()
    {
        while (true)
        {
            GameObject roundControlObject = GameObject.Find("RoundControl(Clone)");

            if (roundControlObject != null)
            {
                roundControl = roundControlObject.GetComponent<RoundControl>();
                roundControlFound = true;
                break;
            }

            yield return null; // Wait for a frame before checking again
        }
    }

    private void Update()
    {
        // Check if the roundControl is available and the timer is ready
        if (roundControl != null && roundControl.timerOn && CanShoot())
        {
            ShootBall();
            ResetShootTimer();
        }

        // Update the shoot timer
        UpdateShootTimer();
    }

    private void UpdateShootTimer()
    {
        if (shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
        }
    }

    private bool CanShoot()
    {
        return shootTimer <= 0.0f;
    }

    private void ResetShootTimer()
    {
        shootTimer = shootCooldown;
    }

    public void ShootBall()
    {
        // GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        // EnergyBallProjectile projectileScript = projectile.GetComponent<EnergyBallProjectile>();

        // if (projectileScript != null)
        // {
        //     // Set the direction of the projectile based on the trap microphone's rotation
        //     Vector3 direction = projectileSpawnPoint.transform.up;
        //     projectileScript.SetDirection(direction);

        //     // Set the z rotation of the projectile to match the trap microphone's z rotation
        //     Vector3 eulerRotation = projectile.transform.rotation.eulerAngles;
        //     eulerRotation.z = transform.rotation.eulerAngles.z;
        //     projectile.transform.rotation = Quaternion.Euler(eulerRotation);

        //     // Spawn the projectile on the network
        //     NetworkServer.Spawn(projectile);
        // }
        // else
        // {
        //     Debug.LogWarning("The projectile prefab is missing the SoundWave component!");
        // }
    }
}

