using UnityEngine;

public class MapCursor : MonoBehaviour
{
    public int cursorID; // Unique identifier for the cursor
    public float moveSpeed = 5f;

    public MapSelection mapSelection; // Reference to the MapSelection script

    private void Start()
    {
        // Find and store the MapSelection script in the scene
        mapSelection = FindObjectOfType<MapSelection>();
    }

    private void Update()
    {
        // Get the input for the current cursor
        Vector2 input = GetCursorInput();

        // Update the position of the cursor based on the input
        Vector3 newPos = transform.position + new Vector3(input.x, input.y, 0f) * moveSpeed * Time.deltaTime;
        transform.position = newPos;
    }

    private Vector2 GetCursorInput()
    {
        // Get the input for the current cursor from the MapSelection script
        Vector2 input = mapSelection.GetCursorInput(cursorID);

        return input;
    }
}
