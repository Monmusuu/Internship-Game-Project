using UnityEngine;

public class ChildTileRotation : MonoBehaviour
{
    // Collision event for 2D objects
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Loop through the player tags
        for (int i = 1; i <= 6; i++)
        {
            string tagToCheck = "Player" + i;

            // Check if the collided game object has the desired tag
            if (collision.gameObject.CompareTag(tagToCheck))
            {
                // Set the collided game object as a child of this game object
                collision.gameObject.transform.SetParent(transform);
                break;
            }
        }
    }

        // Collision exit event for 2D objects
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Loop through the player tags
        for (int i = 1; i <= 6; i++)
        {
            string tagToCheck = "Player" + i;

            // Check if the collided game object has the desired tag
            if (collision.gameObject.CompareTag(tagToCheck))
            {
                // Unchild the collided game object from this game object
                collision.gameObject.transform.SetParent(null);
                break;
            }
        }
    }
}