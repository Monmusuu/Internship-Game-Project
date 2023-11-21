using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FlameThrower : NetworkBehaviour
{
    public Animator animator;
    public Collider2D smallCollider;
    public Collider2D largeCollider;

    private void Start() {
        largeCollider.enabled = false;
    }

    [Server]
    public void ActivateFunction()
    {
        // Start burning
        RpcActivateFlameThrower(true);
        StartCoroutine(DeactivateAfterDelay(2.0f)); // Adjust the delay as needed
    }

    [ClientRpc]
    void RpcActivateFlameThrower(bool activate)
    {
        animator.SetBool("Burn", activate);

        // Enable or disable colliders based on the activation state
        smallCollider.enabled = !activate;
        largeCollider.enabled = activate;
    }

    IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Stop burning after the delay
        RpcActivateFlameThrower(false);
    }
}