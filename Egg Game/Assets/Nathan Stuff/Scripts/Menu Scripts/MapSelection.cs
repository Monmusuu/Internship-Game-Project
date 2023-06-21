using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class MapSelection : MonoBehaviour
{
    public VotingSystem votingSystem;
    public float speed = 5f;

    private PlayerInput playerInput;
    private Vector2 movementInput;

    private void Start()
    {
        votingSystem = GameObject.Find("VotingSystem").GetComponent<VotingSystem>();
        playerInput = GetComponent<PlayerInput>();
        UnityEngine.Cursor.visible = false;
    }

    private void Update()
    {
        if (votingSystem.HasVoted(playerInput.playerIndex))
        {
            // Player has voted, stop movement
            return;
        }

        // Process movement input
        Vector2 movement = movementInput.normalized;
        Vector3 newPosition = transform.position + new Vector3(movement.x, movement.y, 0) * speed * Time.deltaTime;

        // Update the cursor position based on input
        if (playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            // Get the mouse position in world coordinates
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Clamp the cursor position to stay within the screen boundaries
            ClampPosition(mousePosition);
        }
        else if (playerInput.currentControlScheme == "Gamepad")
        {
            ClampPosition(newPosition);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    private void ClampPosition(Vector3 position)
    {
        // Get the screen boundaries in world coordinates
        float screenAspect = (float)Screen.width / Screen.height;
        float screenHorizontalSize = Camera.main.orthographicSize * screenAspect;
        float screenVerticalSize = Camera.main.orthographicSize;

        // Get the object's dimensions
        Renderer renderer = GetComponent<Renderer>();
        float objectWidth = renderer.bounds.size.x;
        float objectHeight = renderer.bounds.size.y;

        // Clamp the position to stay within the screen boundaries
        float clampedX = Mathf.Clamp(position.x, -screenHorizontalSize + objectWidth / 2f, screenHorizontalSize - objectWidth / 2f);
        float clampedY = Mathf.Clamp(position.y, -screenVerticalSize + objectHeight / 2f, screenVerticalSize - objectHeight / 2f);
        float clampedZ = transform.position.z;

        transform.position = new Vector3(clampedX, clampedY, clampedZ);
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            int playerID = playerInput.playerIndex;

            if (!votingSystem.HasVoted(playerID))
            {
                // Get the map objects with the "Map" tag
                GameObject[] mapObjects = GameObject.FindGameObjectsWithTag("Map");

                // Find the exact map index
                int exactMapIndex = GetExactMapIndex(mapObjects);

                if (exactMapIndex != -1)
                {
                    // Vote for the selected map
                    votingSystem.Vote(playerID, exactMapIndex);
                    Debug.Log("Player " + playerID + " voted for map index: " + exactMapIndex);
                }
            }
            else
            {
                // Undo the player's vote
                votingSystem.UndoVote(playerID);
            }
        }
    }

private int GetExactMapIndex(GameObject[] mapObjects)
{
    int mapIndex = -1;
    Vector3 playerPosition = transform.position;

    for (int i = 0; i < mapObjects.Length; i++)
    {
        GameObject mapObject = mapObjects[i];

        // Check for CanvasRenderer
        CanvasRenderer canvasRenderer = mapObject.GetComponent<CanvasRenderer>();
        if (canvasRenderer != null)
        {
            RectTransform rectTransform = mapObject.GetComponent<RectTransform>();
            if (rectTransform != null && RectTransformUtility.RectangleContainsScreenPoint(rectTransform, playerPosition))
            {
                // Player is within the bounds of the current map object
                mapIndex = i;
                break;
            }
        }

        // Check for SpriteRenderer
        SpriteRenderer spriteRenderer = mapObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            if (spriteRenderer.bounds.Contains(playerPosition))
            {
                // Player is within the bounds of the current map object
                mapIndex = i;
                break;
            }
        }
    }

    return mapIndex;
}
}
