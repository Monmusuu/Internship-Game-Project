using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class OliveTurret : NetworkBehaviour
{
    public Transform projectileSpawnPoint;
    public GameObject projectilePrefab;
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip audioClip;

    // Adjustable cooldown parameters
    [SyncVar]public float activationCooldown = 0.5f;
    [SyncVar]private float cooldownTimer = 0f;

    [Server]
    public void ActivateFunction()
    {
        // Check if the cooldown has expired
        if (Time.time > cooldownTimer)
        {
            // Play the activation audio clip
            RpcPlayActivationAudio();

            animator.SetTrigger("Activate");

            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
            OliveOil projectileScript = projectile.GetComponent<OliveOil>();
            if (projectileScript != null)
            {
                Vector3 direction = projectileSpawnPoint.transform.right;
                projectileScript.SetDirection(direction);

                Quaternion adjustedRotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z);
                projectile.transform.rotation = adjustedRotation;

                NetworkServer.Spawn(projectile);
            }
            else
            {
                Debug.LogWarning("The projectile prefab is missing the OliveOil component!");
            }

            // Set the cooldown timer
            cooldownTimer = Time.time + activationCooldown;
        }
        else
        {
            Debug.Log("Turret is on cooldown!");
        }
    }

    [ClientRpc]
    void RpcPlayActivationAudio()
    {
        audioSource.PlayOneShot(audioClip);
    }
}

