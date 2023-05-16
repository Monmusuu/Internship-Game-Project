using UnityEngine;
using UnityEngine.Tilemaps;

public class TileRotation : MonoBehaviour
{
    public KeyCode rotationKey = KeyCode.R;  // Key to rotate the tile

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(rotationKey))
        {
            RotateTile();
        }
    }

    void RotateTile()
    {
        // Get the tile's sprite renderer and rotate it
        SpriteRenderer tileRenderer = GetComponent<SpriteRenderer>();
        tileRenderer.transform.Rotate(Vector3.forward, 90f); // Rotate the tile 90 degrees around the Z-axis
    }
}
