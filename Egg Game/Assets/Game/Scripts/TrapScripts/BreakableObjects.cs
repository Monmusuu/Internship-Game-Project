using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObjects : MonoBehaviour
{
    public Animator animator; // Reference to the Animator component
    public Collider2D colliderToDeactivate; // Collider to deactivate after animation
    public float destroyDelay = 1f; // Delay before destroying the game object
    public AudioSource audioSource; // Reference to the AudioSource component
    public AudioClip audioClip; // The audio clip to be played

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2") || other.CompareTag("Player3")
            || other.CompareTag("Player4") || other.CompareTag("Player5") || other.CompareTag("Player6"))
        {
            // Play the activation audio clip on all clients
            audioSource.PlayOneShot(audioClip);
            animator.SetTrigger("Stepped On");
            Invoke(nameof(DeactivateColliderAndDestroy), destroyDelay);
        }
    }

    private void DeactivateColliderAndDestroy()
    {
        
        colliderToDeactivate.enabled = false;
        Destroy(gameObject);
    }
}
