using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpringScript : NetworkBehaviour
{
    public string[] playerTags;
    public float launchForce = 10f;
    public BoxCollider2D childCollider;

    [SyncVar]
    private bool isAnimating;

    private NetworkAnimator networkAnimator;
    private Rigidbody2D playerRigidbody;
    public AudioSource audioSource; // Reference to the AudioSource component
    public AudioClip audioClip; // The audio clip to be played

    private void Start()
    {
        networkAnimator = GetComponent<NetworkAnimator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isServer && !isAnimating)
        {
            foreach (string tag in playerTags)
            {
                if (collision.CompareTag(tag))
                {
                    isAnimating = true;
                    networkAnimator.SetTrigger("Stepped On");
                    playerRigidbody = collision.GetComponent<Rigidbody2D>();
                    LaunchPlayer();
                    break;
                }
            }
        }
    }

    private void Update()
    {
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

    [Server]
    public void LaunchPlayer()
    {
        if (playerRigidbody != null)
        {
            Debug.Log("Launching player!");

            if (audioSource != null && audioClip != null)
            {
                audioSource.PlayOneShot(audioClip);
            }

            Vector2 launchDirection = transform.up;
            playerRigidbody.AddForce(launchDirection * launchForce, ForceMode2D.Impulse);

            RpcLaunchPlayer(); // Call the Rpc to perform the same action on clients
        }
        else
        {
            Debug.Log("Player Rigidbody is missing!");
        }

        isAnimating = false;
    }

    [ClientRpc]
    private void RpcLaunchPlayer()
    {
        if (!isServer)
        {
            LaunchPlayer();
        }
    }

    [Server]
    public void ResetObject()
    {
        networkAnimator.SetTrigger("Sprung");
        RpcResetObject();
    }

    [ClientRpc]
    private void RpcResetObject()
    {
        if (!isServer)
        {
            networkAnimator.SetTrigger("Sprung");
        }
    }
}
