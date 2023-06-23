using UnityEngine;

public class IceBlock : MonoBehaviour
{
    public float slideForce = 5f;
    public float iceFriction = 0.2f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player1") ||
            collision.gameObject.CompareTag("Player2") ||
            collision.gameObject.CompareTag("Player3") ||
            collision.gameObject.CompareTag("Player4") ||
            collision.gameObject.CompareTag("Player5") ||
            collision.gameObject.CompareTag("Player6"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 slideDirection = playerRb.velocity.normalized;
                playerRb.AddForce(slideDirection * slideForce, ForceMode2D.Impulse);

                Debug.Log("Player collided with ice block: " + collision.gameObject.name);
                Debug.Log("Slide Direction: " + slideDirection);
                Debug.Log("Slide Force: " + slideDirection * slideForce);

                AdjustFriction(collision, iceFriction);
            }
        }
    }

    private void AdjustFriction(Collider2D collider, float friction)
    {
        PhysicsMaterial2D physicsMaterial = new PhysicsMaterial2D();
        physicsMaterial.friction = friction;

        collider.sharedMaterial = physicsMaterial;
    }
}