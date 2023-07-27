using Mirror;
using UnityEngine;

public class Weapon : NetworkBehaviour
{
    [SerializeField] private float knockbackStrength = 20.0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ensure this is only executed on the server
        if (!isServer) return;

        Player hitPlayer = other.GetComponent<Player>();
        Player ownerPlayer = transform.parent.GetComponent<Player>();

        // Check if the hit object is a Player, and it's not the owner of this weapon
        if (hitPlayer != null && ownerPlayer != null && hitPlayer != ownerPlayer)
        {
            // Calculate knockback direction
            Vector2 direction = other.transform.position - transform.position;
            direction.y = 4;

            // Apply knockback to the hit player on the server
            RpcApplyKnockbackOnClients(other.gameObject, direction);
        }
    }

    [ClientRpc]
    private void RpcApplyKnockbackOnClients(GameObject hitPlayerObject, Vector2 direction)
    {
        // Apply knockback to the hit player on the clients
        if (hitPlayerObject != null)
        {
            Rigidbody2D m_rigidbody = hitPlayerObject.GetComponent<Rigidbody2D>();
            if (m_rigidbody != null)
            {
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
            }
        }
    }
}
