using System.Collections;
using UnityEngine;
using Mirror;

public class MouseTrap : NetworkBehaviour
{
    public Animator animator;
    public Collider2D attackCollider;
    private bool isOnCooldown = false;
    public AudioSource audioSource; // Reference to the AudioSource component
    public AudioClip audioClip; // The audio clip to be played
    
    // OnTriggerEnter2D is called when another collider enters the trigger zone
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the other collider belongs to a player with the Player script
        Player playerScript = other.GetComponent<Player>();

        if (playerScript != null && !isOnCooldown)
        {
            // Start the cooldown coroutine
            StartCoroutine(ActivateCooldown());

            // Set the animation parameter to true
            animator.SetBool("Activate", true);

            // Activate the attack collider
            attackCollider.enabled = true;
        }
    }

    // Cooldown coroutine
    private IEnumerator ActivateCooldown()
    {
        // Set the cooldown flag
        isOnCooldown = true;

        // Play the activation audio clip on all clients
        audioSource.PlayOneShot(audioClip);

        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);

        attackCollider.enabled = false;

        animator.SetBool("Activate", false);

        // Reset the cooldown flag
        isOnCooldown = false;
    }
}

