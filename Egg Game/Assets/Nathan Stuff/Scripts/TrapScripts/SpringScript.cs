using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringScript : MonoBehaviour
{
public string[] playerTags; // Tags of the players that can trigger the spring
    public float launchForce = 10f; // Magnitude of the upward force to be applied

    public BoxCollider2D childCollider; // Reference to the child collider to be moved

    private bool isAnimating;
    private Animator animator;
    private Rigidbody2D playerRigidbody; // Reference to the player's Rigidbody2D component

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAnimating)
        {
            foreach (string tag in playerTags)
            {
                if (collision.CompareTag(tag))
                {
                    isAnimating = true;
                    animator.SetTrigger("Stepped On");
                    playerRigidbody = collision.GetComponent<Rigidbody2D>(); // Assign the player's Rigidbody2D component
                    break;
                }
            }
        }
    }

    private void Update()
    {
        // Update the position of the child collider to match the top of the sprite
        UpdateChildColliderPosition();
    }

    private void UpdateChildColliderPosition()
    {
        if (childCollider != null)
        {
            Vector2 spriteSize = GetSpriteSize();
            Vector2 colliderOffset = childCollider.offset;
            colliderOffset.y = spriteSize.y / 2f + childCollider.size.y / 2f;
            childCollider.offset = colliderOffset;
        }
        else
        {
            Debug.LogWarning("Child collider not assigned!");
        }
    }

    private Vector2 GetSpriteSize()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            return spriteRenderer.sprite.bounds.size;
        }

        return Vector2.zero;
    }

    public void LaunchPlayer()
    {
        if (playerRigidbody != null)
        {
            Debug.Log("Launching player!");
            // Apply an upward force to the player
            playerRigidbody.AddForce(Vector2.up * launchForce, ForceMode2D.Impulse);
        }
        else
        {
            Debug.Log("Player Rigidbody is missing!");
        }

        isAnimating = false;
    }

    public void ResetObject()
    {
        animator.SetTrigger("Sprung"); 
    }
}